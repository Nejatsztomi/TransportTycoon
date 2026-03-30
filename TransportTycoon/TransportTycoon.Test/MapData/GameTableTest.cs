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
            Assert.AreEqual(height, gameTable.Table.GetLength(1), "GameTable's Table should have the correct height")
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
        private static GameTable _gameTable = null!;

        [ClassInitialize]
        public static void Initialize(TestContext _)
        {
            _gameTable = new GameTable(10, 10);
        }

        [TestMethod]
        public void GenerateMap_MapIsGenerated() { }

        [TestMethod]
        public void GenerateMap_MapHasValidFields() { }

        [TestMethod]
        public void GenerateMap_MapHasTrees() { }

        [TestMethod]
        public void GenerateMap_MapHasValidTrees() { }
    }
}
