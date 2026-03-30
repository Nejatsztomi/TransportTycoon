using NSubstitute;
using TransportTycoon.Model;
using ITimer = TransportTycoon.Model.ITimer;

namespace TransportTycoon.Test.Model;

public class GameModelTest
{
    [TestClass]
    public class ConstructorTest
    {
        private ITimer _mockTimer = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockTimer = Substitute.For<ITimer>();
        }

        [TestMethod]
        public void Constructor_WithAllParameters()
        {
            GameModel gameModel = new(Difficulty.Easy, 1_000, _mockTimer);

            Assert.AreEqual(Difficulty.Easy, gameModel.Difficulty, "Difficulty should match");
            Assert.AreEqual(1_000, gameModel.Balance, "Balance should match");

            Assert.AreEqual(GameMode.Run, gameModel.Mode, "GameMode should be Run");
            Assert.AreEqual(0, gameModel.GameTime, "GameTime should be 0");

            // Lehet érdemes a Map-ot is átadni, mint paraméter
            Assert.IsNotNull(gameModel.Map, "Map should be generated and not null");

            // Timer mock tesztek
            // Feliratkoztak az Elapsed eseményre
            _mockTimer.Received().Elapsed += Arg.Any<EventHandler>();
        }

        [TestMethod]
        public void Constructor_WithBalance()
        {
            GameModel gameModel = new(Difficulty.Easy, _mockTimer);

            Assert.AreEqual(Difficulty.Easy, gameModel.Difficulty, "Difficulty should match");
            Assert.AreEqual(GameModel.InitialBalance, gameModel.Balance, "Balance should be InitialBalance");
        }

        [TestMethod]
        public void Constructor_WithDifficulty()
        {
            GameModel gameModel = new(1_000, _mockTimer);

            Assert.AreEqual(GameModel.InitialDifficulty, gameModel.Difficulty, "Difficulty should be InitialDifficulty");
            Assert.AreEqual(1_000, gameModel.Balance, "Balance should match");
        }
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
                var mockTimer = Substitute.For<ITimer>();
                _gameModel = new GameModel(Difficulty.Medium, 1000, mockTimer);
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
                var mockTimer = Substitute.For<ITimer>();
                _gameModel = new GameModel(Difficulty.Medium, 1000, mockTimer);
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
