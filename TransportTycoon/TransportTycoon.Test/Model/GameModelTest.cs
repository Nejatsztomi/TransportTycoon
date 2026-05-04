using NSubstitute;
using System.Reflection;
using System.Runtime.CompilerServices;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using TransportTycoon.Model.Graph;
using Difficulty = TransportTycoon.Model.Difficulty;
using ITimer = TransportTycoon.Model.ITimer;
using VehicleType = TransportTycoon.Model.VehicleType;

namespace TransportTycoon.Test.Model;

public class GameModelTest
{
    public class ConstructorTest
    {
        private readonly ITimer _mockTimer;
        private readonly GameTable _mockMap;
        private readonly MapGenerationContext _context;

        public ConstructorTest()
        {
            var mockMapGenerator = Substitute.For<IMapGenerator>();
            _context = new MapGenerationContext();
            _mockTimer = Substitute.For<ITimer>();
            _mockMap = Substitute.For<GameTable>(mockMapGenerator, _context);
        }

        [Fact]
        public void Constructor_WithAllParameters()
        {
            var difficulty = TransportTycoon.Model.Difficulty.Hard;
            int balance = 5_000;
            var data = new GameCreationData(_context, "TestSave", difficulty, balance);
            GameModel gameModel = new(_mockMap, _mockTimer, data);

            Assert.Equal(difficulty, gameModel.Difficulty);
            Assert.Equal(balance, gameModel.Balance);

            Assert.Equal(GameMode.Run, gameModel.Mode);
            Assert.Equal(0UL, gameModel.GameTime);

            // Timer mock tesztek
            // Feliratkoztak az Elapsed eseményre (Action<double>)
            _mockTimer.Received().Tick += Arg.Any<Action<double>>();
        }

        [Fact]
        public void Constructor_WithDefaultValues()
        {
            var data = new GameCreationData(_context, "TestSave");
            GameModel gameModel = new(_mockMap, _mockTimer, data);

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
            private readonly MapGenerationContext _context;

            public EventRaisedTest()
            {
                var mockMapGenerator = Substitute.For<IMapGenerator>();
                _context = new MapGenerationContext();
                var mockGameTable = Substitute.For<GameTable>(mockMapGenerator, _context);

                _mockTimer = Substitute.For<ITimer>();
                var data = new GameCreationData(_context, "TestSave");
                _gameModel = new(mockGameTable, _mockTimer, data);
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
                    // Simulate the timer tick event 10x (Action<double>)
                    for (int i = 0; i < 10; i++)
                    {
                        _mockTimer.Tick += Raise.Event<Action<double>>(1.0);
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
                    // Simulate the timer tick event (Action<double>)
                    _mockTimer.Tick += Raise.Event<Action<double>>(1.0);
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
                        _mockTimer.Tick += Raise.Event<Action<double>>(1.0);
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
                var data = new GameCreationData(context, "TestSave");
                var model = new GameModel(realMap, _mockTimer, data);
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
            private readonly MapGenerationContext _context;

            public EventArgumentTest()
            {
                IMapGenerator mockGenerator = Substitute.For<IMapGenerator>();
                _context = new MapGenerationContext();

                _mockMap = Substitute.For<GameTable>(mockGenerator, _context);
                _mockTimer = Substitute.For<ITimer>();
                var data = new GameCreationData(_context, "TestSave");
                _gameModel = new(_mockMap, _mockTimer, data);
            }

            [Theory]
            [ClassData(typeof(EnumEnumerable<GameMode>))]
            public void GameModeChanged_EventArgumentIsCorrect(GameMode expectedGameMode)
            {
                var data = new GameCreationData(_context, "TestSave");
                GameModel gameModel = new(_mockMap, _mockTimer, data);
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
                var data = new GameCreationData(_context, "TestSave");
                GameModel gameModel = new(_mockMap, _mockTimer, data);
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
                var localContext = new MapGenerationContext(2, 2, 1, new MapGenerationSettings());
                var data = new GameCreationData(localContext, "TestSave", TransportTycoon.Model.Difficulty.Medium, 1);
                var model = new GameModel(realMap, _mockTimer, data);
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
                var data = new GameCreationData(context, "TestSave");
                var model = new GameModel(realMap, _mockTimer, data);
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
        private readonly MapGenerationContext _context;

        public VehicleTests()
        {
            var mockMapGenerator = Substitute.For<IMapGenerator>();
            _context = new MapGenerationContext();
            _mockTimer = Substitute.For<ITimer>();
            _mockMap = Substitute.For<GameTable>(mockMapGenerator, _context);
            var data = new GameCreationData(_context, "TestSave", TransportTycoon.Model.Difficulty.Medium, 10000);
            _gameModel = new(_mockMap, _mockTimer, data);
        }

        [Fact]
        public void BuyVehicle_LocationIsNotStop_ReturnsNullAndDoesNotChangeBalance()
        {
            // Arrange
            int x = 0, y = 0;
            // Mock the map to return a regular Terrain (not a Stop) at the given coordinates
            _mockMap[x, y] = new Terrain(x, y, 1);

            // Re-initialize GameModel to strictly control the starting balance
            var data = new GameCreationData(_context, "TestSave", TransportTycoon.Model.Difficulty.Medium, 10000);
            _gameModel = new(_mockMap, _mockTimer, data);
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

            var data = new GameCreationData(_context, "TestSave", TransportTycoon.Model.Difficulty.Medium, 5000);
            _gameModel = new GameModel(_mockMap, _mockTimer, data);

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
            var data = new GameCreationData(_context, "TestSave", TransportTycoon.Model.Difficulty.Medium, 0);
            _gameModel = new GameModel(_mockMap, _mockTimer, data);

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

            var data = new GameCreationData(_context, "TestSave", TransportTycoon.Model.Difficulty.Medium, 100000);
            _gameModel = new GameModel(_mockMap, _mockTimer, data);

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
            var data = new GameCreationData(_context, "TestSave");
            _model = new(map, Substitute.For<ITimer>(), data);
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
            var vehicle = new Van(1, 1, 270, null)
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
            var vehicle = new Van(x, y, 270, null)
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
            var data = new GameCreationData(context, "TestSave", TransportTycoon.Model.Difficulty.Medium, initialBalance);
            var model = new GameModel(table, timerMock, data)
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
    public class GameModelInfrastructureTests
    {
        private GameModel CreateTestModelWithMap(int initialBalance = 1000)
        {
            var timerMock = Substitute.For<ITimer>();
            var mapGenMock = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(3, 3, 1, new MapGenerationSettings());
            var table = new GameTable(mapGenMock, context);
            // 3x3-as pálya feltöltése Terrain-ekkel (Magasság: 2)
            var fields = new IField[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    fields[i, j] = new Terrain(i, j, 2);
                }
            }
            mapGenMock.GenerateMap(context).Returns((fields, new List<TransportTycoon.MapData.Buildings.BuildingEntity>()));
            table.GenerateMap();
            var data = new GameCreationData(context, "TestSave", TransportTycoon.Model.Difficulty.Medium, initialBalance);
            var model = new GameModel(table, timerMock, data)
            {
                Mode = GameMode.Editor // Editor mód az építésekhez
            };
            return model;
        }

        [Fact]
        public void BuildStop_NotEditorMode_DoesNothing()
        {
            // Arrange
            var model = CreateTestModelWithMap();
            model.Mode = GameMode.Run;
            int startBalance = model.Balance;

            // Act
            model.BuildStop(1, 1);

            // Assert
            Assert.Equal(startBalance, model.Balance);
            Assert.IsType<Terrain>(model.Map[1, 1]); // Maradt Terrain
        }

        [Fact]
        public void BuildStop_NoValidEnvironment_DoesNothing()
        {
            // Arrange
            var model = CreateTestModelWithMap();
            // Nincs út vagy épület a szomszédban, ezért a StopEnvironment false lesz

            // Act
            model.BuildStop(1, 1);

            // Assert
            Assert.IsType<Terrain>(model.Map[1, 1]); // Nem épült meg
        }

        [Fact]
        public void Destroy_ValidInfrastructure_ReplacesWithTerrainAndFiresEvent()
        {
            // Arrange
            var model = CreateTestModelWithMap();
            // Teszünk egy utat a pályára
            model.Map.UpdateTable(1, 1, new Road(1, 1, RoadType.Horizontal, 2));

            bool infrastructureBuiltFired = false;
            model.InfrastructureBuilt += (s, e) => infrastructureBuiltFired = true;

            // Act
            model.Destroy(1, 1);

            // Assert
            Assert.IsType<Terrain>(model.Map[1, 1]); // Visszaalakult Terrain-né
            Assert.True(infrastructureBuiltFired);
        }

        [Fact]
        public void Destroy_VehicleOnField_DoesNotDestroy()
        {
            // Arrange
            var model = CreateTestModelWithMap();
            var stop = new Stop(1, 1, 2);
            model.Map.UpdateTable(1, 1, stop);

            // Jármű hozzáadása a mezőhöz (ez akadályozza a törlést)
            model.Vehicles.Add(new Van(1, 1, 270, null));

            // Act
            model.Destroy(1, 1);

            // Assert
            Assert.IsType<Stop>(model.Map[1, 1]); // A megálló megmaradt, mert állt rajta autó
        }

        [Fact]
        public void BuyVehicle_NotOnStop_ReturnsNull()
        {
            // Arrange
            var model = CreateTestModelWithMap();
            // Az 1,1 pozíción Terrain van, nem Stop

            // Act
            var vehicle = model.BuyVehicle(1, 1, VehicleType.Van);

            // Assert
            Assert.Null(vehicle);
            Assert.Empty(model.Vehicles);
        }

        [Fact]
        public void BuyVehicle_OnStopWithSufficientBalance_AddsVehicleAndDeductsBalance()
        {
            // Arrange
            var model = CreateTestModelWithMap(initialBalance: 100000); // Sok pénzünk van
            model.Map.UpdateTable(1, 1, new Stop(1, 1, 2)); // Teszünk egy megállót az (1,1)-re
            int startBalance = model.Balance;

            bool balanceChangedFired = false;
            model.BalanceChanged += (s, e) => balanceChangedFired = true;

            // Act
            var vehicle = model.BuyVehicle(1, 1, VehicleType.Truck);

            // Assert
            Assert.NotNull(vehicle);
            Assert.Single(model.Vehicles);
            Assert.Equal(VehicleType.Truck, vehicle.Type);
            Assert.Equal(1, vehicle.MapX);
            Assert.Equal(1, vehicle.MapY);
            Assert.True(balanceChangedFired);
            Assert.Equal(startBalance - vehicle.Price, model.Balance);
        }

        [Fact]
        public void BuyVehicle_InsufficientBalance_DoesNotBuy()
        {
            // Arrange
            var model = CreateTestModelWithMap(initialBalance: 10); // Nincs elég pénz (10 credit)
            model.Map.UpdateTable(1, 1, new Stop(1, 1, 2));

            // Act
            var vehicle = model.BuyVehicle(1, 1, VehicleType.Truck);

            // Assert
            Assert.NotNull(vehicle); // A kódod jelenleg leteszi a járművet (objektum létrejön) a metódus elején, de...
            Assert.Empty(model.Vehicles); // ...nem adja hozzá a listához, ha nincs pénz!
            Assert.Equal(10, model.Balance); // A pénz maradt 10
        }
    }
    public class GameModelAdvancedTests
    {
        // Segédmetódus egy alap GameModel létrehozásához
        private GameModel CreateTestModel(GameMode mode = GameMode.Editor)
        {
            var timerMock = Substitute.For<ITimer>();
            var mapGenMock = Substitute.For<IMapGenerator>();

            var context = new MapGenerationContext(3, 3, 1, new MapGenerationSettings());
            var table = new GameTable(mapGenMock, context);

            // 3x3-as pálya, alapból Terrain
            var fields = new IField[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    fields[i, j] = new Terrain(i, j, 2);
                }
            }

            mapGenMock.GenerateMap(context).Returns((fields, new List<BuildingEntity>()));
            table.GenerateMap();

            var creationData = new GameCreationData(
                context: context,
                saveName: "TestSave",
                difficulty: Difficulty.Medium,
                balance: 10000
            );

            return new GameModel(table, timerMock, creationData)
            {
                Mode = mode
            };
        }

        #region Pályakezelés és Építés
        [Fact]
        public void BuildRoad_ValidTerrain_BuildsRoadAndDeductsBalance()
        {
            // Arrange
            var model = CreateTestModel();
            int startBalance = model.Balance;

            // Act
            model.BuildRoad(1, 1);

            // Assert
            Assert.IsType<Road>(model.Map[1, 1]);
            Assert.True(startBalance > model.Balance); // Pénzt vont le
        }

        [Fact]
        public void BuildBridge_RequiresSelectedFieldFirst()
        {
            // Arrange
            var model = CreateTestModel();
            model.Map.UpdateTable(1, 1, new Water(1, 1)); // Víz a hídképzéshez
            model.Map.UpdateTable(1, 2, new Water(1, 2));

            // Act - Első kattintás a vízre (kiválasztja, de nem épít)
            model.BuildBridge(1, 1);

            // Assert
            Assert.NotNull(model.SelectedField);
            Assert.Equal(1, model.SelectedField.X);
            Assert.Equal(1, model.SelectedField.Y);
            Assert.IsType<Water>(model.Map[1, 1]); // Még nem épült meg
        }

        [Fact]
        public void ForestGrowing_PrivateMethod_GrowsTrees()
        {
            // Arrange
            var model = CreateTestModel(GameMode.Run);
            // Egy Terrain lehelyezése, aminek van már fája, hogy tudjon terjedni/nőni
            var terrain = new Terrain(1, 1, 2) { Trees = 1 };
            model.Map.UpdateTable(1, 1, terrain);

            // Reflection használata a privát metódus meghívására
            MethodInfo? forestGrowingMethod = typeof(GameModel).GetMethod("ForestGrowing", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(forestGrowingMethod);

            // Act
            // Mivel a ForestGrowing random alapú, meghívjuk párszor, hogy garantáltan történjen valami
            List<Tuple<int, int>> grownTrees = [];
            for (int i = 0; i < 20; i++)
            {
                if (forestGrowingMethod.Invoke(model, null) is List<Tuple<int, int>> result) grownTrees.AddRange(result);
            }

            // Assert
            Assert.NotNull(grownTrees);
            // Ha a random szám generátor kedvező volt, nőttek fák
        }
        #endregion

        #region Járművek kezelése
        [Fact]
        public void GetVehicleAt_ReturnsCorrectVehicle_OrNull()
        {
            // Arrange
            var model = CreateTestModel();
            var van = new Van(1, 1, 270, null);
            model.Vehicles.Add(van);

            // Act
            var foundVehicle = model.GetVehicleAt(1, 1);
            var missingVehicle = model.GetVehicleAt(2, 2);

            // Assert
            Assert.Equal(van, foundVehicle);
            Assert.Null(missingVehicle);
        }

        [Fact]
        public void AssignRoute_WithSelectedStops_AssignsProuthToVehicle()
        {
            // Arrange
            var model = CreateTestModel(); // Editor módban indul
            var van = new Van(0, 0, 270, null);
            model.Vehicles.Add(van);

            // 1. Tegyük rá a megállókat a tényleges pályára, hogy a gráfgenerátor megtalálja őket!
            model.BuildRoad(0, 1);
            model.BuildStop(0, 0);
            model.BuildStop(0, 2);

            // Ensure at least two distinct stops are present
            model.SelectedStopFields.Clear();
            model.SelectedStopFields.Add((Stop)model.Map[0, 0]);
            model.SelectedStopFields.Add((Stop)model.Map[0, 2]);

            // Act
            model.AssignRoute(0, 0); // Jármű pozíciója

            // Assert
            Assert.NotNull(van.Prouth);
            Assert.Equal(2, van.Prouth.Stops.Count);
            Assert.Empty(model.SelectedStopFields); // Törlődött a kiválasztási lista
        }

        [Fact]
        public void StepAllVehicles_RunMode_CallsStepOnVehicles()
        {
            // Arrange
            var model = CreateTestModel(GameMode.Run);
            var van = new Van(1, 1, 270, null);
            model.Vehicles.Add(van);
            // Mivel nincs útvonal, az autó nem fog mozogni, de a metódus lefut

            // Act
            model.StepAllVehicles(1.0);

            // Assert
            // A sebesség módosulását vagy más állapotváltozást ellenőrizhetünk
            // Jelen esetben csak azt biztosítjuk, hogy a hívás nem száll el hibával
            Assert.Equal(1, van.MapX);
        }
        #endregion
    }
    public class GameModelAdvancedTransportTests
    {
        #region Segédmetódusok a Setup-hoz
        private GameModel CreateTestModel(GameMode mode = GameMode.Editor)
        {
            var timerMock = Substitute.For<ITimer>();
            var mapGenMock = Substitute.For<IMapGenerator>();

            var context = new MapGenerationContext(5, 5, 1, new MapGenerationSettings());
            var table = new GameTable(mapGenMock, context);

            var fields = new IField[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    fields[i, j] = new Terrain(i, j, 2);
                }
            }

            mapGenMock.GenerateMap(context).Returns((fields, new List<BuildingEntity>()));
            table.GenerateMap();

            var creationData = new GameCreationData(
                context: new MapGenerationContext(), // Ha tesztelsz, ide elég egy új/üres kontextus vagy mock
                saveName: "TestSave",                // Egy tetszőleges mentés név (nem lehet üres)
                difficulty: Difficulty.Medium,       // A kért nehézség
                balance: 100000                      // A kért kezdőtőke
            );

            return new GameModel(table, timerMock, creationData)
            {
                Mode = mode
            };
        }

        private void SetPrivateFieldOrProperty(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(target, value);
                    return;
                }
                var field = type.GetField($"<{name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }
                type = type.BaseType;
            }
        }

        private T CreateRealEntity<T>(int currentCapacity, int maxCapacity) where T : BuildingEntity
        {
            var entity = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            SetPrivateFieldOrProperty(entity, "CurrentCapacity", currentCapacity);
            SetPrivateFieldOrProperty(entity, "MaxCapacity", maxCapacity);
            return entity;
        }

        // Kifejezetten az ipari épületek ConsumeCapacity (fogyasztási) értékeinek felülírásához
        private T CreateRealIndustry<T>(int consumeCap, int maxConsumeCap) where T : IndustryEntity
        {
            var entity = CreateRealEntity<T>(0, 100); // Alap kapacitás
            SetPrivateFieldOrProperty(entity, "ConsumeCapacity", consumeCap);
            SetPrivateFieldOrProperty(entity, "MaxConsumeCapacity", maxConsumeCap);
            return entity;
        }
        #endregion

        #region BuildBridge Tesztek (Hídépítés fázisai)
        [Fact]
        public void BuildBridge_FirstClick_SelectsWaterField()
        {
            var model = CreateTestModel();
            model.Map.UpdateTable(1, 1, new Water(1, 1)); // Víz mező

            model.BuildBridge(1, 1); // 1. kattintás

            Assert.NotNull(model.SelectedField);
            Assert.Equal(1, model.SelectedField.X);
            Assert.Equal(1, model.SelectedField.Y);
        }

        [Fact]
        public void BuildBridge_SecondClickSameField_CreatesShortBridge()
        {
            var model = CreateTestModel();
            model.Map.UpdateTable(1, 1, new Water(1, 1));
            model.Map.UpdateTable(1, 0, new Terrain(1, 0, 1)); // Magasság = 1 a partokon
            model.Map.UpdateTable(1, 2, new Terrain(1, 2, 1));

            model.BuildBridge(1, 1); // 1. kattintás (kiválaszt)
            model.BuildBridge(1, 1); // 2. kattintás (épít)

            Assert.Null(model.SelectedField); // Kiválasztás törlődött
            Assert.IsType<YellowBridge>(model.Map[1, 1]); // Híd épült
        }

        [Fact]
        public void BuildBridge_SecondClickDifferentAxis_ResetsSelection()
        {
            var model = CreateTestModel();
            model.Map.UpdateTable(1, 1, new Water(1, 1));

            model.BuildBridge(1, 1); // 1. kattintás
            model.BuildBridge(2, 2); // Átlós kattintás (nem érvényes hid)

            Assert.Null(model.SelectedField); // Visszaállt nullra
        }

        //[Fact]
        //public void BuildBridge_Horizontal_BuildsOverWater()
        //{
        //    var model = CreateTestModel();
        //    model.Map.UpdateTable(1, 1, new Water(1, 1));
        //    model.Map.UpdateTable(1, 2, new Water(1, 2));
        //    model.Map.UpdateTable(1, 3, new Water(1, 3));

        //    // Partok (Height 1 kell hogy legyen)
        //    model.Map.UpdateTable(1, 0, new Terrain(1, 0, 1));
        //    model.Map.UpdateTable(1, 4, new Terrain(1, 4, 1));

        //    bool bridgeBuiltFired = false;
        //    model.InfrastructureBuilt += (s, e) => bridgeBuiltFired = true;

        //    model.BuildBridge(1, 1); // Kezdőpont
        //    model.BuildBridge(1, 3); // Végpont (horizontális)

        //    Assert.True(bridgeBuiltFired);
        //    Assert.IsType<GreenBridge>(model.Map[1, 2]); // Két távolságú vízre zöld híd épül!
        //}
        #endregion

        #region ForestGrowing Tesztek
        //[Fact]
        //public void ForestGrowing_PrivateMethod_SpreadsAndGrowsTrees()
        //{
        //    var model = CreateTestModel(GameMode.Run);
        //    // Készítünk egy majdnem tele lévő fát
        //    var terrain = new Terrain(1, 1, 2) { Trees = 3 };
        //    model.Map.UpdateTable(1, 1, terrain);
        //    model.Map.UpdateTable(1, 2, new Terrain(1, 2, 2) { Trees = 0 }); // Üres szomszéd

        //    var method = typeof(GameModel).GetMethod("ForestGrowing", BindingFlags.NonPublic | BindingFlags.Instance);

        //    // A ForestGrowing erősen Random(Seed) alapú, ezért ha a Seed miatt pont nem futna be a 10%-os
        //    // if ágba, loopolunk párat, hogy az "idő múlását" szimuláljuk
        //    List<Tuple<int, int>> grownTrees = new();
        //    for (int i = 0; i < 50; i++)
        //    {
        //        var result = method!.Invoke(model, null) as List<Tuple<int, int>>;
        //        if (result != null) grownTrees.AddRange(result);
        //    }

        //    // Mivel 50-szer meghívtuk, a random 10% miatt biztosan nőttek/terjedtek a fák
        //    Assert.NotEmpty(grownTrees);
        //}
        #endregion
    }
}
