using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using TransportTycoon.Persistence;
using ITimer = TransportTycoon.Model.ITimer;
using VehicleType = TransportTycoon.Model.VehicleType;

namespace TransportTycoon.Test.Model;

public class GameModelTest
{
    public class ConstructorTest
    {
        private readonly ITimer _mockTimer;
        private readonly IPersistence _mockPersistence;
        private readonly GameTable _mockMap;

        public ConstructorTest()
        {
            var mockMapGenerator = Substitute.For<IMapGenerator>();
            MapGenerationContext context = new();

            _mockTimer = Substitute.For<ITimer>();
            _mockMap = Substitute.For<GameTable>(mockMapGenerator, context);
            _mockPersistence = Substitute.For<IPersistence>();
        }

        [Fact]
        public void Constructor_WithAllParameters()
        {
            Difficulty difficulty = Difficulty.Hard;
            int balance = 5_000;
            GameModel gameModel = new(_mockMap, _mockTimer, _mockPersistence, difficulty, balance);

            Assert.Equal(difficulty, gameModel.Difficulty);
            Assert.Equal(balance, gameModel.Balance);

            Assert.Equal(GameMode.Run, gameModel.Mode);
            Assert.Equal(0UL, gameModel.GameTime);

            // Timer mock tesztek
            // Feliratkoztak az Elapsed eseményre
            _mockTimer.Received().Elapsed += Arg.Any<EventHandler>();
        }

        [Fact]
        public void Constructor_WithDefaultValues()
        {
            GameModel gameModel = new(_mockMap, _mockTimer, _mockPersistence);

            Assert.Equal(GameModel.DefaultDifficulty, gameModel.Difficulty);
            Assert.Equal(GameModel.DefaultBalance, gameModel.Balance);
        }
    }

    public class EventTest
    {
        //public class EnumEnumerable<T> : IEnumerable<T[]> where T : struct, Enum
        //{
        //    private readonly List<T[]> _data = [.. Enum.GetValues<T>().Select(v => new T[] { v })];

        //    public IEnumerator<T[]> GetEnumerator() => _data.GetEnumerator();
        //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        //}

        /// <summary>
        /// A helper class that generates test data for all values of a given enum type <typeparamref name="T"/>.
        /// It inherits from <see cref="TheoryData{T}"/>, which is a convenient way to provide data for xUnit theories.
        /// The constructor populates the data by iterating over all values of the enum <typeparamref name="T"/> and adding them to the theory data.
        /// This allows us to easily run parameterized tests for each value of the enum without manually specifying them.
        /// </summary>
        /// <typeparam name="T">The enum type for which to generate test data.</typeparam>
        public class EnumEnumerable<T> : TheoryData<T> where T : struct, Enum
        {
            public EnumEnumerable()
            {
                foreach (var value in Enum.GetValues<T>())
                {
                    Add(value);
                }
            }
        }

        public class EventRaisedTest
        {
            private readonly GameModel _gameModel;
            private readonly ITimer _mockTimer;
            private readonly IPersistence _mockPersistence;

            public EventRaisedTest()
            {
                var mockMapGenerator = Substitute.For<IMapGenerator>();
                MapGenerationContext context = new();
                var mockGameTable = Substitute.For<GameTable>(mockMapGenerator, context);

                _mockTimer = Substitute.For<ITimer>();
                _mockPersistence = Substitute.For<IPersistence>();
                _gameModel = new(mockGameTable, _mockTimer, _mockPersistence);
            }

            [Fact]
            public void NewGameCreated_EventIsRaised()
            {
                bool raised = false;

                void Handler(object? _1, EventArgs _2)
                {
                    raised = true;
                }

                try
                {
                    _gameModel.NewGameCreated += Handler;
                    _gameModel.NewGame();
                    Assert.True(raised, "NewGameCreated should be raised after creating a new game");
                }
                finally
                {
                    _gameModel.NewGameCreated -= Handler;
                }
            }

            [Theory]
            [ClassData(typeof(EnumEnumerable<GameMode>))]
            public void GameModeChanged_EventIsRaised(GameMode gameMode)
            {
                bool raised = false;

                void Handler(object? _1, GameMode _2)
                {
                    raised = true;
                }

                try
                {
                    _gameModel.GameModeChanged += Handler;
                    _gameModel.Mode = gameMode;
                    Assert.True(raised, "GameModeChanged should be raised after changing the game mode");
                }
                finally
                {
                    _gameModel.GameModeChanged -= Handler;
                }
            }

            [Theory]
            [ClassData(typeof(EnumEnumerable<TimeSpeed>))]
            public void TimeSpeedChanged_EventIsRaised(TimeSpeed timeSpeed)
            {
                bool raised = false;

                void Handler(object? _1, TimeSpeed _2)
                {
                    raised = true;
                }

                try
                {
                    _gameModel.TimeSpeedChanged += Handler;
                    _gameModel.TimeSpeed = timeSpeed;
                    Assert.True(raised, "TimeSpeedChanged should be raised after changing the game speed");
                }
                finally
                {
                    _gameModel.TimeSpeedChanged -= Handler;
                }
            }

            [Fact]
            public void GameOver_EventIsRaised()
            {
                bool raised = false;

                void Handler(object? _1, List<Tuple<int, int>> _2)
                {
                    raised = true;
                }

                try
                {
                    _gameModel.GameAdvanced += Handler;
                    // Szimuláljuk a timer tick eseményét 10x (egyelőre ennyi kell egy event kiváltáshoz)
                    for (int i = 0; i < 10; i++)
                    {
                        _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    }
                    Assert.True(raised, "GameAdvanced event should be raised after 10 timer ticks");
                }
                finally
                {
                    _gameModel.GameAdvanced -= Handler;
                }
            }

            [Fact]
            public void GameTicked_EventIsRaised()
            {
                bool raised = false;

                void Handler(object? _1, EventArgs _2)
                {
                    raised = true;
                }

                try
                {
                    _gameModel.GameTicked += Handler;
                    // Szimuláljuk a timer tick eseményét
                    _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
                    Assert.True(raised, "GameTicked should be raised after 1 timer tick");
                }
                finally
                {
                    _gameModel.GameTicked -= Handler;
                }
            }

            [Fact]
            public void GameAdvanced_EventIsRaised() { }

            [Fact]
            public void InfrastructureBuilt_EventIsRaised() { }
        }

        public class EventArgumentTest
        {
            private readonly GameModel _gameModel;
            private readonly ITimer _mockTimer;
            private readonly GameTable _mockMap;
            private readonly IPersistence _mockPersistence;

            public EventArgumentTest()
            {
                IMapGenerator mockGenerator = Substitute.For<IMapGenerator>();
                MapGenerationContext context = new();

                _mockMap = Substitute.For<GameTable>(mockGenerator, context);
                _mockTimer = Substitute.For<ITimer>();
                _mockPersistence = Substitute.For<IPersistence>();
                _gameModel = new(_mockMap, _mockTimer, _mockPersistence);
            }

            [Theory]
            [ClassData(typeof(EnumEnumerable<GameMode>))]
            public void GameModeChanged_EventArgumentIsCorrect(GameMode expectedGameMode)
            {
                GameModel gameModel = new(_mockMap, _mockTimer, _mockPersistence);
                GameMode actualGameMode = GameMode.Run;

                void Handler(object? _1, GameMode e)
                {
                    actualGameMode = e;
                }

                try
                {
                    gameModel.GameModeChanged += Handler;
                    gameModel.Mode = expectedGameMode;
                    Assert.Equal(expectedGameMode, actualGameMode);
                }
                finally
                {
                    gameModel.GameModeChanged -= Handler;
                }
            }

            [Theory]
            [ClassData(typeof(EnumEnumerable<TimeSpeed>))]
            public void TimeSpeedChanged_EventArgumentIsCorrect(TimeSpeed expectedTimeSpeed)
            {
                GameModel gameModel = new(_mockMap, _mockTimer, _mockPersistence);
                TimeSpeed actualTimeSpeed = TimeSpeed.Normal;

                void Handler(object? _1, TimeSpeed e)
                {
                    actualTimeSpeed = e;
                }

                try
                {
                    gameModel.TimeSpeedChanged += Handler;
                    gameModel.TimeSpeed = expectedTimeSpeed;
                    Assert.Equal(expectedTimeSpeed, actualTimeSpeed);
                }
                finally
                {
                    gameModel.TimeSpeedChanged -= Handler;
                }
            }

            [Fact]
            public void GameOver_EventArgumentIsCorrect() { }

            // TODO: reimplement this with mocking in the future
            //[TestMethod]
            //public void GameAdvanced_EventArgumentIsCorrect()
            //{
            //    GameModel gameModel = new(Difficulty.Medium, 1000, _mockTimer);
            //    List<Tuple<int, int>> actualTrees = [];

            //    EventHandler<List<Tuple<int, int>>> handler = (_, e) =>
            //    {
            //        actualTrees = e;
            //    };

            //    try
            //    {
            //        gameModel.GameAdvanced += handler;
            //        // Indítsunk egy új játékot, hogy biztosan legyen mapunk és fáink
            //        gameModel.NewGame();
            //        // Szimuláljuk a timer tick eseményét 10x (egyelőre ennyi kell egy event kiváltáshoz)
            //        for (int i = 0; i < 10; i++)
            //        {
            //            _mockTimer.Elapsed += Raise.EventWith(this, EventArgs.Empty);
            //        }
            //        Assert.IsNotEmpty(actualTrees, "GameAdvanced event after 10 timer ticks should raise and return with non-empty trees changed");
            //    }
            //    finally
            //    {
            //        gameModel.GameAdvanced -= handler;
            //    }
            //}

            [Fact]
            public void InfrastructureBuilt_EventArgumentIsCorrect() { }
        }
    }

    public class VehicleTests
    {
        private GameModel _gameModel;
        private readonly GameTable _mockMap;
        private readonly ITimer _mockTimer;
        private readonly IPersistence _mockPersistence;

        public VehicleTests()
        {
            var mockMapGenerator = Substitute.For<IMapGenerator>();
            MapGenerationContext context = new();

            _mockTimer = Substitute.For<ITimer>();
            _mockMap = Substitute.For<GameTable>(mockMapGenerator, context);
            _mockPersistence = Substitute.For<IPersistence>();

            // Initialize GameModel with a default high balance for tests
            _gameModel = new(_mockMap, _mockTimer, _mockPersistence, Difficulty.Medium, 10000);
        }

        [Fact]
        public void BuyVehicle_LocationIsNotStop_ReturnsNullAndDoesNotChangeBalance()
        {
            // Arrange
            int x = 0, y = 0;
            // Mock the map to return a regular Terrain (not a Stop) at the given coordinates
            _mockMap[x, y] = new Terrain(x, y, 1);

            // Re-initialize GameModel to strictly control the starting balance
            _gameModel = new(_mockMap, _mockTimer, _mockPersistence, Difficulty.Medium, 10000);
            int initialVehiclesCount = _gameModel.Vehicles.Count;

            // Act
            var result = _gameModel.BuyVehicle(x, y, VehicleType.Van);

            // Assert
            Assert.Null(result);
            Assert.Equal(10000, _gameModel.Balance);
            Assert.Equal(initialVehiclesCount, _gameModel.Vehicles.Count);
        }

        [Fact]
        public void BuyVehicle_SufficientBalance_DeductsBalanceAddsToVehiclesAndRaisesEvent()
        {
            // Arrange
            int x = 1, y = 1;
            // Mock the map to return a Stop at the given coordinates
            _mockMap[x, y] = new Stop(x, y, 1);

            _gameModel = new GameModel(_mockMap, _mockTimer, _mockPersistence, Difficulty.Medium, 5000);

            bool eventRaised = false;
            _gameModel.BalanceChanged += (sender, args) => { eventRaised = true; };

            // Act
            var result = _gameModel.BuyVehicle(x, y, VehicleType.Van);

            // Assert
            Assert.NotNull(result);
            // We assume Van is a valid class derived from Vehicle
            Assert.Equal(typeof(Van).Name, result.GetType().Name);

            Assert.Equal(5000 - result.Price, _gameModel.Balance);
            Assert.Contains(result, _gameModel.Vehicles);
            Assert.True(eventRaised, "BalanceChanged event should be invoked.");
        }

        [Fact]
        public void BuyVehicle_InsufficientBalance_DoesNotDeductBalanceOrAddToList()
        {
            // Arrange
            int x = 2, y = 2;
            _mockMap[x, y] = new Stop(x, y, 1);

            // Initialize with 0 balance
            _gameModel = new GameModel(_mockMap, _mockTimer, _mockPersistence, Difficulty.Medium, 0);

            bool eventRaised = false;
            _gameModel.BalanceChanged += (sender, args) => { eventRaised = true; };

            // Act
            var result = _gameModel.BuyVehicle(x, y, VehicleType.Truck);

            // Assert
            // Note: Based on the current implementation in GameModel, it STILL returns the instantiated vehicle object, 
            // but does not deduct balance or add it to the list.
            Assert.NotNull(result);
            Assert.Equal(typeof(Truck).Name, result.GetType().Name);

            Assert.Equal(0, _gameModel.Balance);
            Assert.DoesNotContain(result, _gameModel.Vehicles);
            Assert.False(eventRaised);
        }

        [Fact]
        public void BuyVehicle_DifferentVehicleType_CreatesCorrectInstance()
        {
            // Arrange
            int x = 3, y = 3;
            _mockMap[x, y] = new Stop(x, y, 1);

            _gameModel = new GameModel(_mockMap, _mockTimer, _mockPersistence, Difficulty.Medium, 100000);

            // Act
            var result = _gameModel.BuyVehicle(x, y, VehicleType.BigBus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(BigBus).Name, result.GetType().Name);
        }
    }
}
