using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using TransportTycoon.Model.Graph;
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
            public void GameAdvanced_EventIsRaised()
            {
                bool raised = false;
                void Handler(object? _1, List<Tuple<int, int>> _2) { raised = true; }
                try
                {
                    _gameModel.GameAdvanced += Handler;
                    // Simulate 10 timer ticks to trigger GameAdvanced
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
            public void InfrastructureBuilt_EventIsRaised()
            {
                // Use a real GameTable for correct indexer behavior
                var mapGen = Substitute.For<IMapGenerator>();
                var context = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
                var realMap = new GameTable(mapGen, context);
                // Fill the map with Terrain
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        realMap[i, j] = new Terrain(i, j, 1);
                var model = new GameModel(realMap, _mockTimer, _mockPersistence);
                bool raised = false;
                void Handler(object? _1, List<(int, int)> _2) { raised = true; }
                try
                {
                    model.InfrastructureBuilt += Handler;
                    int x = 5, y = 5;
                    model.Mode = GameMode.Editor;
                    model.BuildRoad(x, y);
                    Assert.True(raised, "InfrastructureBuilt event should be raised after building road");
                }
                finally
                {
                    model.InfrastructureBuilt -= Handler;
                }
            }
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
            public void GameOver_EventArgumentIsCorrect()
            {
                // Use a real GameTable for correct indexer behavior
                var mapGen = Substitute.For<IMapGenerator>();
                var context = new MapGenerationContext(2, 2, 1, new MapGenerationSettings());
                var realMap = new GameTable(mapGen, context);
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        realMap[i, j] = new Terrain(i, j, 2); // Height 2 so we can decrease
                var model = new GameModel(realMap, _mockTimer, _mockPersistence, Difficulty.Medium, 1);
                TransportTycoonEventArgs? eventArgs = null;
                void Handler(object? _1, TransportTycoonEventArgs e) { eventArgs = e; }
                try
                {
                    model.GameOver += Handler;
                    model.Mode = GameMode.Editor;
                    model.DecreaseHeight(0, 0); // Should reduce balance below 0
                    Assert.NotNull(eventArgs);
                }
                finally
                {
                    model.GameOver -= Handler;
                }
            }

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
            public void InfrastructureBuilt_EventArgumentIsCorrect()
            {
                // Use a real GameTable for correct indexer behavior
                var mapGen = Substitute.For<IMapGenerator>();
                var context = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
                var realMap = new GameTable(mapGen, context);
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        realMap[i, j] = new Terrain(i, j, 1);
                var model = new GameModel(realMap, _mockTimer, _mockPersistence);
                List<(int, int)>? eventArgs = null;
                void Handler(object? _1, List<(int, int)> e) { eventArgs = e; }
                try
                {
                    model.InfrastructureBuilt += Handler;
                    int x = 7, y = 7;
                    model.Mode = GameMode.Editor;
                    model.BuildRoad(x, y);
                    Assert.NotNull(eventArgs);
                    Assert.Contains((x, y), eventArgs!);
                }
                finally
                {
                    model.InfrastructureBuilt -= Handler;
                }
            }
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

    public class RouteAndGraphTests
    {
        private readonly MapGenerationContext _context = new(10, 10, 1, new MapGenerationSettings());
        private readonly GameModel _model;

        public RouteAndGraphTests()
        {
            var mapGen = Substitute.For<IMapGenerator>();
            mapGen
                .GenerateMap(_context)
                .Returns(
                ci =>
                    {
                        var ctx = _context;
                        var table = new IField[ctx.Width, ctx.Height];
                        for (int i = 0; i < ctx.Width; i++)
                        {
                            for (int j = 0; j < ctx.Height; j++)
                            {
                                table[i, j] = new Terrain(i, j, 1);
                            }
                        }
                        return (table, []);
                    }
                );

            var map = new GameTable(mapGen, _context);

            _model = new(map, Substitute.For<ITimer>(), Substitute.For<IPersistence>());
        }

        private void CallRebuildGraph(GameModel model)
        {
            model.Mode = GameMode.Editor; // Ensure we're in a mode that allows graph rebuilding
            model.Mode = GameMode.Run; // Trigger graph rebuilding
        }

        [Fact]
        public void RebuildGraph_DoesNothingIfMapNotGenerated()
        {
            // Arrange
            var oldGraph = _model.GraphNetwork;

            // Act
            CallRebuildGraph(_model);

            // Assert
            Assert.Same(oldGraph, _model.GraphNetwork);
        }

        [Fact]
        public void RebuildGraph_UpdatesGraphAndVehicleRoutes()
        {
            // Arrange
            _model.NewGame();

            var node = new Node(1, 1, typeof(Stop));
            _model.Map[1, 1] = new Stop(1, 1, 1);
            var prouth = new Prouth([node]);
            var vehicle = new Van(1, 1, Direction.Up)
            {
                Prouth = prouth
            };
            _model.Vehicles.Add(vehicle);


            // Act
            CallRebuildGraph(_model);

            // Assert
            Assert.NotNull(vehicle.Prouth);
            Assert.NotEmpty(vehicle.Prouth.Stops);
        }

        [Fact]
        public void DefineRoute_AddsStopAndRaisesEvent()
        {
            // Arrange
            _model.NewGame();
            int x = 2, y = 3;
            _model.Map[x, y] = new Stop(x, y, 1);
            bool eventRaised = false;
            _model.SelectedStopFieldsChanged += (_, stops) => eventRaised = true;

            // Act
            _model.DefineRoute(x, y);

            // Assert
            Assert.Contains(_model.SelectedStopFields, s => s.X == x && s.Y == y && s.Height == 1);
            Assert.True(eventRaised);
        }

        [Fact]
        public void DefineRoute_DoesNotAddDuplicateStop()
        {
            // Arrange
            _model.NewGame();
            int x = 2, y = 3;
            var stop = new Stop(x, y, 1);
            _model.Map[x, y] = stop;
            _model.SelectedStopFields.Add(stop);

            // Act
            _model.DefineRoute(x, y);

            // Assert
            Assert.Single(_model.SelectedStopFields, s => s.X == x && s.Y == y && s.Height == 1);
        }

        [Fact]
        public void QueryRoute_SetsSelectedStopFieldsFromVehicleRouteAndRaisesEvent()
        {
            // Arrange
            _model.NewGame();
            int x = 1, y = 1;
            var stop = new Stop(x, y, 1);
            _model.Map[x, y] = stop;
            var node = new Node(x, y, typeof(Stop));
            var prouth = new Prouth([node]);
            var vehicle = new Van(x, y, Direction.Up)
            {
                Prouth = prouth
            };
            _model.Vehicles.Add(vehicle);
            bool eventRaised = false;
            _model.SelectedStopFieldsChanged += (_, stops) => eventRaised = true;

            // Act
            _model.QueryRoute(x, y);

            // Assert
            Assert.Contains(_model.SelectedStopFields, s => s.X == x && s.Y == y && s.Height == 1);
            Assert.True(eventRaised);
        }

        [Fact]
        public void DeleteRoute_RemovesStopAndRaisesEvent()
        {
            // Arrange
            _model.NewGame();
            int x = 2, y = 2;
            var stop = new Stop(x, y, 1);
            _model.SelectedStopFields.Add(stop);
            bool eventRaised = false;
            _model.SelectedStopFieldsChanged += (_, stops) => eventRaised = true;

            // Act
            _model.DeleteRoute(x, y);

            // Assert
            Assert.DoesNotContain(_model.SelectedStopFields, s => s.X == x && s.Y == y && s.Height == 1);
            Assert.True(eventRaised);
        }

        [Fact]
        public void DeleteRoute_ClearAllStopsWithMinusOne()
        {
            // Arrange
            _model.SelectedStopFields.Add(new Stop(1, 1, 1));
            _model.SelectedStopFields.Add(new Stop(2, 2, 1));

            // Act
            _model.DeleteRoute(-1, -1);

            // Assert
            Assert.Empty(_model.SelectedStopFields);
        }
    }
    public class GameModelHeightTests
    {
        // Segédmetódus egy alap GameModel és egy 3x3-as tesztpálya létrehozásához
        private GameModel CreateTestModel(int initialBalance = 1000, int initialHeight = 2)
        {
            var timerMock = Substitute.For<ITimer>();
            var persistenceMock = Substitute.For<IPersistence>();
            var mapGenMock = Substitute.For<IMapGenerator>();

            var context = new MapGenerationContext(3, 3, 1, new MapGenerationSettings());
            var table = new GameTable(mapGenMock, context);

            // 3x3-as pálya feltöltése Terrain-ekkel
            var fields = new IField[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    fields[i, j] = new Terrain(i, j, initialHeight);
                }
            }

            mapGenMock.GenerateMap(context).Returns((fields, new List<BuildingEntity>()));
            table.GenerateMap();

            var model = new GameModel(table, timerMock, persistenceMock, Difficulty.Medium, initialBalance)
            {
                Mode = GameMode.Editor // A metódusok csak Editor módban működnek
            };

            return model;
        }

        [Fact]
        public void IncreaseHeight_ValidTerrain_IncreasesHeightAndDeducts100Balance()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 2);
            int startBalance = model.Balance;

            // Act
            model.IncreaseHeight(1, 1);

            // Assert
            Assert.Equal(3, model.Map[1, 1].Height);
            Assert.Equal(startBalance - 100, model.Balance);
        }

        [Fact]
        public void IncreaseHeight_WithTrees_Deducts150Balance()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 2);
            var terrain = (Terrain)model.Map[1, 1];
            terrain.Trees = 2; // Fákat adunk a területhez
            model.Map.UpdateTable(1, 1, terrain);
            int startBalance = model.Balance;

            // Act
            model.IncreaseHeight(1, 1);

            // Assert
            Assert.Equal(3, model.Map[1, 1].Height);
            Assert.Equal(startBalance - 150, model.Balance);
        }

        [Fact]
        public void IncreaseHeight_NotEditorMode_DoesNothing()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 2);
            model.Mode = GameMode.Run; // Átállítjuk futás módba
            int startBalance = model.Balance;

            // Act
            model.IncreaseHeight(1, 1);

            // Assert
            Assert.Equal(2, model.Map[1, 1].Height);
            Assert.Equal(startBalance, model.Balance); // A pénz nem változhatott
        }

        [Fact]
        public void IncreaseHeight_MaxHeight_DoesNothing()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 4); // Max magasság

            // Act
            model.IncreaseHeight(1, 1);

            // Assert
            Assert.Equal(4, model.Map[1, 1].Height); // Nem nőhet 5-re
        }

        [Fact]
        public void DecreaseHeight_ValidTerrain_DecreasesHeightAndDeducts100Balance()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 3);
            int startBalance = model.Balance;

            // Act
            model.DecreaseHeight(1, 1);

            // Assert
            Assert.Equal(2, model.Map[1, 1].Height);
            Assert.Equal(startBalance - 100, model.Balance);
        }

        [Fact]
        public void DecreaseHeight_MinHeight_DoesNothing()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 1); // Min magasság

            // Act
            model.DecreaseHeight(1, 1);

            // Assert
            Assert.Equal(1, model.Map[1, 1].Height); // Nem mehet 0-ra
        }

        [Fact]
        public void IncreaseHeight_FiresFieldAndBalanceChangedEvents()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 1000, initialHeight: 2);
            bool fieldChangedFired = false;
            bool balanceChangedFired = false;

            model.FieldChanged += (sender, args) => fieldChangedFired = true;
            model.BalanceChanged += (sender, args) => balanceChangedFired = true;

            // Act
            model.IncreaseHeight(1, 1);

            // Assert
            Assert.True(fieldChangedFired);
            Assert.True(balanceChangedFired);
        }

        [Fact]
        public void DecreaseHeight_CausesGameOver_WhenBalanceDropsBelowZero()
        {
            // Arrange
            var model = CreateTestModel(initialBalance: 50, initialHeight: 3); // 50 a kezdeti egyenleg, a módosítás 100-ba kerül
            bool gameOverFired = false;

            model.GameOver += (sender, args) => gameOverFired = true;

            // Act
            model.DecreaseHeight(1, 1);

            // Assert
            Assert.True(gameOverFired);
            Assert.True(model.IsGameOver);
            Assert.Equal(GameMode.Paused, model.Mode); // A GameMode.Paused állapotba kell váltania a játéknak
        }
    }
}
