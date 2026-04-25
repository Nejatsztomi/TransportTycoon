using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData;

public class GameTableTest
{
    public class ConstructorTest
    {
        [Fact]
        public void Constructor_WithAllParameters()
        {
            int width = 50;
            int height = 75;
            MapGenerationSettings mockSettings = Substitute.For<MapGenerationSettings>();
            MapGenerationContext context = new(width, height, 0, mockSettings);

            var mockGenerator = Substitute.For<IMapGenerator>();

            GameTable gameTable = new(mockGenerator, context);

            Assert.NotNull(gameTable.Table);
            Assert.Equal(width, gameTable.Table.GetLength(0));
            Assert.Equal(height, gameTable.Table.GetLength(1));
        }
    }

    //public class MapGenerationTest
    //{
    //    private GameTable _gameTable = null!;

    //    public MapGenerationTest
    //    {
    //        _gameTable = new(5, 5);
    //    }

    //    //[Fact]
    //    //public void GenerateMap_MapIsGenerated()
    //    //{
    //    //    _gameTable.GenerateMap();
    //    //    Assert.NotEmpty(_gameTable.Table);
    //    //    bool hasInvalidField = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (field is null)
    //    //        {
    //    //            Assert.Fail("Each field in the map should be initialized");
    //    //        }

    //    //        if (!(field is Water || field is Terrain))
    //    //        {
    //    //            hasInvalidField = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasInvalidField, "All fields in the map should be either Water or Terrain");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasValidFields()
    //    //{
    //    //    _gameTable.GenerateMap();

    //    //    bool hasFieldNotInRange = false;
    //    //    bool hasInvalidField = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (!(0 <= field.Height && field.Height <= 4))
    //    //        {
    //    //            hasFieldNotInRange = true;
    //    //            break;
    //    //        }

    //    //        if (!_gameTable.IsTileHeightPossible(field.X, field.Y, field.Height))
    //    //        {
    //    //            hasInvalidField = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasFieldNotInRange, "Each field should have height between 0 and 4");
    //    //    Assert.False(hasInvalidField, "Each field should have a possible height respecting neighbouring tiles");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasTrees()
    //    //{
    //    //    _gameTable.GenerateMap();
    //    //    int treeCount = 0;

    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        treeCount += field.GetTrees();
    //    //    }

    //    //    Assert.True(treeCount > 0, "Generated map should contain trees");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasValidTrees()
    //    //{
    //    //    _gameTable.GenerateMap();

    //    //    bool hasFieldWithInvalidTrees = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (!(0 <= field.GetTrees() && field.GetTrees() <= 4))
    //    //        {
    //    //            hasFieldWithInvalidTrees = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasFieldWithInvalidTrees, "Each field should have a valid number of trees between 0 and 4");
    //    //}
    //}
}
