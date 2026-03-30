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

            Assert.IsEmpty(gameTable.Pointers, "GameTable should initialize Pointers as an empty list");
            Assert.IsEmpty(gameTable.BuildingIDs, "GameTable should initialize BuildingIDs as an empty list");

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

        [TestMethod]
        public void GenerateMap_MapIsGenerated()
        {
            _gameTable.GenerateMap();
            Assert.IsNotEmpty(_gameTable.Table, "GameTable should generate a non-empty map");

            foreach (var field in _gameTable.Table)
            {
                Assert.IsNotNull(field, "Each field in the map should be initialized");
                Assert.IsTrue(field is Water || field is Terrain);
            }
        }

        [TestMethod]
        public void GenerateMap_MapHasValidFields()
        {
            _gameTable.GenerateMap();
            foreach (var field in _gameTable.Table)
            {
                Assert.IsTrue(0 <= field.Height && field.Height <= 4, "Each field should have a height between 0 and 4");
                Assert.IsTrue(_gameTable.IsTileHeightPossible(field.X, field.Y, field.Height), "Each field should have a possible height");
            }
        }

        [TestMethod]
        public void GenerateMap_MapHasTrees()
        {
            _gameTable.GenerateMap();
            int treeCount = 0;

            foreach (var field in _gameTable.Table)
            {
                treeCount += field.GetTrees();
            }

            Assert.IsGreaterThan(0, treeCount, "Generated map should contain trees");
        }

        [TestMethod]
        public void GenerateMap_MapHasValidTrees()
        {
            _gameTable.GenerateMap();

            foreach (var field in _gameTable.Table)
            {
                Assert.IsTrue(0 <= field.GetTrees() && field.GetTrees() <= 4, "Each field should have a trees between 0 and 4");
            }
        }
    }
}
