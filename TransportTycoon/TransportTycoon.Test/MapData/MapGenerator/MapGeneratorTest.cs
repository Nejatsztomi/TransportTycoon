using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator
{
    public class MapGeneratorTest
    {
        public class FactoryCreateTest
        {
            [Fact]
            public void MapGeneratorFactory_CreateMapGenerator_WithValidContext()
            {
                // Arrange
                MapGenerationContext context = new(15, 15, 42, new MapGenerationSettings());

                // Act
                IMapGenerator result = MapGeneratorFactory.CreateMapGenerator(context);

                // Assert
                Assert.NotNull(result);
                // To avoid namespace problems
                Assert.IsType<TransportTycoon.MapData.MapGenerator.MapGenerator>(result);
            }
        }

        public class GenerateMapTest
        {
            private readonly IMapGenerator _mapGenerator;
            private readonly MapGenerationContext _context;
            private readonly MapGenerationSettings _settings = new() { MinCities = 2, MaxCities = 2, MinStructure = 1, MaxStructure = 1 };

            public GenerateMapTest()
            {
                _context = new(50, 50, 42, _settings);
                _mapGenerator = MapGeneratorFactory.CreateMapGenerator(_context);
            }


            [Fact]
            public void GenerateMap_ReturnsCorrectDimensions()
            {
                // Arrange
                // (Setup done in constructor)

                // Act
                var (map, _) = _mapGenerator.GenerateMap(_context);

                // Assert
                Assert.Equal(_context.Width, map.GetLength(0));
                Assert.Equal(_context.Height, map.GetLength(1));
            }

            [Fact]
            public void GenerateMap_WaterCellsHaveCorrectData()
            {
                // Arrange
                // (Setup done in constructor)

                // Act
                var (map, _) = _mapGenerator.GenerateMap(_context);

                // Assert
                bool hasInvalidWaterCells = false;
                for (int x = 0; x < _context.Width && !hasInvalidWaterCells; x++)
                {
                    for (int y = 0; y < _context.Height && !hasInvalidWaterCells; y++)
                    {
                        if (map[x, y].GetType().Name.Contains("Water"))
                        {
                            dynamic water = map[x, y];
                            hasInvalidWaterCells = water.X != x || water.Y != y;
                        }
                    }
                }
                Assert.False(hasInvalidWaterCells, "All water cells should have correct coordinates");
            }

            [Fact]
            public void GenerateMap_TerrainCellsHaveCorrectData()
            {
                // Arrange
                // (Setup done in constructor)

                // Act
                var (map, _) = _mapGenerator.GenerateMap(_context);

                // Assert
                bool hasInvalidTerrainCells = false;
                for (int x = 0; x < _context.Width && !hasInvalidTerrainCells; x++)
                {
                    for (int y = 0; y < _context.Height && !hasInvalidTerrainCells; y++)
                    {
                        if (map[x, y].GetType().Name.Contains("Terrain"))
                        {
                            dynamic terrain = map[x, y];
                            hasInvalidTerrainCells = terrain.X != x
                                || terrain.Y != y
                                || !(1 <= terrain.Height && terrain.Height <= 4)
                                || !(0 <= terrain.Trees && terrain.Trees <= 4);
                        }
                    }
                }
                Assert.False(hasInvalidTerrainCells, "All terrain cells should have correct coordinates, valid height and valid tree count");
            }

            [Fact]
            public void GenerateMap_SameSeedProducesSameResult()
            {
                // Arrange
                MapGenerationContext context = new(50, 50, 12345, _settings);
                IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(context);

                MapGenerationContext context1 = new(context.Width, context.Height, context.Seed, context.Settings);
                MapGenerationContext context2 = new(context.Width, context.Height, context.Seed, context.Settings);

                // Act
                var (map1, _) = mapGen.GenerateMap(context1);
                var (map2, _) = mapGen.GenerateMap(context2);
                // Assert
                bool hasDifferentCells = false;
                for (int x = 0; x < context.Width && !hasDifferentCells; x++)
                {
                    for (int y = 0; y < context.Height && !hasDifferentCells; y++)
                    {
                        hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();
                        hasDifferentCells |= map1[x, y].Height != map2[x, y].Height;

                        if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
                        {
                            hasDifferentCells |= t1.Trees != t2.Trees;
                        }
                        Assert.False(hasDifferentCells, $"Maps differ at ({x}, {y}) (t1: {map1[x, y]}) (t2: {map2[x, y]})");
                    }
                }

                Assert.False(hasDifferentCells, "Same seed should produce identical maps");
            }

            [Fact]
            public void GenerateMap_DifferentSeedsProduceDifferentMaps()
            {
                // Arrange
                MapGenerationContext context1 = new(50, 50, 111, _settings);
                MapGenerationContext context2 = new(50, 50, 222, _settings);
                IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(context1);

                // Act
                var (map1, _) = mapGen.GenerateMap(context1);
                var (map2, _) = mapGen.GenerateMap(context2);

                // Assert - At least some cells should differ
                bool hasDifferentCells = false;
                for (int x = 0; x < context1.Width && !hasDifferentCells; x++)
                {
                    for (int y = 0; y < context1.Height && !hasDifferentCells; y++)
                    {
                        hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();
                        hasDifferentCells |= map1[x, y].Height != map2[x, y].Height;

                        if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
                        {
                            hasDifferentCells |= t1.Trees != t2.Trees;
                        }
                    }
                }

                Assert.True(hasDifferentCells, "Different seeds should produce different maps");
            }
        }

        public class MapGeneratorWithMocksTest
        {
            [Fact]
            public void MapGenerator_Constructor_WithMockedDependencies()
            {
                // Arrange
                var terrainGen = Substitute.For<ITerrainGenerator>();
                var forestGen = Substitute.For<IForestGenerator>();
                var lakeGen = Substitute.For<IWaterGenerator>();
                var riverGen = Substitute.For<IWaterGenerator>();
                var structureGen = Substitute.For<IStructureGenerator>();
                var random = Substitute.For<IRandomProvider>();
                List<IMapPluginGenerator> generators = [terrainGen, forestGen, lakeGen, riverGen, structureGen];

                // Act & Assert - Should not throw
                var mapGen = new TransportTycoon.MapData.MapGenerator.MapGenerator(generators, random);
                Assert.NotNull(mapGen);
            }

            [Fact]
            public void MapGenerator_GenerateMap_CallsAllGenerators()
            {
                // Arrange
                var terrainGen = Substitute.For<ITerrainGenerator>();
                var forestGen = Substitute.For<IForestGenerator>();
                var waterGen = Substitute.For<IWaterGenerator>();
                var structureGen = Substitute.For<IStructureGenerator>();

                int[,] heightMap = new int[5, 5];
                int[,] forestMap = new int[5, 5];
                bool[,] waterMap = new bool[5, 5];

                MapGenerationContext context = new(5, 5, 42, new());
                terrainGen.GenerateTerrain(Arg.Any<float[,]>(), context).Returns(heightMap);
                forestGen.GenerateForests(Arg.Any<int[,]>(), context).Returns(forestMap);
                waterGen.GenerateWaterMap(Arg.Any<float[,]>(), Arg.Any<bool[,]>(), context).Returns(waterMap);
                structureGen.GenerateStructures(context).Returns([]);

                TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new([terrainGen, forestGen, waterGen, structureGen], Substitute.For<IRandomProvider>());

                // Act
                mapGen.GenerateMap(context);

                // Assert - Verify all generators were called
                terrainGen.Received(1).GenerateTerrain(Arg.Any<float[,]>(), context);
                forestGen.Received(1).GenerateForests(Arg.Any<int[,]>(), context);
                waterGen.Received(1).GenerateWaterMap(Arg.Any<float[,]>(), Arg.Any<bool[,]>(), context);
            }

            [Fact]
            public void MapGenerator_GenerateMap_ReturnsFieldsWithCorrectDimensions()
            {
                // Arrange
                ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
                IForestGenerator forestGen = Substitute.For<IForestGenerator>();
                IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();
                IStructureGenerator structureGen = Substitute.For<IStructureGenerator>();

                int width = 10;
                int height = 12;
                MapGenerationContext context = new(width, height, 42, new());
                // Create arrays with correct dimensions for context: [width, height]
                int[,] heightMap = new int[width, height];
                int[,] forestMap = new int[width, height];
                bool[,] waterMap = new bool[width, height];

                terrainGen.GenerateTerrain(Arg.Any<float[,]>(), context).Returns(heightMap);
                forestGen.GenerateForests(Arg.Any<int[,]>(), context).Returns(forestMap);
                waterGen.GenerateWaterMap(Arg.Any<float[,]>(), Arg.Any<bool[,]>(), context).Returns(waterMap);
                structureGen.GenerateStructures(context).Returns([]);

                TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new([terrainGen, forestGen, waterGen, structureGen], Substitute.For<IRandomProvider>());

                // Act
                (IField[,] result, _) = mapGen.GenerateMap(context);

                // Assert
                Assert.Equal(width, result.GetLength(0));
                Assert.Equal(height, result.GetLength(1));
            }
        }

        public class MapGenerationContextEdgeCaseTest
        {
            [Fact]
            public void MapGenerationContext_Throws_OnNegativeWidth()
            {
                Assert.Throws<ArgumentException>(() => new MapGenerationContext(-1, 10, 1, new MapGenerationSettings()));
            }

            [Fact]
            public void MapGenerationContext_Throws_OnNegativeHeight()
            {
                Assert.Throws<ArgumentException>(() => new MapGenerationContext(10, -1, 1, new MapGenerationSettings()));
            }
        }

        public class MapGenerationSettingsEdgeCaseTest
        {
            [Fact]
            public void MapGenerationSettings_Throws_OnInvalidMinStructure()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new MapGenerationSettings { MinStructure = 0 });
            }

            [Fact]
            public void MapGenerationSettings_Throws_OnInvalidMaxStructure()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new MapGenerationSettings { MinStructure = 2, MaxStructure = 1 });
            }

            [Fact]
            public void MapGenerationSettings_Throws_OnInvalidMinCities()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new MapGenerationSettings { MinCities = 1 });
            }

            [Fact]
            public void MapGenerationSettings_Throws_OnInvalidMaxCities()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new MapGenerationSettings { MinCities = 3, MaxCities = 2 });
            }
        }

        public class MapGeneratorPluginOrderTest
        {
            [Fact]
            public void MapGenerator_InvokesGeneratorsInOrderOfPhase()
            {
                // Arrange
                var calls = new List<string>();
                var gen1 = Substitute.For<INoiseGenerator>();
                var gen2 = Substitute.For<INoiseGenerator>();
                var gen3 = Substitute.For<INoiseGenerator>();
                gen1.Phase.Returns(GenerationPhase.Noise);
                gen2.Phase.Returns(GenerationPhase.BaseTerrain);
                gen3.Phase.Returns(GenerationPhase.WaterLayer);
                gen1.When(x => x.GenerateNoiseMap(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())).Do(_ => calls.Add("gen1"));
                gen2.When(x => x.GenerateNoiseMap(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())).Do(_ => calls.Add("gen2"));
                gen3.When(x => x.GenerateNoiseMap(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())).Do(_ => calls.Add("gen3"));
                var random = Substitute.For<IRandomProvider>();
                var mapGen = new TransportTycoon.MapData.MapGenerator.MapGenerator([gen2, gen3, gen1], random);
                var context = new MapGenerationContext(5, 5, 1, new MapGenerationSettings());

                // Act
                mapGen.GenerateMap(context);

                // Assert
                Assert.Equal(new[] { "gen1", "gen2", "gen3" }, calls);
            }
        }

        public class MapGeneratorNullAndEmptyTest
        {
            [Fact]
            public void MapGenerator_HandlesEmptyGeneratorList()
            {
                // Arrange
                int width = 3;
                int height = 3;

                var random = Substitute.For<IRandomProvider>();
                var mapGen = new TransportTycoon.MapData.MapGenerator.MapGenerator([], random);
                var context = new MapGenerationContext(width, height, 1, new MapGenerationSettings());

                // Act
                var result = mapGen.GenerateMap(context);

                // Assert
                Assert.True(result.Item1.Cast<IField>().All(cell => cell.Height == 0));
                Assert.Empty(result.Item2); // No structures should be generated
            }
        }
    }
}
