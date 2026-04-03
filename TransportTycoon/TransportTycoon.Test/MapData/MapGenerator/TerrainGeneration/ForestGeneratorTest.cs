using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class ForestGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void ForestGeneratorFactory_Create_WithValidParameters()
        {
            // Arrange
            INoiseGenerator noiseGenerator = Substitute.For<INoiseGenerator>();

            // Act
            IForestGenerator result = ForestGeneratorFactory.Create(noiseGenerator);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ForestGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateForestsTest
    {
        private IForestGenerator _forestGenerator = null!;
        private MapGenerationContext _context = default;
        private int[,] _heightMap = null!;

        private INoiseGenerator GetMockedNoiseGenerator()
        {
            INoiseGenerator noiseGenerator_mock = Substitute.For<INoiseGenerator>();
            noiseGenerator_mock.GenerateNoise(Arg.Any<float>(), Arg.Any<float>(), Arg.Any<int>())
                .Returns(x =>
                {
                    // Deterministic noise based only on seed and coordinates
                    // Use hash function to generate deterministic values from seed + coordinates
                    uint hash = (uint)(((int)x[2] ^ ((int)(float)x[0] * 73856093) ^ ((int)(float)x[1] * 19349663)) * 2654435761);
                    return (float)(hash % 1000) / 1000f; // Values between 0.0f and 1.0f
                });
            return noiseGenerator_mock;
        }

        private int[,] GenerateHeightMap(int width, int height, int extraHeight = 1)
        {
            int[,] heightMap = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // With ExtraHeight, we can create some higher terrain for testing (e.g., all heights = 4, when extraHeight = 4)
                    heightMap[x, y] = 4 - (extraHeight * (x + y)) % 4; // Heights 1-4
                }
            }
            return heightMap;
        }

        [TestInitialize]
        public void Initialize()
        {
            INoiseGenerator noiseGenerator_mock = GetMockedNoiseGenerator();
            _context = new MapGenerationContext(20, 20, 42, new MapGenerationSettings());
            _forestGenerator = ForestGeneratorFactory.Create(noiseGenerator_mock);

            // Create a basic height map for testing
            _heightMap = GenerateHeightMap(_context.Width, _context.Height);
        }

        [TestMethod]
        public void GenerateForests_ReturnsCorrectDimensions()
        {
            // Act
            int[,] forestMap = _forestGenerator.GenerateForests(_heightMap, _context);

            // Assert
            Assert.AreEqual(_context.Width, forestMap.GetLength(0), "Forest map width should match context");
            Assert.AreEqual(_context.Height, forestMap.GetLength(1), "Forest map height should match context");
        }

        [TestMethod]
        public void GenerateForests_AllValuesAreValidForestDensities()
        {
            // Arrange
            int minDensity = 0;
            int maxDensity = 4;

            // Act
            int[,] forestMap = _forestGenerator.GenerateForests(_heightMap, _context);

            // Assert
            bool validDensity = true;
            for (int x = 0; x < _context.Width && validDensity; x++)
            {
                for (int y = 0; y < _context.Height && validDensity; y++)
                {
                    int density = forestMap[x, y];
                    validDensity = minDensity <= density && density <= maxDensity;
                }
            }
            Assert.IsTrue(validDensity, "All forest densities should be valid");
        }

        [TestMethod]
        public void GenerateForests_NoForestsOnVeryHighTerrain()
        {
            // Arrange - Create height map with some very high terrain (height = 4)
            MapGenerationContext smallContext = new(15, 15, 42, new MapGenerationSettings());
            int[,] heightMap = GenerateHeightMap(smallContext.Width, smallContext.Height, extraHeight: 4);

            // Act
            int[,] forestMap = _forestGenerator.GenerateForests(heightMap, smallContext);

            // Assert - Forests should not appear on very high terrain (height >= 4)
            bool hasForest = false;
            for (int x = 0; x < smallContext.Width && !hasForest; x++)
            {
                for (int y = 0; y < smallContext.Height && !hasForest; y++)
                {
                    hasForest = forestMap[x, y] > 0;
                }
            }
            Assert.IsFalse(hasForest, "Forests should not appear on very high terrain");
        }

        [TestMethod]
        public void GenerateForests_LowPercentageProducesFewerTrees()
        {
            // Arrange
            MapGenerationSettings lowSettings = new() { ForestPercentage = 0.1f };
            MapGenerationSettings highSettings = new() { ForestPercentage = 0.8f };
            MapGenerationContext lowContext = new(30, 30, 42, lowSettings);
            MapGenerationContext highContext = new(30, 30, 42, highSettings);
            int[,] heightMap = GenerateHeightMap(highContext.Width, highContext.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator lowForestGen = ForestGeneratorFactory.Create(noiseGen);
            IForestGenerator highForestGen = ForestGeneratorFactory.Create(noiseGen);

            // Act
            int[,] lowForestMap = lowForestGen.GenerateForests(heightMap, lowContext);
            int[,] highForestMap = highForestGen.GenerateForests(heightMap, highContext);

            // Count trees in each
            int lowTreeCount = 0;
            int highTreeCount = 0;
            for (int x = 0; x < lowContext.Width; x++)
            {
                for (int y = 0; y < lowContext.Height; y++)
                {
                    if (lowForestMap[x, y] > 0) lowTreeCount++;
                    if (highForestMap[x, y] > 0) highTreeCount++;
                }
            }

            // Assert - Higher percentage should generally produce more trees
            Assert.IsGreaterThanOrEqualTo(lowTreeCount, highTreeCount, $"Higher forest percentage should produce more trees: low={lowTreeCount}, high={highTreeCount}");
        }

        [TestMethod]
        public void GenerateForests_SameSeedProducesSameResult()
        {
            // Arrange
            MapGenerationContext context1 = new(15, 15, 12345, new MapGenerationSettings());
            MapGenerationContext context2 = new(15, 15, 12345, new MapGenerationSettings());

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator forestGen = ForestGeneratorFactory.Create(noiseGen);

            // Act
            int[,] forest1 = forestGen.GenerateForests(heightMap, context1);
            int[,] forest2 = forestGen.GenerateForests(heightMap, context2);

            // Assert
            bool hasDifferentTrees = false;
            for (int x = 0; x < context1.Width && !hasDifferentTrees; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentTrees; y++)
                {
                    hasDifferentTrees = forest1[x, y] != forest2[x, y];
                }
            }
            Assert.IsFalse(hasDifferentTrees, "Same seed should generate same forest");
        }

        [TestMethod]
        public void GenerateForests_DifferentSeedProducesDifferentesult()
        {
            // Arrange
            MapGenerationContext context1 = new(15, 15, 69, new MapGenerationSettings());
            MapGenerationContext context2 = new(15, 15, 420, new MapGenerationSettings());

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator forestGen = ForestGeneratorFactory.Create(noiseGen);

            // Act
            int[,] forest1 = forestGen.GenerateForests(heightMap, context1);
            int[,] forest2 = forestGen.GenerateForests(heightMap, context2);

            // Assert
            bool hasDifferentTrees = false;
            for (int x = 0; x < context1.Width && !hasDifferentTrees; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentTrees; y++)
                {
                    hasDifferentTrees = forest1[x, y] == forest2[x, y];
                }
            }
            Assert.IsTrue(hasDifferentTrees, "Different seed should generate different forest");
        }
    }
}
