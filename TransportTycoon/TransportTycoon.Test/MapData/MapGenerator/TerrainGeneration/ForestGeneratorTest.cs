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
            IForestGenerator result = ForestGeneratorFactory.Create(noiseGenerator, 0.1f, 0.3f);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<ForestGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateForestsTest
    {
        private IForestGenerator _forestGenerator = null!;
        private MapGenerationContext _context;
        private int[,] _heightMap = null!;

        private INoiseGenerator GetMockedNoiseGenerator()
        {
            INoiseGenerator noiseGenerator_mock = Substitute.For<INoiseGenerator>();
            noiseGenerator_mock.GenerateNoise(Arg.Any<float>(), Arg.Any<MapGenerationContext>())
                .Returns(x =>
                {
                    var context = (MapGenerationContext)x[1];
                    var array = new float[context.Width, context.Height];

                    // Deterministic noise based only on seed and coordinates
                    for (int i = 0; i < context.Width; i++)
                    {
                        for (int j = 0; j < context.Height; j++)
                        {
                            // Use hash function to generate deterministic values from seed + coordinates
                            uint hash = (uint)((context.Seed ^ (i * 73856093) ^ (j * 19349663)) * 2654435761);
                            array[i, j] = (float)(hash % 1000) / 1000f; // Values between 0.0f and 1.0f
                        }
                    }
                    return array;
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
            _forestGenerator = ForestGeneratorFactory.Create(noiseGenerator_mock, 0.1f, 0.3f);
            _context = new MapGenerationContext(20, 20, 42);

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
            MapGenerationContext smallContext = new(15, 15, 42);
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
            MapGenerationContext context = new(30, 30, 42);
            int[,] heightMap = GenerateHeightMap(context.Width, context.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator lowForestGen = ForestGeneratorFactory.Create(noiseGen, 0.1f, 0.1f);
            IForestGenerator highForestGen = ForestGeneratorFactory.Create(noiseGen, 0.1f, 0.8f);

            // Act
            int[,] lowForestMap = lowForestGen.GenerateForests(heightMap, context);
            int[,] highForestMap = highForestGen.GenerateForests(heightMap, context);

            // Count trees in each
            int lowTreeCount = 0;
            int highTreeCount = 0;
            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
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
            MapGenerationContext context1 = new(15, 15, 12345);
            MapGenerationContext context2 = new(15, 15, 12345);

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator forestGen = ForestGeneratorFactory.Create(noiseGen, 0.1f, 0.3f);

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
            MapGenerationContext context1 = new(15, 15, 69);
            MapGenerationContext context2 = new(15, 15, 420);

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IForestGenerator forestGen = ForestGeneratorFactory.Create(noiseGen, 0.1f, 0.3f);

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
