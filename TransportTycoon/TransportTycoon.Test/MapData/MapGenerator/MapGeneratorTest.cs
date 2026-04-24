using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator;

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

        public GenerateMapTest()
        {
            _context = new(15, 15, 42, new());
            _mapGenerator = MapGeneratorFactory.CreateMapGenerator(_context);
        }

        //[Fact]
        //public void GenerateMap_ReturnsCorrectDimensions()
        //{
        //    // Act
        //    Field[,] map = _mapGenerator.GenerateMap(_context);

        //    // Assert
        //    Assert.Equal(_context.Width, map.GetLength(0));
        //    Assert.Equal(_context.Height, map.GetLength(1));
        //}

        //[Fact]
        //public void GenerateMap_AllCellsContainValidFieldType()
        //{
        //    // Act
        //    Field[,] map = _mapGenerator.GenerateMap(_context);

        //    // Assert
        //    bool hasValidFields = true;
        //    for (int x = 0; x < _context.Width && hasValidFields; x++)
        //    {
        //        for (int y = 0; y < _context.Height && hasValidFields; y++)
        //        {
        //            // For now, when we have structure this will fail
        //            // Maybe we can also remove the test
        //            hasValidFields = map[x, y] is not null && (map[x, y] is Water || map[x, y] is Terrain);
        //        }
        //    }
        //}

        //[Fact]
        //public void GenerateMap_WaterCellsHaveCorrectData()
        //{
        //    // Act
        //    Field[,] map = _mapGenerator.GenerateMap(_context);

        //    // Assert
        //    bool hasInvalidWaterCells = false;
        //    for (int x = 0; x < _context.Width && !hasInvalidWaterCells; x++)
        //    {
        //        for (int y = 0; y < _context.Height && !hasInvalidWaterCells; y++)
        //        {
        //            if (map[x, y] is Water water)
        //            {
        //                hasInvalidWaterCells = water.X != x
        //                    || water.Y != y;
        //            }
        //        }
        //    }
        //    Assert.False(hasInvalidWaterCells, "All water cells should have correct coordinates");
        //}

        //[Fact]
        //public void GenerateMap_TerrainCellsHaveCorrectData()
        //{
        //    // Act
        //    Field[,] map = _mapGenerator.GenerateMap(_context);

        //    // Assert
        //    bool hasInvalidTerrainCells = false;
        //    for (int x = 0; x < _context.Width && !hasInvalidTerrainCells; x++)
        //    {
        //        for (int y = 0; y < _context.Height && !hasInvalidTerrainCells; y++)
        //        {
        //            if (map[x, y] is Terrain terrain)
        //            {
        //                hasInvalidTerrainCells = terrain.X != x
        //                    || terrain.Y != y
        //                    || !(1 <= terrain.Height && terrain.Height <= 4)
        //                    || !(0 <= terrain.Trees && terrain.Trees <= 4);
        //            }
        //        }
        //    }
        //    Assert.False(hasInvalidTerrainCells, "All terrain cells should have correct coordinates, valid height and valid tree count");
        //}

        //[Fact]
        //public void GenerateMap_SameSeedProducesSameResult()
        //{
        //    // Arrange
        //    MapGenerationContext context1 = new(15, 15, 12345, new MapGenerationSettings());
        //    MapGenerationContext context2 = new(15, 15, 12345, new MapGenerationSettings());
        //    IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(context1);

        //    // Act
        //    Field[,] map1 = mapGen.GenerateMap(context1);
        //    Field[,] map2 = mapGen.GenerateMap(context2);

        //    // Assert
        //    bool hasDifferentCells = false;
        //    for (int x = 0; x < context1.Width && !hasDifferentCells; x++)
        //    {
        //        for (int y = 0; y < context1.Height && !hasDifferentCells; y++)
        //        {
        //            hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();

        //            if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
        //            {
        //                hasDifferentCells |= t1.Height != t2.Height || t1.Trees != t2.Trees;
        //            }
        //        }
        //    }

        //    Assert.False(hasDifferentCells, "Same seed should produce identical maps");
        //}

        //[Fact]
        //public void GenerateMap_DifferentSeedsProduceDifferentMaps()
        //{
        //    // Arrange
        //    MapGenerationContext context1 = new(15, 15, 111, new MapGenerationSettings());
        //    MapGenerationContext context2 = new(15, 15, 222, new MapGenerationSettings());
        //    IMapGenerator mapGen = MapGeneratorFactory.CreateMapGenerator(context1);

        //    // Act
        //    Field[,] map1 = mapGen.GenerateMap(context1);
        //    Field[,] map2 = mapGen.GenerateMap(context2);

        //    // Assert - At least some cells should differ
        //    bool hasDifferentCells = false;
        //    for (int x = 0; x < context1.Width && !hasDifferentCells; x++)
        //    {
        //        for (int y = 0; y < context1.Height && !hasDifferentCells; y++)
        //        {
        //            hasDifferentCells = map1[x, y].GetType() != map2[x, y].GetType();

        //            if (map1[x, y] is Terrain t1 && map2[x, y] is Terrain t2)
        //            {
        //                hasDifferentCells |= t1.Height != t2.Height || t1.Trees != t2.Trees;
        //            }
        //        }
        //    }

        //    Assert.True(hasDifferentCells, "Different seeds should produce different maps");
        //}
    }

    public class MapGeneratorWithMocksTest
    {
        [Fact]
        public void MapGenerator_Constructor_WithMockedDependencies()
        {
            // Arrange
            ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
            IForestGenerator forestGen = Substitute.For<IForestGenerator>();
            IWaterGenerator lakeGen = Substitute.For<IWaterGenerator>();
            IWaterGenerator riverGen = Substitute.For<IWaterGenerator>();
            IStructureGenerator structureGen = Substitute.For<IStructureGenerator>();
            IRandom random = Substitute.For<IRandom>();
            List<IMapPluginGenerator> generators = [terrainGen, forestGen, lakeGen, riverGen, structureGen];
            // Act & Assert - Should not throw
            var mapGen = new TransportTycoon.MapData.MapGenerator.MapGenerator(generators, random);
            Assert.NotNull(mapGen);
        }

        //[Fact]
        //public void MapGenerator_GenerateMap_CallsAllGenerators()
        //{
        //    // Arrange
        //    ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
        //    IForestGenerator forestGen = Substitute.For<IForestGenerator>();
        //    IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();
        //    IStructureGenerator structureGen = Substitute.For<IStructureGenerator>();

        //    int[,] heightMap = new int[5, 5];
        //    int[,] forestMap = new int[5, 5];
        //    bool[,] waterMap = new bool[5, 5];

        //    terrainGen.GenerateTerrain(Arg.Any<MapGenerationContext>()).Returns(heightMap);
        //    forestGen.GenerateForests(heightMap, Arg.Any<MapGenerationContext>()).Returns(forestMap);
        //    waterGen.GenerateWaterMap(heightMap, Arg.Any<MapGenerationContext>()).Returns(waterMap);

        //    MapGenerationSettings settings = new();
        //    TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new(terrainGen, forestGen, waterGen, structureGen);
        //    MapGenerationContext context = new(5, 5, 42, settings);

        //    // Act
        //    _ = mapGen.GenerateMap(context);

        //    // Assert - Verify all generators were called
        //    terrainGen.Received(1).GenerateTerrain(context);
        //    forestGen.Received(1).GenerateForests(heightMap, context);
        //    waterGen.Received(1).GenerateWaterMap(heightMap, context);
        //}

        //[Fact]
        //public void MapGenerator_GenerateMap_ReturnsFieldsWithCorrectDimensions()
        //{
        //    // Arrange
        //    ITerrainGenerator terrainGen = Substitute.For<ITerrainGenerator>();
        //    IForestGenerator forestGen = Substitute.For<IForestGenerator>();
        //    IWaterGenerator waterGen = Substitute.For<IWaterGenerator>();
        //    IStructureGenerator structureGen = Substitute.For<IStructureGenerator>();

        //    int width = 10;
        //    int height = 12;
        //    // Create arrays with correct dimensions for context: [width, height]
        //    int[,] heightMap = new int[width, height];
        //    int[,] forestMap = new int[width, height];
        //    bool[,] waterMap = new bool[width, height];

        //    terrainGen.GenerateTerrain(Arg.Any<MapGenerationContext>()).Returns(heightMap);
        //    forestGen.GenerateForests(heightMap, Arg.Any<MapGenerationContext>()).Returns(forestMap);
        //    waterGen.GenerateWaterMap(heightMap, Arg.Any<MapGenerationContext>()).Returns(waterMap);

        //    MapGenerationSettings settings = new();
        //    TransportTycoon.MapData.MapGenerator.MapGenerator mapGen = new(terrainGen, forestGen, waterGen, structureGen);
        //    MapGenerationContext context = new(width, height, 42, settings);

        //    // Act
        //    Field[,] result = mapGen.GenerateMap(context);

        //    // Assert
        //    Assert.Equal(width, result.GetLength(0));
        //    Assert.Equal(height, result.GetLength(1));
        //}
    }
}
