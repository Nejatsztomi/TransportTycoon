using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class TerrainGeneratorTest
{
    public class FactoryCreateTest
    {
        [Fact]
        public void TerraingGeneratorFactory_Create_WithValidParameters()
        {
            // Arrange
            // Act
            ITerrainGenerator result = TerraingGeneratorFactory.Create();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TerrainGenerator>(result);
        }
    }

    public class GenerateTerrainTest
    {
        private readonly ITerrainGenerator _terrainGenerator;
        private readonly MapGenerationContext _context;

        private INoiseGenerator GetMockedNoiseGenerator()
        {
            INoiseGenerator noiseGenerator_mock = Substitute.For<INoiseGenerator>();
            noiseGenerator_mock.GenerateNoiseMap(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(x =>
                {
                    float[,] noiseMap = new float[(int)x[0], (int)x[1]];
                    for (int i = 0; i < (int)x[0]; i++)
                    {
                        for (int j = 0; j < (int)x[1]; j++)
                        {
                            noiseMap[i, j] = MockedNoise(i, j, (int)x[2]);
                        }
                    }
                    return noiseMap;
                });
            return noiseGenerator_mock;
        }

        private float MockedNoise(int x, int y, int seed)
        {
            uint hash = (uint)((seed ^ (x * 73856093) ^ (y * 19349663)) * 2654435761);
            return (hash % 1000) / 1000f; // Values between 0.0f and 1.0f
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

        public GenerateTerrainTest()
        {
            _context = new(20, 20, 42, new MapGenerationSettings { Biome = GetMockedBiome() });
            _terrainGenerator = TerraingGeneratorFactory.Create();
        }

        [Fact]
        public void GenerateTerrain_ReturnsCorrectDimensions()
        {
            // Act
            float[,] noiseMap = GetMockedNoiseGenerator().GenerateNoiseMap(_context.Width, _context.Height, _context.Seed);
            int[,] terrain = _terrainGenerator.GenerateTerrain(noiseMap, _context);

            // Assert
            Assert.Equal(_context.Width, terrain.GetLength(0));
            Assert.Equal(_context.Height, terrain.GetLength(1));
        }

        [Fact]
        public void GenerateTerrain_AllValuesAreValidTerrainHeights()
        {
            // Arrange
            int minTerrainHeight = 1;
            int maxTerrainHeight = 4;
            float[,] noiseMap = GetMockedNoiseGenerator().GenerateNoiseMap(_context.Width, _context.Height, _context.Seed);

            // Act
            int[,] terrain = _terrainGenerator.GenerateTerrain(noiseMap, _context);

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
            Assert.True(hasValidHeights, "All terrain heights should be valid");
        }

        [Fact]
        public void GenerateTerrain_SameSeedProducesSameResult()
        {
            // Arrange
            MapGenerationSettings settings = new() { Biome = GetMockedBiome() };
            MapGenerationContext context1 = new(15, 15, 12345, settings);
            MapGenerationContext context2 = new(15, 15, 12345, settings);

            ITerrainGenerator terrainGen = TerraingGeneratorFactory.Create();
            float[,] noiseMap1 = GetMockedNoiseGenerator().GenerateNoiseMap(context1.Width, context1.Height, context1.Seed);
            float[,] noiseMap2 = GetMockedNoiseGenerator().GenerateNoiseMap(context2.Width, context2.Height, context2.Seed);

            // Act
            int[,] terrain1 = terrainGen.GenerateTerrain(noiseMap1, context1);
            int[,] terrain2 = terrainGen.GenerateTerrain(noiseMap2, context2);

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
            Assert.False(differentTerrains, "Same seeds should produce same terrains");
        }

        [Fact]
        public void GenerateTerrain_DifferentSeedProducesDifferentResult()
        {
            // Arrange
            MapGenerationSettings settings = new() { Biome = GetMockedBiome() };
            MapGenerationContext context1 = new(15, 15, 111, settings);
            MapGenerationContext context2 = new(15, 15, 222, settings);

            float[,] noiseMap1 = GetMockedNoiseGenerator().GenerateNoiseMap(context1.Width, context1.Height, context1.Seed);
            float[,] noiseMap2 = GetMockedNoiseGenerator().GenerateNoiseMap(context2.Width, context2.Height, context2.Seed);
            ITerrainGenerator terrainGen = TerraingGeneratorFactory.Create();

            // Act
            int[,] terrain1 = terrainGen.GenerateTerrain(noiseMap1, context1);
            int[,] terrain2 = terrainGen.GenerateTerrain(noiseMap2, context2);

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
            Assert.True(hasDifferentTerrains, "Different seeds should produce different terrains");
        }
    }
}
