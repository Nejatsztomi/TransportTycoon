using NSubstitute;
using NSubstitute.ClearExtensions;
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
            private static IEnumerable<object[]> GetAllGameModes()
            {
                foreach (var role in Enum.GetValues<GameMode>())
                {
                    yield return new object[] { role };
                }
            }

            private static IEnumerable<object[]> GetAllTimeSpeeds()
            {
                foreach (var role in Enum.GetValues<TimeSpeed>())
                {
                    yield return new object[] { role };
                }
            }

            private GameModel _gameModel = null!;
            private ITimer _mockTimer = null!;

            [TestInitialize]
            public void Initialize()
            {
                _mockTimer = Substitute.For<ITimer>();
                _gameModel = new(Difficulty.Medium, 1000, _mockTimer);
            }

            [TestMethod]
            public void NewGameCreated_EventIsRaised()
            {
                bool raised = false;

                EventHandler handler = (sender, args) =>
                {
                    raised = true;
                };

                try
                {
                    _gameModel.NewGameCreated += handler;
                    _gameModel.NewGame();
                    Assert.IsTrue(raised, "NewGameCreated should be raised after creating a new game");
                }
                finally
                {
                    _gameModel.NewGameCreated -= handler;
                }
            }

            [TestMethod]
            [DynamicData(nameof(GetAllGameModes))]
            public void GameModeChanged_EventIsRaised(GameMode gameMode)
            {
                bool raised = false;

                EventHandler<GameMode> handler = (_, _) =>
                {
                    raised = true;
                };

                try
                {
                    _gameModel.GameModeChanged += handler;
                    _gameModel.SetMode(gameMode);
                    Assert.IsTrue(raised, "GameModeChanged should be raised after changing the game mode");
                }
                finally
                {
                    _gameModel.GameModeChanged -= handler;
                }
            }

            [TestMethod]
            [DynamicData(nameof(GetAllTimeSpeeds))]
            public void TimeSpeedChanged_EventIsRaised(TimeSpeed timeSpeed)
            {
                bool raised = false;

                EventHandler<TimeSpeed> handler = (_, _) =>
                {
                    raised = true;
                };

                try
                {
                    _gameModel.TimeSpeedChanged += handler;
                    _gameModel.SetTimeSpeed(timeSpeed);
                    Assert.IsTrue(raised, "TimeSpeedChanged should be raised after changing the game speed");
                }
                finally
                {
                    _gameModel.TimeSpeedChanged -= handler;
                }
            }

            [TestMethod]
            public void GameOver_EventIsRaised()
            {
                bool raised = false;

                EventHandler<List<Tuple<int, int>>> handler = (_, _) =>
                {
                    raised = true;
                };

                try
                {
                    _gameModel.GameAdvanced += handler;
                    // Szimuláljuk a timer tick eseményét 10x (egyelőre ennyi kell egy event kiváltáshoz)
                    for (int i = 0; i < 10; i++)
                    {
                        _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    }
                    Assert.IsTrue(raised, "GameAdvanced event should be raised after 10 timer ticks");
                }
                finally
                {
                    _gameModel.GameAdvanced -= handler;
                }
            }

            [TestMethod]
            public void GameTicked_EventIsRaised()
            {
                bool raised = false;

                EventHandler handler = (_, _) =>
                {
                    raised = true;
                };

                try
                {
                    _gameModel.GameTicked += handler;
                    // Szimuláljuk a timer tick eseményét
                    _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    Assert.IsTrue(raised, "GameTicked should be raised after 1 timer tick");
                }
                finally
                {
                    _gameModel.GameTicked -= handler;
                }
            }

            [TestMethod]
            public void GameAdvanced_EventIsRaised() { }

            [TestMethod]
            public void InfrastructureBuilt_EventIsRaised() { }
        }

        [TestClass]
        public class EventArgumentTest
        {
            private static IEnumerable<object[]> GetAllGameModes()
            {
                foreach (var role in Enum.GetValues<GameMode>())
                {
                    yield return new object[] { role };
                }
            }

            private static IEnumerable<object[]> GetAllTimeSpeeds()
            {
                foreach (var role in Enum.GetValues<TimeSpeed>())
                {
                    yield return new object[] { role };
                }
            }

            private static GameModel _gameModel = null!;
            private static ITimer _mockTimer = null!;

            [TestInitialize]
            public void Initialize()
            {
                _mockTimer = Substitute.For<ITimer>();
                _gameModel = new GameModel(Difficulty.Medium, 1000, _mockTimer);
            }

            [TestMethod]
            [DynamicData(nameof(GetAllGameModes))]
            public void GameModeChanged_EventArgumentIsCorrect(GameMode expectedGameMode)
            {
                GameMode actualGameMode = GameMode.Run;

                EventHandler<GameMode> handler = (_, e) =>
                {
                    actualGameMode = e;
                };

                try
                {
                    _gameModel.GameModeChanged += handler;
                    _gameModel.SetMode(expectedGameMode);
                    Assert.AreEqual(expectedGameMode, actualGameMode, "GameModeChanged event should have correct argument");
                }
                finally
                {
                    _gameModel.GameModeChanged -= handler;
                }
            }

            [TestMethod]
            [DynamicData(nameof(GetAllTimeSpeeds))]
            public void TimeSpeedChanged_EventArgumentIsCorrect(TimeSpeed expectedTimeSpeed)
            {
                TimeSpeed actualTimeSpeed = TimeSpeed.Normal;

                EventHandler<TimeSpeed> handler = (_, e) =>
                {
                    actualTimeSpeed = e;
                };

                try
                {
                    _gameModel.TimeSpeedChanged += handler;
                    _gameModel.SetTimeSpeed(expectedTimeSpeed);
                    Assert.AreEqual(expectedTimeSpeed, actualTimeSpeed, "TimeSpeedChanged event should have correct argument");
                }
                finally
                {
                    _gameModel.TimeSpeedChanged -= handler;
                }
            }

            [TestMethod]
            public void GameOver_EventArgumentIsCorrect() { }

            [TestMethod]
            public void GameAdvanced_EventArgumentIsCorrect()
            {
                List<Tuple<int, int>> actualTrees = [];

                EventHandler<List<Tuple<int, int>>> handler = (_, e) =>
                {
                    actualTrees = e;
                };

                try
                {

                    _gameModel.GameAdvanced += handler;
                    // Indítsunk egy új játékot, hogy biztosan legyen mapunk és fáink
                    _gameModel.NewGame();
                    // Szimuláljuk a timer tick eseményét 10x (egyelőre ennyi kell egy event kiváltáshoz)
                    for (int i = 0; i < 10; i++)
                    {
                        _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    }
                    Assert.IsNotEmpty(actualTrees, "GameAdvanced event after 10 timer ticks should raise and return with non-empty trees changed");
                }
                finally
                {
                    _gameModel.GameAdvanced -= handler;
                }
            }

            [TestMethod]
            public void InfrastructureBuilt_EventArgumentIsCorrect() { }
        }
    }
}
