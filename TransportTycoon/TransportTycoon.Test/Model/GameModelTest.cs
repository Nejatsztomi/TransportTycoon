using TransportTycoon.Model;

namespace TransportTycoon.Test.Model;

public class GameModelTest
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        public void Constructor_WithAllParameters() { }

        [TestMethod]
        public void Constructor_WithBalance() { }

        [TestMethod]
        public void Constructor_WithDifficulty() { }
    }

    public class EventTest
    {
        [TestClass]
        public class EventRaisedTest
        {
            private static GameModel _gameModel = null!;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                _gameModel = new GameModel(Difficulty.Medium, 1000, null!);
            }

            [TestMethod]
            public void NewGameCreated_EventIsRaised() { }

            [TestMethod]
            public void GameModeChanged_EventIsRaised() { }

            [TestMethod]
            public void TimeSpeedChanged_EventIsRaised() { }

            [TestMethod]
            public void GameOver_EventIsRaised() { }

            [TestMethod]
            public void GameTicked_EventIsRaised() { }

            [TestMethod]
            public void GameAdvanced_EventIsRaised() { }

            [TestMethod]
            public void InfrastructureBuilt_EventIsRaised() { }
        }

        [TestClass]
        public class EventArgumentTest
        {
            private static GameModel _gameModel = null!;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                _gameModel = new GameModel(Difficulty.Medium, 1000, null!);
            }

            [TestMethod]
            public void GameModeChanged_EventArgumentIsCorrect() { }

            [TestMethod]
            public void TimeSpeedChanged_EventArgumentIsCorrect() { }

            [TestMethod]
            public void GameOver_EventArgumentIsCorrect() { }

            [TestMethod]
            public void GameAdvanced_EventArgumentIsCorrect() { }

            [TestMethod]
            public void InfrastructureBuilt_EventArgumentIsCorrect() { }
        }
    }
}
