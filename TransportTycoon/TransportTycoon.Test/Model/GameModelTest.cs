using NSubstitute;
using NSubstitute.ClearExtensions;
using TransportTycoon.MapData;
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
                _gameModel = new(Difficulty.Medium, 1000, _mockTimer);
            }

            [TestMethod]
            [DynamicData(nameof(GetAllGameModes))]
            public void GameModeChanged_EventArgumentIsCorrect(GameMode expectedGameMode)
            {
                GameModel gameModel = new(Difficulty.Medium, 1000, _mockTimer);
                GameMode actualGameMode = GameMode.Run;

                EventHandler<GameMode> handler = (_, e) =>
                {
                    actualGameMode = e;
                };

                try
                {
                    gameModel.GameModeChanged += handler;
                    gameModel.SetMode(expectedGameMode);
                    Assert.AreEqual(expectedGameMode, actualGameMode, "GameModeChanged event should have correct argument");
                }
                finally
                {
                    gameModel.GameModeChanged -= handler;
                }
            }

            [TestMethod]
            [DynamicData(nameof(GetAllTimeSpeeds))]
            public void TimeSpeedChanged_EventArgumentIsCorrect(TimeSpeed expectedTimeSpeed)
            {
                GameModel gameModel = new(Difficulty.Medium, 1000, _mockTimer);
                TimeSpeed actualTimeSpeed = TimeSpeed.Normal;

                EventHandler<TimeSpeed> handler = (_, e) =>
                {
                    actualTimeSpeed = e;
                };

                try
                {
                    gameModel.TimeSpeedChanged += handler;
                    gameModel.SetTimeSpeed(expectedTimeSpeed);
                    Assert.AreEqual(expectedTimeSpeed, actualTimeSpeed, "TimeSpeedChanged event should have correct argument");
                }
                finally
                {
                    gameModel.TimeSpeedChanged -= handler;
                }
            }

            [TestMethod]
            public void GameOver_EventArgumentIsCorrect() { }

            [TestMethod]
            public void GameAdvanced_EventArgumentIsCorrect()
            {
                GameModel gameModel = new(Difficulty.Medium, 1000, _mockTimer);
                List<Tuple<int, int>> actualTrees = [];

                EventHandler<List<Tuple<int, int>>> handler = (_, e) =>
                {
                    actualTrees = e;
                };

                try
                {
                    gameModel.GameAdvanced += handler;
                    // Indítsunk egy új játékot, hogy biztosan legyen mapunk és fáink
                    gameModel.NewGame();
                    // Szimuláljuk a timer tick eseményét 10x (egyelőre ennyi kell egy event kiváltáshoz)
                    for (int i = 0; i < 10; i++)
                    {
                        _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    }
                    Assert.IsNotEmpty(actualTrees, "GameAdvanced event after 10 timer ticks should raise and return with non-empty trees changed");
                }
                finally
                {
                    gameModel.GameAdvanced -= handler;
                }
            }

            [TestMethod]
            public void InfrastructureBuilt_EventArgumentIsCorrect() { }

            //Public methods

            [TestClass]
            public class GameModelHeightTests
            {
                // Segédmetódus a tesztkörnyezet felállításához
                private GameModel CreateEditorModelWithMap()
                {
                    var model = new GameModel(Difficulty.Medium, _mockTimer);
                    model.SetMode(GameMode.Editor);

                    // Feltöltjük a pályát magasság=2 síkságokkal, hogy az IsTileHeightPossible ne szálljon el null reference miatt
                    for (int i = 0; i < model.Map.Width; i++)
                    {
                        for (int j = 0; j < model.Map.Height; j++)
                        {
                            model.Map[i, j] = new Terrain(i, j, 2);
                        }
                    }
                    return model;
                }

                private ITimer _mockTimer = null!;

                [TestInitialize]
                public void Initialize()
                {
                    _mockTimer = Substitute.For<ITimer>();
                }

                [TestMethod]
                public void IncreaseHeight_DoesNothing_IfNotInEditorMode()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    model.SetMode(GameMode.Run); // Átváltjuk Run módba
                    int initialBalance = model.Balance;

                    // Act
                    model.IncreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(2, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance, model.Balance);
                }

                [TestMethod]
                public void IncreaseHeight_DoesNothing_IfHeightIsAlready4()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    var terrain = (Terrain)model.Map[5, 5];
                    terrain.IncreaseHeight();
                    terrain.IncreaseHeight(); // Magasság most már 4 (max)
                    int initialBalance = model.Balance;

                    // Act
                    model.IncreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(4, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance, model.Balance); // A pénz nem vonódhat le
                }

                [TestMethod]
                public void IncreaseHeight_Costs100Balance_WhenNoTrees()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    int initialBalance = model.Balance;
                    bool eventFired = false;
                    model.FieldChanged += (s, e) => eventFired = true;

                    // Act
                    model.IncreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(3, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance - 100, model.Balance);
                    Assert.IsTrue(eventFired);
                }

                [TestMethod]
                public void IncreaseHeight_Costs150Balance_WhenTreesExist()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    var terrain = (Terrain)model.Map[5, 5];
                    terrain.Grow(); // Van már 1 fa (amit ki kell vágni, plusz 50 pénz)
                    int initialBalance = model.Balance;

                    // Act
                    model.IncreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(initialBalance - 150, model.Balance);
                }

                [TestMethod]
                public void DecreaseHeight_DoesNothing_IfNotInEditorMode()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    model.SetMode(GameMode.Run);
                    int initialBalance = model.Balance;

                    // Act
                    model.DecreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(2, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance, model.Balance);
                }

                [TestMethod]
                public void DecreaseHeight_DoesNothing_IfHeightIsAlready1()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    var terrain = (Terrain)model.Map[5, 5];
                    terrain.DecreaseHeight(); // Magasság most már 1 (min)
                    int initialBalance = model.Balance;

                    // Act
                    model.DecreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(1, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance, model.Balance);
                }

                [TestMethod]
                public void DecreaseHeight_Costs100Balance_WhenNoTrees()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    int initialBalance = model.Balance;

                    // Act
                    model.DecreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(1, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance - 100, model.Balance);
                }

                [TestMethod]
                public void DecreaseHeight_Costs150Balance_WhenTreesExist()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    var terrain = (Terrain)model.Map[5, 5];
                    terrain.Grow(); // Van 1 fa
                    int initialBalance = model.Balance;

                    // Act
                    model.DecreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(1, model.Map[5, 5].Height);
                    Assert.AreEqual(initialBalance - 150, model.Balance);
                }

                [TestMethod]
                public void DecreaseHeight_DoesNothing_IfTileHeightDifferenceWouldBeTooHigh()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap();
                    var neighbor = (Terrain)model.Map[4, 5];
                    neighbor.IncreaseHeight();
                    neighbor.IncreaseHeight(); // Felső szomszéd most 4-es

                    int initialBalance = model.Balance;

                    // Act
                    // Megpróbáljuk 1-re vinni a teszt csempét. 
                    // A különbség 4 és 1 között 3 lenne, ami illegális.
                    model.DecreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(2, model.Map[5, 5].Height); // Nem csökkent a magasság
                    Assert.AreEqual(initialBalance, model.Balance); // Nem vett le pénzt
                }
                

                private GameModel CreateEditorModelWithMap(int startingBalance = 1000)
                {
                    var model = new GameModel(startingBalance, _mockTimer);
                    model.SetMode(GameMode.Editor);

                    // Feltöltjük a pályát magasság=2 síkságokkal, hogy a szomszédvizsgálat ne dobjon kivételt
                    for (int i = 0; i < model.Map.Width; i++)
                    {
                        for (int j = 0; j < model.Map.Height; j++)
                        {
                            model.Map[i, j] = new Terrain(i, j, 2);
                        }
                    }
                    return model;
                }
                [TestMethod]
                public void IncreaseHeight_TriggersGameOver_WhenBalanceDropsToZeroOrBelow()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap(100);
                    bool gameOverFired = false;
                    model.GameOver += (s, e) => gameOverFired = true;

                    // Act
                    model.IncreaseHeight(5, 5);

                    // Assert
                    Assert.AreEqual(0, model.Balance, "A balance-nak 0-ra kellett csökkennie.");
                    Assert.IsTrue(model.IsGameOver, "Az IsGameOver property-nek true-nak kell lennie.");
                    Assert.IsTrue(gameOverFired, "A GameOver eseménynek el kellett sülnie.");
                }

                [TestMethod]
                public void DecreaseHeight_TriggersGameOver_WhenBalanceDropsToZeroOrBelow()
                {
                    // Arrange
                    var model = CreateEditorModelWithMap(150);
                    var terrain = (Terrain)model.Map[5, 5];
                    // Hogy a magasságot lehessen csökkenteni, előbb felvisszük 3-ra.
                    // (A manuális Height növelés a Terrain-ben nem kerül pénzbe)
                    terrain.Grow();
                    terrain.IncreaseHeight();

                    bool gameOverFired = false;
                    model.GameOver += (s, e) => gameOverFired = true;

                    // Act
                    model.DecreaseHeight(5, 5); // -100 (alap) - 50 (fa kivágás) = -150

                    // Assert
                    Assert.AreEqual(0, model.Balance, "A balance-nak 0-ra kellett csökkennie a fa és az ásás miatt.");
                    Assert.IsTrue(model.IsGameOver, "Az IsGameOver property-nek true-nak kell lennie.");
                    Assert.IsTrue(gameOverFired, "A GameOver eseménynek el kellett sülnie.");
                }
                //Forest methods
            }
        }
    }
}
