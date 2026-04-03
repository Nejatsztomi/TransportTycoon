using TransportTycoon.MapData;

namespace TransportTycoon.Test.MapData;

public class GameTableTest
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void Constructor_WithAllParameters()
        {
            int width = 50;
            int height = 75;

            GameTable gameTable = new(width, height);

            Assert.AreEqual(width, gameTable.Width, "GameTable should have the specified Width");
            Assert.AreEqual(height, gameTable.Height, "GameTable should have the specified Height");

            Assert.IsNotNull(gameTable.Table, "GameTable should initialize the Table array");
            Assert.AreEqual(width, gameTable.Table.GetLength(0), "GameTable's Table should have the correct width");
            Assert.AreEqual(height, gameTable.Table.GetLength(1), "GameTable's Table should have the correct height");
        }

        [TestMethod]
        public void Constructor_Default()
        {
            GameTable gameTable = new();

            Assert.AreEqual(GameTable.DefaultWidth, gameTable.Width, "GameTable should DefaultWidth as Width");
            Assert.AreEqual(GameTable.DefaultHeight, gameTable.Height, "GameTable should DefaultHeight as Height");
        }
    }

    [TestClass]
    public class MapGenerationTest
    {
        private GameTable _gameTable = null!;

        [TestInitialize]
        public void Initialize()
        {
            _gameTable = new(5, 5);
        }

        //[TestMethod]
        //public void GenerateMap_MapIsGenerated()
        //{
        //    _gameTable.GenerateMap();
        //    Assert.IsNotEmpty(_gameTable.Table, "GameTable should generate a non-empty map");

        //    bool hasInvalidField = false;
        //    foreach (var field in _gameTable.Table)
        //    {
        //        if (field is null)
        //        {
        //            Assert.Fail("Each field in the map should be initialized");
        //        }

        //        if (!(field is Water || field is Terrain))
        //        {
        //            hasInvalidField = true;
        //            break;
        //        }
        //    }

        //    Assert.IsFalse(hasInvalidField, "All fields in the map should be either Water or Terrain");
        //}

        //[TestMethod]
        //public void GenerateMap_MapHasValidFields()
        //{
        //    _gameTable.GenerateMap();

        //    bool hasFieldNotInRange = false;
        //    bool hasInvalidField = false;
        //    foreach (var field in _gameTable.Table)
        //    {
        //        if (!(0 <= field.Height && field.Height <= 4))
        //        {
        //            hasFieldNotInRange = true;
        //            break;
        //        }

        //        if (!_gameTable.IsTileHeightPossible(field.X, field.Y, field.Height))
        //        {
        //            hasInvalidField = true;
        //            break;
        //        }
        //    }

        //    Assert.IsFalse(hasFieldNotInRange, "Each field should have height between 0 and 4");
        //    Assert.IsFalse(hasInvalidField, "Each field should have a possible height respecting neighbouring tiles");
        //}

        //[TestMethod]
        //public void GenerateMap_MapHasTrees()
        //{
        //    _gameTable.GenerateMap();
        //    int treeCount = 0;

        //    foreach (var field in _gameTable.Table)
        //    {
        //        treeCount += field.GetTrees();
        //    }

        //    Assert.IsGreaterThan(0, treeCount, "Generated map should contain trees");
        //}

        //[TestMethod]
        //public void GenerateMap_MapHasValidTrees()
        //{
        //    _gameTable.GenerateMap();

        //    bool hasFieldWithInvalidTrees = false;
        //    foreach (var field in _gameTable.Table)
        //    {
        //        if (!(0 <= field.GetTrees() && field.GetTrees() <= 4))
        //        {
        //            hasFieldWithInvalidTrees = true;
        //            break;
        //        }
        //    }

        //    Assert.IsFalse(hasFieldWithInvalidTrees, "Each field should have a valid number of trees between 0 and 4");
        //}
    }
}
