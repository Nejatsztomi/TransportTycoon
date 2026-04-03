using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class TerrainGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void TerraingGeneratorFactory_Create_WithValidParameters()
        {
            // Arrange
            INoiseGenerator noiseGenerator_mock = Substitute.For<INoiseGenerator>();
            MapGenerationContext context = new(10, 10, 42, new MapGenerationSettings());

            // Act
            ITerrainGenerator result = TerraingGeneratorFactory.Create(noiseGenerator_mock, new RandomProvider(), context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<TerrainGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateTerrainTest
    {
        private ITerrainGenerator _terrainGenerator = null!;
        private IBiome _biome = null!;
        private MapGenerationContext _context = default;

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

        private IBiome GetMockedBiome()
        {
            IBiome biome_mock = Substitute.For<IBiome>();
            biome_mock.PlainRange.Returns(0.3f);
            biome_mock.HillRange.Returns(0.6f);
            biome_mock.MountainRange.Returns(0.8f);
            biome_mock.HighMountainRange.Returns(1.0f);
            return biome_mock;
        }

        [TestInitialize]
        public void Initialize()
        {
            INoiseGenerator noiseGenerator_mock = GetMockedNoiseGenerator();

            _context = new(20, 20, 42, new MapGenerationSettings { Biome = GetMockedBiome() });
            _terrainGenerator = TerraingGeneratorFactory.Create(noiseGenerator_mock, new RandomProvider(), _context);
        }

        [TestMethod]
        public void GenerateTerrain_ReturnsCorrectDimensions()
        {
            // Act
            int[,] terrain = _terrainGenerator.GenerateTerrain(_context);

            // Assert
            Assert.AreEqual(_context.Width, terrain.GetLength(0), "Terrain width should match context");
            Assert.AreEqual(_context.Height, terrain.GetLength(1), "Terrain height should match context");
        }

        [TestMethod]
        public void GenerateTerrain_AllValuesAreValidTerrainHeights()
        {
            // Arrange
            int minTerrainHeight = 1;
            int maxTerrainHeight = 4;

            // Act
            int[,] terrain = _terrainGenerator.GenerateTerrain(_context);

            // Assert
            bool hasValidHeights = true;
            for (int x = 0; x < _context.Width; x++)
            {
                for (int y = 0; y < _context.Height; y++)
                {
                    int height = terrain[x, y];
                    if (!(minTerrainHeight <= height && height <= maxTerrainHeight))
                    {
                        hasValidHeights = false;
                        break;
                    }
                }
            }
            Assert.IsTrue(hasValidHeights, "All terrain heights should be valid");
        }

        [TestMethod]
        public void GenerateTerrain_SameSeedProducesSameResult()
        {
            // Arrange
            MapGenerationSettings settings = new() { Biome = GetMockedBiome() };
            MapGenerationContext context1 = new(15, 15, 12345, settings);
            MapGenerationContext context2 = new(15, 15, 12345, settings);

            INoiseGenerator noiseGen_mock = GetMockedNoiseGenerator();
            ITerrainGenerator terrainGen = TerraingGeneratorFactory.Create(noiseGen_mock, new RandomProvider(), context1);

            // Act
            int[,] terrain1 = terrainGen.GenerateTerrain(context1);
            int[,] terrain2 = terrainGen.GenerateTerrain(context2);

            // Assert
            bool differentTerrains = false;
            for (int x = 0; x < context1.Width; x++)
            {
                for (int y = 0; y < context1.Height; y++)
                {
                    if (terrain1[x, y] != terrain2[x, y])
                    {
                        differentTerrains = true;
                        break;
                    }
                }
            }
            Assert.IsFalse(differentTerrains, "Same seeds should produce same terrains");
        }

        [TestMethod]
        public void GenerateTerrain_DifferentSeedProducesDifferentResult()
        {
            // Arrange
            MapGenerationSettings settings = new() { Biome = GetMockedBiome() };
            MapGenerationContext context1 = new(15, 15, 111, settings);
            MapGenerationContext context2 = new(15, 15, 222, settings);

            INoiseGenerator noiseGen = GetMockedNoiseGenerator();
            ITerrainGenerator terrainGen = TerraingGeneratorFactory.Create(noiseGen, new RandomProvider(), context1);

            // Act
            int[,] terrain1 = terrainGen.GenerateTerrain(context1);
            int[,] terrain2 = terrainGen.GenerateTerrain(context2);

            // Assert - At least some cells should differ
            bool hasDifferentTerrains = false;
            for (int x = 0; x < context1.Width; x++)
            {
                for (int y = 0; y < context1.Height; y++)
                {
                    if (terrain1[x, y] != terrain2[x, y])
                    {
                        hasDifferentTerrains = true;
                        break;
                    }
                }
            }
            Assert.IsTrue(hasDifferentTerrains, "Different seeds should produce different terrains");
        }
    }
}
