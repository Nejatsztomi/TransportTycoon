using TransportTycoon.MapData;

namespace TransportTycoon.Test.MapData;

public class GameTableTest
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void Constructor_WithAllParameters() { }

        [TestMethod]
        public void Constructor_Default() { }
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
