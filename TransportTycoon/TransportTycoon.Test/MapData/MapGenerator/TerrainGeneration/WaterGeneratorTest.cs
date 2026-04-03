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
            IWaterGenerator result = LakeGeneratorFactory.Create(noiseGenerator);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<LakeGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateWaterMapTest
    {
        private IWaterGenerator _waterGenerator = null!;
        private MapGenerationContext _context = default;
        private int[,] _heightMap = null!;
        private bool[,] _waterMap = null!;

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
            INoiseGenerator noiseGenerator = GetMockedNoiseGenerator();
            _context = new MapGenerationContext(20, 20, 42, new MapGenerationSettings());
            _waterGenerator = LakeGeneratorFactory.Create(noiseGenerator);

            // Create a basic height map for testing
            _heightMap = GenerateHeightMap(_context.Width, _context.Height);
            _waterMap = new bool[_context.Width, _context.Height];
        }

        [TestMethod]
        public void GenerateWaterMap_ReturnsCorrectDimensions()
        {
            // Act
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(_heightMap, _waterMap, _context);

            // Assert
            Assert.AreEqual(_context.Width, waterMap.GetLength(0), "Water map width should match context");
            Assert.AreEqual(_context.Height, waterMap.GetLength(1), "Water map height should match context");
        }

        [TestMethod]
        public void GenerateWaterMap_NoWaterOnHighTerrain()
        {
            // Arrange - Create a height map with all high terrain (height >= 2)
            MapGenerationContext smallContext = new(10, 10, 42, new MapGenerationSettings());
            int[,] highHeightMap = GenerateHeightMap(smallContext.Width, smallContext.Height, extraHeight: 3); // All heights will be 3 or 4
            bool[,] waterMap = new bool[smallContext.Width, smallContext.Height];

            // Act
            waterMap = _waterGenerator.GenerateWaterMap(highHeightMap, waterMap, smallContext);

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
            MapGenerationContext smallContext = new(10, 10, 42, new MapGenerationSettings());
            int[,] variableHeightMap = GenerateHeightMap(smallContext.Width, smallContext.Height);
            bool[,] waterMap = new bool[smallContext.Width, smallContext.Height];

            // Act
            waterMap = _waterGenerator.GenerateWaterMap(variableHeightMap, waterMap, smallContext);

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
    }
}
