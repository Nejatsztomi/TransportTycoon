using System.Reflection;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using TransportTycoon.Model.Graph;
using GraphModel = TransportTycoon.Model.Graph.Graph;

namespace TransportTycoon.Test.Model
{
    public class VehicleTests
    {
        private sealed class TestMapGenerator : IMapGenerator
        {
            public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context)
            {
                return (new IField[context.Width, context.Height], []);
            }
        }

        private sealed class TestPathFinder : IPathFinder
        {
            private readonly List<Edge>? _result;

            public TestPathFinder(List<Edge>? result)
            {
                _result = result;
            }

            public List<Edge>? FindPath(Node startNode, Node endNode) => _result;
        }

        private sealed class SequencedPathFinder : IPathFinder
        {
            private readonly Queue<List<Edge>?> _results;

            public SequencedPathFinder(params List<Edge>?[] results)
            {
                _results = new Queue<List<Edge>?>(results);
            }

            public int CallCount { get; private set; }

            public List<Edge>? FindPath(Node startNode, Node endNode)
            {
                CallCount++;
                return _results.Count > 0 ? _results.Dequeue() : null;
            }
        }

        private sealed class TestVehicle : Vehicle
        {
            public TestVehicle(List<LoadType>? acceptedGoods, int maxCapacity = 0) : base(0, 0, 0, null)
            {
                AcceptedGoods = acceptedGoods;
                MaxCapacity = maxCapacity;
            }

            public void SetCurrentRouteValue(List<Edge>? route) => CurrentRoute = route;

            public void SetIsLostValue(bool isLost)
            {
                typeof(Vehicle).GetProperty(nameof(IsLost), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(this, isLost);
            }

            public void SetPrivateField<T>(string fieldName, T value)
            {
                typeof(Vehicle).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, value);
            }

            public T GetPrivateField<T>(string fieldName)
            {
                return (T)typeof(Vehicle).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(this)!;
            }
        }

        private static Edge CreateEdge(params IField[] roads)
        {
            return new Edge(new Node(0, 0, typeof(Stop)), new Node(1, 1, typeof(Stop)), roads, 1);
        }

        private static GhostNodeInjector CreateInjector(IField[,] table, Dictionary<Node, List<Edge>>? adjacencyList = null)
        {
            var context = new MapGenerationContext(table.GetLength(0), table.GetLength(1), 0, new MapGenerationSettings());
            var gameTable = new GameTable(new TestMapGenerator(), context);

            for (int x = 0; x < table.GetLength(0); x++)
            {
                for (int y = 0; y < table.GetLength(1); y++)
                {
                    gameTable.UpdateTable(x, y, table[x, y]);
                }
            }

            return new GhostNodeInjector(new GraphModel(adjacencyList ?? []), new PathTracer(gameTable));
        }

        private static GhostNodeInjector CreateInjectorWithStopNode(int x, int y, int width = 3, int height = 3)
        {
            var table = new IField[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    table[i, j] = new Water(i, j);
                }
            }

            table[x, y] = new Stop(x, y, 1);

            return CreateInjector(table, new Dictionary<Node, List<Edge>>
            {
                [new Node(x, y, typeof(Stop))] = []
            });
        }

        #region Konstruktor és Alaptulajdonságok Tesztjei
        [Fact]
        public void SmallBus_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var bus = new SmallBus(10, 20, 270, null); // Up = 270

            // Assert
            Assert.Equal(10, bus.X);
            Assert.Equal(20, bus.Y);
            Assert.Equal(270, bus.Angle);
            Assert.Equal(1, bus.TopSpeed);
            Assert.Equal(100, bus.MaxCapacity);
            Assert.Equal(100, bus.Price);
            Assert.Equal(100, bus.Maintenance);
            Assert.Equal(VehicleType.SmallBus, bus.Type);
            Assert.Single(bus.AcceptedGoods!);
            Assert.Contains(LoadType.People, bus.AcceptedGoods!);
            Assert.Equal(bus.TopSpeed, bus.CurrentSpeed);
        }

        [Fact]
        public void Truck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var truck = new Truck(5, 5, 0, null); // Right = 0

            // Assert
            Assert.Equal(1.5, truck.TopSpeed);
            Assert.Equal(100, truck.MaxCapacity);
            Assert.Equal(VehicleType.Truck, truck.Type);
            Assert.Equal(5, truck.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Wood, truck.AcceptedGoods);
            Assert.Contains(LoadType.Flour, truck.AcceptedGoods);
        }

        [Fact]
        public void LiquidTruck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var liquidTruck = new LiquidTruck(0, 0, 90, null); // Down = 90

            // Assert
            Assert.Equal(0.9, liquidTruck.TopSpeed);
            Assert.Equal(VehicleType.LiquidTruck, liquidTruck.Type);
            Assert.Single(liquidTruck.AcceptedGoods!);
            Assert.Contains(LoadType.Oil, liquidTruck.AcceptedGoods!);
        }
        #endregion

        #region Kapacitás és Rakomány Tesztek
        [Fact]
        public void SetCurrentCapacity_ValidAmount_UpdatesCapacity()
        {
            // Arrange
            var van = new Van(0, 0, 270, null); // Up = 270

            // Act
            van.SetCurrentCapacity(50);

            // Assert
            Assert.Equal(50, van.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentCapacity_ExceedsMaxCapacity_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, 270, null); // Up = 270
            van.SetCurrentCapacity(50); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(150); // Érvénytelen érték, mert MaxCapacity = 100

            // Assert
            Assert.Equal(50, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentCapacity_BelowZero_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, 270, null); // Up = 270
            van.SetCurrentCapacity(50); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(-20); // Érvénytelen (negatív) érték

            // Assert
            Assert.Equal(50, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentLoad_UpdatesLoadCorrectly()
        {
            // Arrange
            var pickup = new Pickup(0, 0, 270, null); // Up = 270
            var wood = new Wood();

            // Act
            pickup.SetCurrentLoad(wood);

            // Assert
            Assert.Equal(wood, pickup.CurrentLoad);
        }

        [Fact]
        public void SetCurrentLoad_WhenAcceptedGoodsIsNull_DoesNotSetLoad()
        {
            // Arrange
            var vehicle = new TestVehicle(null, 100);
            var wood = new Wood();
            vehicle.SetCurrentCapacity(25);

            // Act
            vehicle.SetCurrentLoad(wood);

            // Assert
            Assert.Null(vehicle.CurrentLoad);
            Assert.Equal(25, vehicle.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentLoad_WhenAcceptedGoodsDoesNotContainLoad_DoesNotSetLoad()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People], 100);
            var wood = new Wood();
            vehicle.SetCurrentCapacity(25);

            // Act
            vehicle.SetCurrentLoad(wood);

            // Assert
            Assert.Null(vehicle.CurrentLoad);
            Assert.Equal(25, vehicle.CurrentCapacity);
        }
        #endregion

        #region Indulás Tesztek
        [Fact]
        public void StartDrivingFromStopToStop_WhenVehicleIsLost_DoesNothing()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeIdx", 7);
            vehicle.SetPrivateField("_currentTileIdx", 3);
            vehicle.SetPrivateField("_tileProgress", 0.75);
            vehicle.SetPrivateField("_lerpX", 12.5);
            vehicle.SetPrivateField("_lerpY", 34.5);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField>());
            vehicle.SetIsLostValue(true);

            // Act
            vehicle.StartDrivingFromStopToStop();

            // Assert
            Assert.True(vehicle.IsLost);
            Assert.Equal(7, vehicle.GetPrivateField<int>("_currentEdgeIdx"));
            Assert.Equal(3, vehicle.GetPrivateField<int>("_currentTileIdx"));
            Assert.Equal(0.75, vehicle.GetPrivateField<double>("_tileProgress"));
            Assert.Equal(12.5, vehicle.GetPrivateField<double>("_lerpX"));
            Assert.Equal(34.5, vehicle.GetPrivateField<double>("_lerpY"));
        }

        [Fact]
        public void StartDrivingFromStopToStop_WhenCurrentRouteIsNull_ResetsRouteState()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeIdx", 7);
            vehicle.SetPrivateField("_currentTileIdx", 3);
            vehicle.SetPrivateField("_tileProgress", 0.75);
            vehicle.SetPrivateField("_lerpX", 12.5);
            vehicle.SetPrivateField("_lerpY", 34.5);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField>());
            vehicle.SetCurrentRouteValue(null);

            // Act
            vehicle.StartDrivingFromStopToStop();

            // Assert
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentEdgeIdx"));
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentTileIdx"));
            Assert.Equal(0.0, vehicle.GetPrivateField<double>("_tileProgress"));
            Assert.Equal(vehicle.X, vehicle.GetPrivateField<double>("_lerpX"));
            Assert.Equal(vehicle.Y, vehicle.GetPrivateField<double>("_lerpY"));
            Assert.Empty(vehicle.GetPrivateField<List<IField>>("_currentEdgeTiles"));
        }

        [Fact]
        public void StartDrivingFromStopToStop_WhenCurrentRouteIsEmpty_ResetsRouteState()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeIdx", 7);
            vehicle.SetPrivateField("_currentTileIdx", 3);
            vehicle.SetPrivateField("_tileProgress", 0.75);
            vehicle.SetPrivateField("_lerpX", 12.5);
            vehicle.SetPrivateField("_lerpY", 34.5);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField>());
            vehicle.SetCurrentRouteValue([]);

            // Act
            vehicle.StartDrivingFromStopToStop();

            // Assert
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentEdgeIdx"));
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentTileIdx"));
            Assert.Equal(0.0, vehicle.GetPrivateField<double>("_tileProgress"));
            Assert.Equal(vehicle.X, vehicle.GetPrivateField<double>("_lerpX"));
            Assert.Equal(vehicle.Y, vehicle.GetPrivateField<double>("_lerpY"));
            Assert.Empty(vehicle.GetPrivateField<List<IField>>("_currentEdgeTiles"));
        }

        [Fact]
        public void GetNextRoute_WhenPathFinderIsNull_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 1, typeof(Stop))])
            };

            // Act
            var route = vehicle.GetNextRoute();

            // Assert
            Assert.Null(route);
        }

        [Fact]
        public void GetNextRoute_WhenProuthIsNull_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                PathFinder = new TestPathFinder([])
            };

            // Act
            var route = vehicle.GetNextRoute();

            // Assert
            Assert.Null(route);
        }

        [Fact]
        public void GetNextRoute_WhenProuthHasLessThanTwoStops_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop))]),
                PathFinder = new TestPathFinder([])
            };

            // Act
            var route = vehicle.GetNextRoute();

            // Assert
            Assert.Null(route);
        }

        [Fact]
        public void GetNextRoute_WhenNoPathIsFound_MarksVehicleLost()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 1, typeof(Stop))]),
                PathFinder = new TestPathFinder(null)
            };

            // Act
            var route = vehicle.GetNextRoute();

            // Assert
            Assert.Null(route);
            Assert.True(vehicle.IsLost);
        }
        #endregion

        #region Újraszámolt Útvonal Tesztek
        [Fact]
        public void RecalculateRoute_WhenPathFinderIsNull_DoesNothing()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 3, typeof(Stop))])
            };
            vehicle.SetCurrentRouteValue([CreateEdge(new Road(1, 1, RoadType.Vertical, 1))]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Road(1, 1, RoadType.Vertical, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            var injector = CreateInjector(new IField[5, 5]);
            var currentRoute = vehicle.CurrentRoute;
            var currentTileIdx = vehicle.GetPrivateField<int>("_currentTileIdx");

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.False(vehicle.IsLost);
            Assert.Same(currentRoute, vehicle.CurrentRoute);
            Assert.Equal(currentTileIdx, vehicle.GetPrivateField<int>("_currentTileIdx"));
        }

        [Fact]
        public void RecalculateRoute_WhenStopPairCannotBeDetermined_DoesNothing()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                PathFinder = new TestPathFinder([])
            };
            var injector = CreateInjector(new IField[5, 5]);
            vehicle.Prouth = new Prouth([new Node(0, 0, typeof(Stop))]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Road(1, 1, RoadType.Vertical, 1) });
            var currentRoute = vehicle.CurrentRoute;

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.False(vehicle.IsLost);
            Assert.Same(currentRoute, vehicle.CurrentRoute);
        }

        [Fact]
        public void RecalculateRoute_WhenGhostNodeCannotBeInjected_MarksVehicleLost()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 3, typeof(Stop))]),
                PathFinder = new TestPathFinder([])
            };
            vehicle.SetPrivateField<List<IField>?>("_currentEdgeTiles", null);
            var injector = CreateInjector(new IField[3, 3]);

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.True(vehicle.IsLost);
            Assert.Null(vehicle.CurrentRoute);
        }

        [Fact]
        public void RecalculateRoute_WhenPathFinderReturnsNull_MarksVehicleLost()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 3, typeof(Stop))]),
                PathFinder = new TestPathFinder(null)
            };
            vehicle.SetCurrentRouteValue([CreateEdge(new Road(1, 1, RoadType.Vertical, 1))]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Road(1, 1, RoadType.Vertical, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            var injector = CreateInjectorWithStopNode(1, 1);

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.True(vehicle.IsLost);
            Assert.Null(vehicle.CurrentRoute);
        }

        [Fact]
        public void RecalculateRoute_WhenPathFinderReturnsEmptyRoute_UsesNextRoute()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(1, 3, typeof(Stop)), new Node(2, 3, typeof(Stop))])
            };
            vehicle.SetCurrentRouteValue([CreateEdge(new Stop(1, 1, 1), new Road(1, 2, RoadType.Vertical, 1), new Stop(1, 3, 1))]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField>
            {
                new Stop(1, 1, 1),
                new Road(1, 2, RoadType.Vertical, 1),
                new Stop(1, 3, 1)
            });
            vehicle.SetPrivateField("_currentTileIdx", 0);

            var pathFinder = new SequencedPathFinder(
                [],
                [CreateEdge(new Road(1, 1, RoadType.Vertical, 1), new Road(1, 2, RoadType.Vertical, 1), new Stop(1, 3, 1))]
            );

            vehicle.PathFinder = pathFinder;
            var injector = CreateInjectorWithStopNode(1, 1);

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.Equal(2, pathFinder.CallCount);
            Assert.NotNull(vehicle.CurrentRoute);
            Assert.NotEmpty(vehicle.CurrentRoute!);
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentEdgeIdx"));
            Assert.NotNull(vehicle.GetPrivateField<List<IField>>("_currentEdgeTiles"));
        }

        [Fact]
        public void RecalculateRoute_WhenNewRouteExists_UpdatesCurrentRouteAndTileState()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People])
            {
                Prouth = new Prouth([new Node(0, 0, typeof(Stop)), new Node(1, 3, typeof(Stop))])
            };
            vehicle.SetCurrentRouteValue(null);

            var route = new List<Edge>
            {
                CreateEdge(
                    new Stop(0, 0, 1),
                    new Road(0, 1, RoadType.Vertical, 1),
                    new Road(0, 2, RoadType.Vertical, 1),
                    new Stop(1, 3, 1)
                )
            };

            vehicle.PathFinder = new TestPathFinder(route);
            var injector = CreateInjectorWithStopNode(0, 0);

            // Act
            vehicle.RecalculateRoute(injector);

            // Assert
            Assert.False(vehicle.IsLost);
            Assert.Equal(route, vehicle.CurrentRoute);
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentEdgeIdx"));
            Assert.Equal(0, vehicle.GetPrivateField<int>("_currentTileIdx"));
            Assert.Equal(vehicle.X, vehicle.GetPrivateField<double>("_lerpX"));
            Assert.Equal(vehicle.Y, vehicle.GetPrivateField<double>("_lerpY"));
        }
        #endregion

        #region Lane Index Tesztek
        [Theory]
        [InlineData(-1, 0, 3)] // Left
        [InlineData(1, 0, 1)]  // Right
        [InlineData(0, -1, 0)] // Up
        [InlineData(0, 1, 2)]  // Down
        public void GetLaneIdx_ReturnsExpectedLaneIndexForEachDirection(int targetX, int targetY, int expectedLaneIdx)
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Stop(targetX, targetY, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            vehicle.SetPrivateField("_lerpX", 0.0);
            vehicle.SetPrivateField("_lerpY", 0.0);

            // Act
            var laneIdx = vehicle.GetLaneIdx();

            // Assert
            Assert.Equal(expectedLaneIdx, laneIdx);
        }
        #endregion

        #region Következő Csempe Tesztek
        [Fact]
        public void GetNextTileCoordinates_WhenCurrentEdgeTilesIsNull_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField<List<IField>?>("_currentEdgeTiles", null);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Null(nextTile);
        }

        [Fact]
        public void GetNextTileCoordinates_WhenVehicleIsLost_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Stop(0, 0, 1) });
            vehicle.SetIsLostValue(true);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Null(nextTile);
        }

        [Fact]
        public void GetNextTileCoordinates_WhenNextTileExists_ReturnsNextTileCoordinates()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField>
            {
                new Stop(0, 0, 1),
                new Stop(2, 3, 1)
            });
            vehicle.SetPrivateField("_currentTileIdx", 0);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Equal((2, 3), nextTile);
        }

        [Fact]
        public void GetNextTileCoordinates_WhenEndOfEdgeAndNextEdgeExists_ReturnsFirstTileOfNextEdge()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            var firstEdge = CreateEdge(new Stop(0, 0, 1));
            var secondEdge = CreateEdge(new Stop(4, 5, 1), new Stop(6, 7, 1));

            vehicle.SetCurrentRouteValue([firstEdge, secondEdge]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Stop(0, 0, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            vehicle.SetPrivateField("_currentEdgeIdx", 0);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Equal((4, 5), nextTile);
        }

        [Fact]
        public void GetNextTileCoordinates_WhenEndOfEdgeAndCurrentRouteIsNull_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Stop(0, 0, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            vehicle.SetPrivateField("_currentEdgeIdx", 0);
            vehicle.SetCurrentRouteValue(null);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Null(nextTile);
        }

        [Fact]
        public void GetNextTileCoordinates_WhenNextEdgeHasNoRoads_ReturnsNull()
        {
            // Arrange
            var vehicle = new TestVehicle([LoadType.People]);
            var firstEdge = CreateEdge(new Stop(0, 0, 1));
            var secondEdge = CreateEdge();

            vehicle.SetCurrentRouteValue([firstEdge, secondEdge]);
            vehicle.SetPrivateField("_currentEdgeTiles", new List<IField> { new Stop(0, 0, 1) });
            vehicle.SetPrivateField("_currentTileIdx", 0);
            vehicle.SetPrivateField("_currentEdgeIdx", 0);

            // Act
            var nextTile = vehicle.GetNextTileCoordinates();

            // Assert
            Assert.Null(nextTile);
        }
        #endregion

        #region Sebesség Tesztek
        [Fact]
        public void ChangeCurrentSpeed_ValidSpeed_UpdatesSpeed()
        {
            // Arrange
            var bus = new BigBus(0, 0, 270, null); // Up = 270
            double validSpeed = bus.TopSpeed / 2;

            // Act
            bus.ChangeCurrentSpeed(validSpeed);

            // Assert
            Assert.Equal(validSpeed, bus.CurrentSpeed);
        }

        [Fact]
        public void ChangeCurrentSpeed_ExceedsTopSpeed_CapsAtTopSpeed()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 270, null); // Up = 270

            // Act
            bus.ChangeCurrentSpeed(5.0);

            // Assert
            Assert.Equal(1.0, bus.CurrentSpeed);
        }
        [Fact]
        public void ChangeCurrentSpeed_BelowZero_DoesNotChangeSpeed()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 270, null); // Up = 270

            // Act
            bus.ChangeCurrentSpeed(-1.0);

            // Assert
            Assert.Equal(0.0, bus.CurrentSpeed); // Math.Clamp sets to 0.0
        }
        #endregion

        #region További Tesztjei
        [Fact]
        public void Vehicle_Ids_AreUnique()
        {
            // Arrange & Act
            var vehicle1 = new SmallBus(0, 0, 0, null);
            var vehicle2 = new Truck(0, 0, 0, null);

            // Assert
            Assert.NotEqual(vehicle1.Id, vehicle2.Id);
        }

        [Fact]
        public void SetCurrentCapacity_ToZero_ClearsCurrentLoad()
        {
            // Arrange
            var van = new Van(0, 0, 0, null);
            var wood = new Wood();
            van.SetCurrentLoad(wood);
            van.SetCurrentCapacity(50);

            // Act
            van.SetCurrentCapacity(0);

            // Assert
            Assert.Equal(0, van.CurrentCapacity);
            Assert.Null(van.CurrentLoad);
        }

        [Fact]
        public void SetCurrentLoad_ToNull_ClearsCurrentCapacity()
        {
            // Arrange
            var pickup = new Pickup(0, 0, 0, null);
            pickup.SetCurrentCapacity(50);

            // Act
            pickup.SetCurrentLoad(null);

            // Assert
            Assert.Null(pickup.CurrentLoad);
            Assert.Equal(0, pickup.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentLoad_InvalidLoadType_DoesNotSetLoad()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 0, null);
            var wood = new Wood(); // SmallBus only accepts People

            // Act
            bus.SetCurrentLoad(wood);

            // Assert
            Assert.Null(bus.CurrentLoad);
        }

        [Fact]
        public void SetCurrentLoad_ValidLoadType_SetsLoadWithoutChangingCapacity()
        {
            // Arrange
            var truck = new Truck(0, 0, 0, null);
            var wood = new Wood();
            truck.SetCurrentCapacity(50);

            // Act
            truck.SetCurrentLoad(wood);

            // Assert
            Assert.Equal(wood, truck.CurrentLoad);
            Assert.Equal(50, truck.CurrentCapacity);
        }

        [Fact]
        public void MapX_MapY_RoundCoordinatesCorrectly()
        {
            // Arrange
            var van = new Van(1, 3, 0, null);

            // Assert
            Assert.Equal(1, van.MapX);
            Assert.Equal(3, van.MapY);
        }

        [Fact]
        public void Vehicle_IsLost_InitiallyFalse()
        {
            // Arrange
            var vehicle = new Truck(0, 0, 0, null);

            // Assert
            Assert.False(vehicle.IsLost);
        }

        [Fact]
        public void Van_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var van = new Van(5, 10, 180, null); // Left = 180

            // Assert
            Assert.Equal(5, van.X);
            Assert.Equal(10, van.Y);
            Assert.Equal(180, van.Angle);
            Assert.Equal(0.9, van.TopSpeed);
            Assert.Equal(100, van.MaxCapacity);
            Assert.Equal(100, van.Price);
            Assert.Equal(100, van.Maintenance);
            Assert.Equal(VehicleType.Van, van.Type);
            Assert.Equal(5, van.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Wood, van.AcceptedGoods);
            Assert.Equal(van.TopSpeed, van.CurrentSpeed);
        }

        [Fact]
        public void Pickup_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var pickup = new Pickup(0, 0, 90, null); // Down = 90

            // Assert
            Assert.Equal(0.9, pickup.TopSpeed);
            Assert.Equal(100, pickup.MaxCapacity);
            Assert.Equal(VehicleType.Pickup, pickup.Type);
            Assert.Equal(5, pickup.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Flour, pickup.AcceptedGoods);
        }

        [Fact]
        public void BigBus_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var bus = new BigBus(15, 25, 0, null); // Right = 0

            // Assert
            Assert.Equal(15, bus.X);
            Assert.Equal(25, bus.Y);
            Assert.Equal(0, bus.Angle);
            Assert.Equal(1, bus.TopSpeed);
            Assert.Equal(100, bus.MaxCapacity);
            Assert.Equal(100, bus.Price);
            Assert.Equal(100, bus.Maintenance);
            Assert.Equal(VehicleType.BigBus, bus.Type);
            Assert.Single(bus.AcceptedGoods!);
            Assert.Contains(LoadType.People, bus.AcceptedGoods!);
            Assert.Equal(bus.TopSpeed, bus.CurrentSpeed);
        }
        #endregion
    }
}
