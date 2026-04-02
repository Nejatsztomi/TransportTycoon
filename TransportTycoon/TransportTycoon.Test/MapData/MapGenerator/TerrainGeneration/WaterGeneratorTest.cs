using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class WaterGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void WaterGeneratorFactory_Create_WithValidParameters()
        {
            // Arrange
            INoiseGenerator noiseGenerator = Substitute.For<INoiseGenerator>();

            // Act
            IWaterGenerator result = WaterGeneratorFactory.Create(noiseGenerator, 0.1f);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<WaterGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateWaterMapTest
    {
        private IWaterGenerator _waterGenerator = null!;
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
            INoiseGenerator noiseGenerator = GetMockedNoiseGenerator();
            _waterGenerator = WaterGeneratorFactory.Create(noiseGenerator, 0.1f);
            _context = new MapGenerationContext(20, 20, 42);

            // Create a basic height map for testing
            _heightMap = GenerateHeightMap(_context.Width, _context.Height);
        }

        [TestMethod]
        public void GenerateWaterMap_ReturnsCorrectDimensions()
        {
            // Act
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(3, _heightMap, _context);

            // Assert
            Assert.AreEqual(_context.Width, waterMap.GetLength(0), "Water map width should match context");
            Assert.AreEqual(_context.Height, waterMap.GetLength(1), "Water map height should match context");
        }

        [TestMethod]
        public void GenerateWaterMap_NoWaterOnHighTerrain()
        {
            // Arrange - Create a height map with all high terrain (height >= 2)
            MapGenerationContext smallContext = new(10, 10, 42);
            int[,] highHeightMap = GenerateHeightMap(smallContext.Width, smallContext.Height, extraHeight: 3); // All heights will be 3 or 4

            // Act
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(3, highHeightMap, smallContext);

            // Assert - Water should not appear on high terrain
            bool hasWaterCells = false;
            for (int x = 0; x < smallContext.Width && !hasWaterCells; x++)
            {
                for (int y = 0; y < smallContext.Height && !hasWaterCells; y++)
                {
                    hasWaterCells = waterMap[x, y];
                }
            }
            Assert.IsFalse(hasWaterCells, "Water should not appear on high terrain");
        }

        [TestMethod]
        public void GenerateWaterMap_WaterOnlyOnLowTerrain()
        {
            // Arrange - Create height map with clear terrain height variation
            MapGenerationContext smallContext = new(10, 10, 42);
            int[,] variableHeightMap = GenerateHeightMap(smallContext.Width, smallContext.Height);

            // Act
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(3, variableHeightMap, smallContext);

            // Assert - Water can only exist on terrain with height < 2
            bool hasWaterOnHighTerrain = false;
            for (int x = 0; x < smallContext.Width && !hasWaterOnHighTerrain; x++)
            {
                for (int y = 0; y < smallContext.Height && !hasWaterOnHighTerrain; y++)
                {
                    hasWaterOnHighTerrain = waterMap[x, y] && variableHeightMap[x, y] >= 2;
                }
            }

            Assert.IsFalse(hasWaterOnHighTerrain, "Water should only exist on low terrain (height < 2)");
        }

        [TestMethod]
        public void GenerateWaterMap_SameSeedProducesSameResult()
        {
            // Arrange
            MapGenerationContext context1 = new(15, 15, 12345);
            MapGenerationContext context2 = new(15, 15, 12345);

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IWaterGenerator waterGen = WaterGeneratorFactory.Create(noiseGen, 0.1f);

            // Act
            bool[,] water1 = waterGen.GenerateWaterMap(3, heightMap, context1);
            bool[,] water2 = waterGen.GenerateWaterMap(3, heightMap, context2);

            // Assert
            bool hasDifferentWater = false;
            for (int x = 0; x < context1.Width && !hasDifferentWater; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentWater; y++)
                {
                    hasDifferentWater = water1[x, y] != water2[x, y];
                }
            }
            Assert.IsFalse(hasDifferentWater, "Same seed should produce the same water map");
        }

        [TestMethod]
        public void GenerateWaterMap_DifferentSeedProducesDifferentResult()
        {
            // Arrange
            MapGenerationContext context1 = new(15, 15, 69);
            MapGenerationContext context2 = new(15, 15, 420);

            int[,] heightMap = GenerateHeightMap(context1.Width, context1.Height);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            IWaterGenerator waterGen = WaterGeneratorFactory.Create(noiseGen, 0.1f);

            // Act
            bool[,] water1 = waterGen.GenerateWaterMap(3, heightMap, context1);
            bool[,] water2 = waterGen.GenerateWaterMap(3, heightMap, context2);

            // Assert
            bool hasDifferentWater = false;
            for (int x = 0; x < context1.Width && !hasDifferentWater; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentWater; y++)
                {
                    hasDifferentWater = water1[x, y] == water2[x, y];
                }
            }
            Assert.IsTrue(hasDifferentWater, "Same seed should produce the same water map");
        }
    }
}
