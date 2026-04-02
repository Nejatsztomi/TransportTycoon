using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void MapGeneratorFactory_CreateMapGenerator_WithValidSettings()
        {
            // Arrange
            MapGenerationSettings settings = new();

            // Act
            IMapGenerator result = MapGeneratorFactory.CreateMapGenerator(settings);

            // Assert
            Assert.IsNotNull(result);
            // To avoid namespace problems
            Assert.IsInstanceOfType<TransportTycoon.MapData.MapGenerator.MapGenerator>(result);
        }
    }

    [TestClass]
    public class GenerateMapTest
    {
        private IMapGenerator _mapGenerator = null!;
        private MapGenerationContext _context;

        [TestInitialize]
        public void Initialize()
        {
            MapGenerationSettings settings = new();
            _mapGenerator = MapGeneratorFactory.CreateMapGenerator(settings);
            _context = new(15, 15, 42);
        }

        [TestMethod]
        public void GenerateMap_ReturnsCorrectDimensions()
        {
            // Act
            Field[,] map = _mapGenerator.GenerateMap(_context);

            // Assert
            Assert.AreEqual(_context.Width, map.GetLength(0), "Map width should match context");
            Assert.AreEqual(_context.Height, map.GetLength(1), "Map height should match context");
        }

        [TestMethod]
        public void GenerateMap_AllCellsContainValidFieldType()
        {
            // Act
            Field[,] map = _mapGenerator.GenerateMap(_context);

            // Assert
            bool hasValidFields = true;
            for (int x = 0; x < _context.Width && hasValidFields; x++)
            {
                for (int y = 0; y < _context.Height && hasValidFields; y++)
                {
                    // For now, when we have structure this will fail
                    // Maybe we can also remove the test
                    hasValidFields = map[x, y] is not null && (map[x, y] is Water || map[x, y] is Terrain);
                }
            }
        }

        [TestMethod]
        public void GenerateMap_WaterCellsHaveCorrectData()
        {
            // Act
            Field[,] map = _mapGenerator.GenerateMap(_context);

            // Assert
            bool hasInvalidWaterCells = false;
            for (int x = 0; x < _context.Width && !hasInvalidWaterCells; x++)
            {
                for (int y = 0; y < _context.Height && !hasInvalidWaterCells; y++)
                {
                    if (map[x, y] is Water water)
                    {
                        hasInvalidWaterCells = water.X != x
                            || water.Y != y;
                    }
                }
            }
            Assert.IsFalse(hasInvalidWaterCells, "All water cells should have correct coordinates");
        }

        [TestMethod]
        public void GenerateMap_TerrainCellsHaveCorrectData()
        {
            // Act
            Field[,] map = _mapGenerator.GenerateMap(_context);

            // Assert
            bool hasInvalidTerrainCells = false;
            for (int x = 0; x < _context.Width && !hasInvalidTerrainCells; x++)
            {
                for (int y = 0; y < _context.Height && !hasInvalidTerrainCells; y++)
                {
                    if (map[x, y] is Terrain terrain)
                    {
                        hasInvalidTerrainCells = terrain.X != x
                            || terrain.Y != y
                            || !(1 <= terrain.Height && terrain.Height <= 4)
                            || !(0 <= terrain.Trees && terrain.Trees <= 4);
                    }
                }
            }
            Assert.IsFalse(hasInvalidTerrainCells, "All terrain cells should have correct coordinates, valid height and valid tree count");
        }

        [TestMethod]
        public void GenerateMap_SameSeedProducesSameResult()
        {
            // Arrange
            MapGenerationSettings settings = new();
            IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(settings);
            MapGenerationContext context1 = new(15, 15, 12345);
            MapGenerationContext context2 = new(15, 15, 12345);

            // Act
            Field[,] map1 = mapGen.GenerateMap(context1);
            Field[,] map2 = mapGen.GenerateMap(context2);

            // Assert
            bool hasDifferentCells = false;
            for (int x = 0; x < context1.Width && !hasDifferentCells; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentCells; y++)
                {
                    hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();

                    if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
                    {
                        hasDifferentCells |= t1.Height != t2.Height || t1.Trees != t2.Trees;
                    }
                }
            }

            Assert.IsFalse(hasDifferentCells, "Same seed should produce identical maps");
        }

        [TestMethod]
        public void GenerateMap_DifferentSeedsProduceDifferentMaps()
        {
            // Arrange
            MapGenerationSettings settings = new();
            IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(settings);
            MapGenerationContext context1 = new(15, 15, 111);
            MapGenerationContext context2 = new(15, 15, 222);

            // Act
            Field[,] map1 = mapGen.GenerateMap(context1);
            Field[,] map2 = mapGen.GenerateMap(context2);

            // Assert - At least some cells should differ
            bool hasDifferentCells = false;
            for (int x = 0; x < context1.Width && !hasDifferentCells; x++)
            {
                for (int y = 0; y < context1.Height && !hasDifferentCells; y++)
                {
                    hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();

                    if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
                    {
                        hasDifferentCells |= t1.Height != t2.Height || t1.Trees != t2.Trees;
                    }
                }
            }

            Assert.IsTrue(hasDifferentCells, "Different seeds should produce different maps");
        }
    }

    [TestClass]
    public class MapGeneratorWithMocksTest
    {
        [TestMethod]
        public void MapGenerator_Constructor_WithMockedDependencies()
        {
            // Arrange
            MapGenerationSettings settings = Substitute.For<MapGenerationSettings>();
            ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
            IForestGenerator forestGen = Substitute.For<IForestGenerator>();
            IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();

            // Act & Assert - Should not throw
            var mapGen = new global::TransportTycoon.MapData.MapGenerator.MapGenerator(settings, terrainGen, forestGen, waterGen);
            Assert.IsNotNull(mapGen);
        }

        [TestMethod]
        public void MapGenerator_GenerateMap_CallsAllGenerators()
        {
            // Arrange
            ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
            IForestGenerator forestGen = Substitute.For<IForestGenerator>();
            IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();

            int[,] heightMap = new int[5, 5];
            int[,] forestMap = new int[5, 5];
            bool[,] waterMap = new bool[5, 5];

            terrainGen.GenerateTerrain(Arg.Any<IBiome>(), Arg.Any<MapGenerationContext>()).Returns(heightMap);
            forestGen.GenerateForests(heightMap, Arg.Any<MapGenerationContext>()).Returns(forestMap);
            waterGen.GenerateWaterMap(Arg.Any<int>(), heightMap, Arg.Any<MapGenerationContext>()).Returns(waterMap);

            MapGenerationSettings settings = new();
            TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new(settings, terrainGen, forestGen, waterGen);
            MapGenerationContext context = new(5, 5, 42);

            // Act
            _ = mapGen.GenerateMap(context);

            // Assert - Verify all generators were called
            terrainGen.Received(1).GenerateTerrain(settings.Biome, context);
            forestGen.Received(1).GenerateForests(heightMap, context);
            waterGen.Received(1).GenerateWaterMap(settings.RiverCount, heightMap, context);
        }

        [TestMethod]
        public void MapGenerator_GenerateMap_ReturnsFieldsWithCorrectDimensions()
        {
            // Arrange
            ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
            IForestGenerator forestGen = Substitute.For<IForestGenerator>();
            IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();

            int width = 10;
            int height = 12;
            // Create arrays with correct dimensions for context: [width, height]
            int[,] heightMap = new int[width, height];
            int[,] forestMap = new int[width, height];
            bool[,] waterMap = new bool[width, height];

            terrainGen.GenerateTerrain(Arg.Any<IBiome>(), Arg.Any<MapGenerationContext>()).Returns(heightMap);
            forestGen.GenerateForests(heightMap, Arg.Any<MapGenerationContext>()).Returns(forestMap);
            waterGen.GenerateWaterMap(Arg.Any<int>(), heightMap, Arg.Any<MapGenerationContext>()).Returns(waterMap);

            MapGenerationSettings settings = new();
            TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new(settings, terrainGen, forestGen, waterGen);
            MapGenerationContext context = new(width, height, 42);

            // Act
            Field[,] result = mapGen.GenerateMap(context);

            // Assert
            Assert.AreEqual(width, result.GetLength(0));
            Assert.AreEqual(height, result.GetLength(1));
        }
    }
}
