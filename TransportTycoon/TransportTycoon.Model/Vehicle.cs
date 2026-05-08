using System.Diagnostics;
using System.Runtime.CompilerServices;
using TransportTycoon.MapData;
using TransportTycoon.Model.Graph;

namespace TransportTycoon.Model
{
    /// <summary>
    /// An enum to represent vehicle types.
    /// </summary>
    public enum VehicleType : byte
    {
        Van = 0,
        Pickup = 1,
        Truck = 2,
        LiquidTruck = 3,
        SmallBus = 4,
        BigBus = 5,
    }

    /// <summary>
    /// Represents an abstract base class for vehicles that travel along routes, manage loads, and interact with a
    /// pathfinding system.
    /// </summary>
    /// <remarks>
    /// The <see cref="Vehicle"/> class provides core functionality for movement, routing, and load management in a
    /// transportation or simulation context. It exposes properties and methods for controlling position, speed, route
    /// assignment, and load handling. Derived classes should implement specific vehicle behaviors as needed. Vehicles
    /// can become lost if they are unable to find a valid route to their destination, in which case intervention may be
    /// required. Thread safety is not guaranteed; external synchronization may be necessary if accessed from multiple
    /// threads.
    /// </remarks>
    public abstract class Vehicle
    {
        #region Private static fields
        private static UInt64 _globalIdCounter = 0;
        #endregion

        #region Private fields
        /// <summary>
        /// The prouth's stop index.
        /// </summary>
        private int _currentStopIdx = 0;
        /// <summary>
        /// The current route's edge index.
        /// </summary>
        private int _currentEdgeIdx = 0;
        /// <summary>
        /// The current edge's tile index.
        /// </summary>
        private int _currentTileIdx = 0;
        /// <summary>
        /// The current tile's progress.
        /// </summary>
        private double _tileProgress = 0.0;
        /// <summary>
        /// The current edge's tiles.
        /// </summary>
        private List<IField>? _currentEdgeTiles = null;
        private double _lerpX;
        private double _lerpY;
        #endregion

        #region Properties
        #region Readonly
        /// <summary>
        /// The unique identifier for the vehicle, assigned automatically upon creation.
        /// </summary>
        public UInt64 Id { get; private set; }

        /// <summary>
        /// The maximum speed that can be achieved.
        /// </summary>
        public double TopSpeed { get; protected init; }

        /// <summary>
        /// Gets the maximum capacity allowed for the vehicle.
        /// </summary>
        public int MaxCapacity { get; protected init; }

        /// <summary>
        /// Gets the type of the vehicle.
        /// </summary>
        public VehicleType Type { get; protected init; }

        /// <summary>
        /// Gets the price associated with the vehicle.
        /// </summary>
        public int Price { get; protected init; }

        /// <summary>
        /// Gets the maintenance cost associated with the vehicle.
        /// </summary>
        public int Maintenance { get; protected init; }
        #endregion

        /// <summary>
        /// The current speed of the vehicle.
        /// </summary>
        public double CurrentSpeed { get; protected set; }

        /// <summary>
        /// Gets the current load information, if available.
        /// </summary>
        public Load? CurrentLoad { get; protected set; }

        /// <summary>
        /// Gets the current capacity value.
        /// </summary>
        public int CurrentCapacity { get; protected set; }

        /// <summary>
        /// Gets the current <see cref="Prouth"/> value associated with this vehicle.
        /// </summary>
        public Prouth? Prouth { get; set; }

        /// <summary>
        /// The current route of the vehicle, represented as a list of <see cref="Edge"/> objects.
        /// The route may be <see langword="null"/> if the vehicle does not have a current route assigned.
        /// The vehicle will be repeating this route, if not given a new one.
        /// </summary>
        public List<Edge>? CurrentRoute { get; protected set; }

        #region Position and movement
        /// <summary>
        /// The vehicle's current X coordinate on the map.
        /// </summary>
        public double X { get; protected set; }

        /// <summary>
        /// The vehicle's current Y coordinate on the map.
        /// </summary>
        public double Y { get; protected set; }

        /// <summary>
        /// The angle of the vehicle in degrees, where <see langword="0"/> degrees represents facing right (positive X direction), and angles increase counterclockwise (90 degrees is up, 180 degrees is left, and 270 degrees is down).
        /// </summary>
        public double Angle { get; protected set; }

        /// <summary>
        /// How fast should the vehicle turn in degrees per second.
        /// </summary>
        public double TurnSpeed { get; protected set; } = 360.0;

        /// <summary>
        /// Gets the rounded X coordinate of the vehicle on the map.
        /// </summary>
        public int MapX => (int)Math.Round(X);

        /// <summary>
        /// Gets the rounded Y coordinate of the vehicle on the map.
        /// </summary>
        public int MapY => (int)Math.Round(Y);

        /// <summary>
        /// Gets or sets the X-coordinate of the last map position accessed.
        /// </summary>
        /// <remarks>
        /// The value -1 indicates that no map position has been accessed yet.
        /// This property can be used to track the most recently accessed map coordinates for optimization or state management purposes.
        /// </remarks>
        public int LastMapX { get; set; } = -1;

        /// <summary>
        /// Gets or sets the Y-coordinate of the last map position accessed.
        /// </summary>
        /// <remarks>
        /// The value -1 indicates that no map position has been accessed yet.
        /// This property can be used to track the most recently accessed map coordinates for optimization or state management purposes.
        /// </remarks>
        public int LastMapY { get; set; } = -1;

        /// <summary>
        /// Gets or sets the index of the last lane used or selected.
        /// </summary>
        /// <remarks>
        /// A value of -1 indicates that no lane has been selected or used. This property can be
        /// used to track the most recently accessed lane in scenarios where multiple lanes are available.
        /// </remarks>
        public int LastLaneIdx { get; set; } = -1;

        /// <summary>
        /// Gets the list of load types that are accepted.
        /// </summary>
        public List<LoadType>? AcceptedGoods { get; protected set; } = [];

        /// <summary>
        /// <see langword="true"/> if the vehicle is lost, meaning it cannot find a valid route to its destination and is effectively stuck.
        /// This can occur when the vehicle's current position is not connected to the rest of the map, or when all possible routes to the destination are blocked or inaccessible.
        /// When a vehicle is marked as lost, it may require intervention to be moved back onto a valid path or to be removed from the game if it cannot be recovered.
        /// </summary>
        public bool IsLost { get; private set; } = false;

        /// <summary>
        /// Gets the pathfinding service used to calculate routes or paths within the application.
        /// </summary>
        /// <remarks>
        /// The returned pathfinder may be <see langword="null"/> if no pathfinding service is configured.
        /// Consumers should check for <see langword="null"/> before invoking pathfinding operations.
        /// </remarks>
        public IPathFinder? PathFinder { get; set; }
        #endregion
        #endregion

        #region Protected constructor
        /// <summary>
        /// The base constructor for the <see cref="Vehicle"/> class, which initializes the vehicle's unique identifier and sets default values for its properties.
        /// </summary>
        protected Vehicle()
        {
            Id = _globalIdCounter++;
            CurrentLoad = null;
            CurrentCapacity = 0;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Vehicle"/> class with the specified initial position, angle, and route.
        /// </summary>
        /// <param name="x">The initial X coordinate of the vehicle.</param>
        /// <param name="y">The initial Y coordinate of the vehicle.</param>
        /// <param name="angle">The initial angle of the vehicle in degrees.</param>
        /// <param name="route">The initial route of the vehicle.</param>
        protected Vehicle(int x, int y, double angle, Prouth? route) : this()
        {
            X = x;
            Y = y;
            Angle = angle;
            Prouth = route;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Advances the entity along its current route by moving it one step toward the next target tile, updating its position and direction as needed.
        /// </summary>
        /// <remarks>
        /// This method performs a single movement operation for the entity.
        /// </remarks>
        public void Step(double deltaTime)
        {
            if (IsLost || Prouth is null || _currentEdgeTiles is null) return;

            double distanceToTravel = CurrentSpeed * deltaTime;
            _tileProgress += distanceToTravel;

            if (_tileProgress >= 1.0)
            {
                _tileProgress -= 1.0;

                _lerpX = _currentEdgeTiles[_currentTileIdx].X;
                _lerpY = _currentEdgeTiles[_currentTileIdx].Y;

                X = _lerpX;
                Y = _lerpY;

                // update indexes for next tile
                AdvanceToNextTile();

                if (IsLost || _currentEdgeTiles is null) return;
            }

            UpdateContinuousPosition(deltaTime);
        }

        /// <summary>
        /// Sets the current capacity of the vehicle, if the given quantity is between 0 and the maximum capacity of the vehicle. If the quantity is set to 0, the current load is also set to null.
        /// </summary>
        /// <param name="quantity"></param>
        public void SetCurrentCapacity(int quantity)
        {
            if (quantity >= 0 && quantity <= MaxCapacity) CurrentCapacity = quantity;

            if (CurrentCapacity == 0) CurrentLoad = null;
        }

        /// <summary>
        /// Sets the current load to the specified value if it is valid and accepted by the vehicle.
        /// </summary>
        /// <remarks>If the specified load is accepted by the vehicle, the current load is updated.
        /// Setting the current load to null also resets the current capacity to zero.</remarks>
        /// <param name="load">The load to assign as the current load. Specify null to clear the current load.</param>
        public void SetCurrentLoad(Load? load)
        {
            if (load is null || AcceptedGoods is not null && AcceptedGoods.Contains(load.LoadType))
            {
                CurrentLoad = load;
                if (CurrentLoad is null) CurrentCapacity = 0;
            }
        }

        /// <summary>
        /// Initializes the vehicle's driving state and begins movement from the current stop to the next stop along the assigned route.
        /// </summary>
        /// <remarks>
        /// If the vehicle is in a lost state, this method does not perform any action.
        /// The method resets the vehicle's progress and prepares it to follow the current route, if one available.
        /// </remarks>
        public void StartDrivingFromStopToStop()
        {
            if (IsLost) return;

            Debug.WriteLine($"Vehicle {Id} is starting to drive...");
            _currentEdgeIdx = 0;
            _currentTileIdx = 0;
            _tileProgress = 0.0;

            _lerpX = X;
            _lerpY = Y;

            if (CurrentRoute?.Count > 0)
            {
                _currentEdgeTiles = [.. CurrentRoute[0].Roads];
                Debug.WriteLine($"Vehicle {Id} route: {string.Join(" -> ", _currentEdgeTiles.Select(e => $"({e.X}, {e.Y})"))}");
            }
        }

        /// <summary>
        /// Changes the current speed of the vehicle to a value between 0 and <see cref="TopSpeed"/>.
        /// </summary>
        /// <param name="speed">The desired speed to set for the vehicle.</param>
        public void ChangeCurrentSpeed(double speed)
        {
            CurrentSpeed = Math.Clamp(speed, 0.0, TopSpeed);
        }

        /// <summary>
        /// Gets the next route between two <see cref="Stop"/> tiles.
        /// If it's last stop in the route, it loops over.
        /// </summary>
        public List<Edge>? GetNextRoute()
        {
            if (PathFinder is null || Prouth is null) return null;

            Debug.WriteLine($"Vehicle {Id} is getting the next route...");
            var stopPair = GetNextStopNodePair();
            _currentStopIdx = (_currentStopIdx + 1) % Prouth.Stops.Count;
            if (stopPair is (Node start, Node end))
            {
                var currentRoute = PathFinder.FindPath(start, end);
                Debug.WriteLineIf(currentRoute is null, $"Vehicle {Id} could not find a route from ({start.X}, {start.Y}) to ({end.X}, {end.Y}).");
                Debug.WriteLineIf(currentRoute is not null, $"Vehicle {Id} found a route from ({start.X}, {start.Y}) to ({end.X}, {end.Y}) with {currentRoute!.Count} edges.");
                Debug.WriteLineIf(currentRoute is not null, $"Vehicle {Id} route: {string.Join(" -> ", currentRoute!.Select(e => $"({e.StartNode.X}, {e.StartNode.Y}) to ({e.EndNode.X}, {e.EndNode.Y})"))}");

                if (currentRoute is null) IsLost = true;

                return currentRoute;
            }
            return null;
        }

        /// <summary>
        /// A method to set the vehicle's <see cref="Prouth"/>, <see cref="PathFinder"/>, and <see cref="GhostNodeInjector"/>.
        /// </summary>
        /// <remarks>
        /// It always tries to found a route to the first stop of the new <see cref="Prouth"/>. If it fails, the vehicle is marked as lost.
        /// It uses <see cref="RecalculateRoute(GhostNodeInjector)"/> to determine the route to the first stop.
        /// It is "lazy" since if the vehicle correctly stand on the <see cref="Prouth"/>'s first stop it returns early.
        /// </remarks>
        /// <param name="prouth">The <see cref="Prouth"/> to assign to the vehicle.</param>
        /// <param name="pathFinder">The <see cref="IPathFinder"/> used to calculate routes.</param>
        /// <param name="injector">The <see cref="GhostNodeInjector"/> used to manage temporary nodes during route calculation.</param> 
        public void SetProuth(Prouth prouth, IPathFinder pathFinder, GhostNodeInjector injector)
        {
            Prouth = prouth;
            PathFinder = pathFinder;

            _currentStopIdx = 0;

            RecalculateRoute(injector);

            Debug.WriteLine($"Vehicle {Id} was reassigned a new Prouth and is routing to the first stop.");
        }

        /// <summary>
        /// Recalculates the current route.
        /// </summary>
        /// <remarks>
        /// If the current route or edge tiles are not set, or if the next stop node pair cannot be determined, the method does nothing.
        /// If a ghost node cannot be injected, the route is marked as lost.
        /// </remarks>
        /// <param name="injector">The ghost node injector used to manage temporary nodes during route calculation.</param>
        public void RecalculateRoute(GhostNodeInjector injector)
        {
            if (PathFinder is null) return;
            if (GetCurrentStopNodePair() is not (Node _, Node end)) return;

            IField currentTile;
            if (IsLost || CurrentRoute is null || _currentEdgeTiles is null)
            {
                currentTile = new Stop(MapX, MapY, 1);
            }
            else
            {
                currentTile = _currentEdgeTiles[_currentTileIdx];
            }

            (Node? ghostNode, bool isGhost) = injector.GetOrInjectGhostNode(currentTile);

            if (ghostNode is null)
            {
                IsLost = true;
                return;
            }

            List<Edge>? newRoute = PathFinder.FindPath(ghostNode, end);

            if (isGhost)
            {
                injector.RemoveGhostNode(ghostNode);
            }

            if (newRoute is not null && newRoute.Count == 0)
            {
                CurrentRoute = GetNextRoute();
                StartDrivingFromStopToStop();
                return;
            }

            CurrentRoute = newRoute;
            IsLost = CurrentRoute is null;

            if (CurrentRoute is not null)
            {
                _currentEdgeIdx = 0;
                _currentEdgeTiles = [.. CurrentRoute[_currentEdgeIdx].Roads];
                _currentTileIdx = Math.Max(0, _currentEdgeTiles.IndexOf(currentTile));

                _lerpX = X;
                _lerpY = Y;
            }
        }

        /// <summary>
        /// Gets the lane index of the vehicle based on its current position and the next target tile in its route.
        /// </summary>
        /// <returns>The lane index: 0 = Up, 1 = Right, 2 = Down, 3 = Left.</returns>
        public int GetLaneIdx()
        {
            if (_currentEdgeTiles is null || _currentTileIdx >= _currentEdgeTiles.Count) return 0;

            double targetX = _currentEdgeTiles[_currentTileIdx].X;
            double targetY = _currentEdgeTiles[_currentTileIdx].Y;

            double dx = targetX - _lerpX;
            double dy = targetY - _lerpY;

            // 0 = Up, 1 = Right, 2 = Down, 3 = Left
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // Left or Right
                return dx > 0 ? 1 : 3;
            }
            else
            {
                // Up or Down
                return dy > 0 ? 2 : 0;
            }
        }
        
        public (int X, int Y)? GetNextTileCoordinates()
        {
            if (_currentEdgeTiles is null || IsLost) return null;

            int nextIdx = _currentTileIdx + 1;

            // If there is a next tile in the current edge
            if (nextIdx < _currentEdgeTiles.Count)
            {
                return (_currentEdgeTiles[nextIdx].X, _currentEdgeTiles[nextIdx].Y);
            }

            // End of edge
            if (CurrentRoute is not null && _currentEdgeIdx + 1 < CurrentRoute.Count)
            {
                var nextEdgeRoads = CurrentRoute[_currentEdgeIdx + 1].Roads.ToList();
                if (nextEdgeRoads.Count > 0)
                {
                    return (nextEdgeRoads[0].X, nextEdgeRoads[0].Y);
                }
            }

            return null; // Nowhere to go
        }
        #endregion

        #region Private method
        private void UpdateContinuousPosition(double deltaTime)
        {
            if (_currentEdgeTiles is null) return;

            double startX = _lerpX;
            double startY = _lerpY;

            double targetX = _currentEdgeTiles[_currentTileIdx].X;
            double targetY = _currentEdgeTiles[_currentTileIdx].Y;

            // Linear interpolation: Start + (Difference * Progress)
            // The difference should be always between -1 and 1.
            X = startX + ((targetX - startX) * _tileProgress);
            Y = startY + ((targetY - startY) * _tileProgress);
            //Debug.WriteLine($"Vechile ({Id}) startX={startX:0.00} startY={startY:0.00}  X={X:0.00} Y={Y:0.00} with tileProgress={_tileProgress:0.00}");

            double dx = targetX - startX;
            double dy = targetY - startY;

            if (Math.Abs(dx) > 0.001 || Math.Abs(dy) > 0.001)
            {
                double targetAngle = Math.Atan2(dy, dx) * (180.0 / Math.PI);

                if (targetAngle < 0) targetAngle += 360.0;

                Angle = RotateTowards(Angle, targetAngle, TurnSpeed * deltaTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RotateTowards(double current, double target, double maxDelta)
        {
            double delta = target - current;

            // Normalize to [-180, 180]
            while (delta > 180) delta -= 360;
            while (delta <= -180) delta += 360;

            // If the remaining turn is smaller than our step size, just snap to the target
            if (Math.Abs(delta) <= maxDelta)
            {
                return target;
            }

            double newAngle = current + (Math.Sign(delta) * maxDelta);

            if (newAngle < 0) newAngle += 360;
            if (newAngle >= 360) newAngle -= 360;

            return newAngle;
        }

        /// <summary>
        /// Advances the current tile index to the next tile.
        /// </summary>
        private void AdvanceToNextTile()
        {
            if (_currentEdgeTiles is null) return;

            _currentTileIdx++;

            // We reached the end of the current edge
            if (_currentTileIdx >= _currentEdgeTiles.Count)
            {
                _currentEdgeIdx++;
                _currentTileIdx = 0;

                // We still have edges
                if (CurrentRoute is not null && _currentEdgeIdx < CurrentRoute.Count)
                {
                    _currentEdgeTiles = [.. CurrentRoute[_currentEdgeIdx].Roads];

                    if (_currentEdgeTiles.Count > 1 &&
                        Math.Abs(_currentEdgeTiles[0].X - _lerpX) < 0.001 &&
                        Math.Abs(_currentEdgeTiles[0].Y - _lerpY) < 0.001)
                    {
                        _currentTileIdx = 1;
                    }
                }
                // It was the last edge == stop reached
                else
                {
                    CurrentRoute = GetNextRoute();
                    StartDrivingFromStopToStop();
                }
            }
        }

        /// <summary>
        /// Gets the next pair of nodes representing the start and end stops for the next route.
        /// </summary>
        /// <returns>The next pair of nodes, or <see langword="null"/> if there are not enough stops or the route is not defined.</returns>
        private (Node startNode, Node endNode)? GetNextStopNodePair()
        {
            if (Prouth is null || Prouth.Stops.Count < 2)
            {
                return null;
            }

            Node start = Prouth.Stops[_currentStopIdx];

            var nextId = (_currentStopIdx + 1) % Prouth.Stops.Count;

            Node destination = Prouth.Stops[nextId];
            return (start, destination);
        }

        private (Node startNode, Node endNode)? GetCurrentStopNodePair()
        {
            if (Prouth is null || Prouth.Stops.Count < 2)
            {
                return null;
            }

            var prevIndex = (_currentStopIdx - 1 + Prouth.Stops.Count) % Prouth.Stops.Count;

            Node start = Prouth.Stops[prevIndex];

            Node destination = Prouth.Stops[_currentStopIdx];
            return (start, destination);
        }
        #endregion
    }
}
