using System.Diagnostics;
using System.Runtime.CompilerServices;
using TransportTycoon.MapData;
using TransportTycoon.Model.Graph;

namespace TransportTycoon.Model
{
    public enum Direction
    {
        Up = 0, Down = 1, Left = 2, Right = 3
    }

    public enum VehicleType
    {
        Van = 0, Pickup = 1, Truck = 2, LiquidTruck = 3, SmallBus = 4, BigBus = 5
    }

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
        public UInt64 Id { get; private set; }
        public double TopSpeed { get; protected set; }
        public double CurrentSpeed { get; protected set; }
        public Load? CurrentLoad { get; protected set; }
        public int MaxCapacity { get; protected set; }
        public int CurrentCapacity { get; protected set; }
        public Prouth? Prouth
        {
            get;
            set;
        }
        /// <summary>
        /// The current route of the vehicle, represented as a list of <see cref="Edge"/> objects.
        /// The route may be <see langword="null"/> if the vehicle does not have a current route assigned.
        /// The vehicle will be repeating this route, if not given a new one.
        /// </summary>
        public List<Edge>? CurrentRoute { get; protected set; }
        public VehicleType Type { get; protected set; }
        public double X { get; protected set; }
        public double Y { get; protected set; }
        [Obsolete]
        public Direction Direction { get; protected set; }

        /// <summary>
        /// The angle of the vehicle in degrees, where <see langword="0"/> degrees represents facing right (positive X direction), and angles increase counterclockwise (90 degrees is up, 180 degrees is left, and 270 degrees is down).
        /// </summary>
        public double Angle { get; protected set; }

        /// <summary>
        /// How fast should the vehicle turn in degrees per second.
        /// </summary>
        public double TurnSpeed { get; protected set; } = 360.0;
        public int Price { get; protected set; }
        public int Maintenance { get; protected set; }

        public int MapX => (int)Math.Round(X);
        public int MapY => (int)Math.Round(Y);
        public List<LoadType>? AcceptedGoods { get; protected set; } = [];

        /// <summary>
        /// <see langword="true"/> if the vehicle is lost, meaning it cannot find a valid route to its destination and is effectively stuck.
        /// This can occur when the vehicle's current position is not connected to the rest of the map, or when all possible routes to the destination are blocked or inaccessible.
        /// When a vehicle is marked as lost, it may require intervention to be moved back onto a valid path or to be removed from the game if it cannot be recovered.
        /// </summary>
        public bool IsLost { get; private set; } = false;
        public IField? TargetTile
        {
            get
            {
                if (_currentEdgeTiles is null || _currentTileIdx >= _currentEdgeTiles.Count) return null;
                return _currentEdgeTiles[_currentTileIdx];
            }
        }

        public IPathFinder? PathFinder
        {
            get;
            set
            {
                field = value;
            }
        } = null;
        #endregion

        #region Protected constructor
        protected Vehicle()
        {
            Id = _globalIdCounter++;
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
        /// Changes the current speed of the vehicle, if the given speed is between 0 and the top speed of the vehicle
        /// </summary>
        /// <param name="speed"></param>
        public void ChangeCurrentSpeed(double speed)
        {
            if (speed >= 0 && speed <= TopSpeed) CurrentSpeed = speed;
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
        /// If the vehicle already has a <see cref="Prouth"/> assigned, it will recalculate its route using the new <see cref="Prouth"/> and start driving to the first stop of the new <see cref="Prouth"/>.
        /// If the vehicle did not have a <see cref="Prouth"/> before, it will simply get the next route and start driving.
        /// </summary>
        /// <param name="prouth">The <see cref="Prouth"/> to assign to the vehicle.</param>
        /// <param name="pathFinder">The <see cref="IPathFinder"/> used to calculate routes.</param>
        /// <param name="injector">The <see cref="GhostNodeInjector"/> used to manage temporary nodes during route calculation.</param> 
        public void SetProuth(Prouth prouth, IPathFinder pathFinder, GhostNodeInjector injector)
        {
            bool alreadyHadProuth = Prouth is not null;
            Prouth = prouth;
            PathFinder = pathFinder;

            // If Prouth is not null, the vehicle has a valid Prouth, and can be mid-route
            // if we called this method, the vehicle should recalculate it's route via the new Prouth.
            // It should always go to the first stop of the new Prouth if possible, else be lost.
            if (alreadyHadProuth)
            {
                _currentStopIdx = 0;

                RecalculateRoute(injector);

                Debug.WriteLine($"Vehicle {Id} was reassigned a new Prouth and is routing to the first stop.");
            }
            else
            {

                CurrentRoute = GetNextRoute();
                StartDrivingFromStopToStop();
            }
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
            if (PathFinder is null || Prouth is null) return;
            if (GetCurrentStopNodePair() is not (Node _, Node end)) return;

            IField currentTile;
            if (IsLost)
            {
                currentTile = new Stop(MapX, MapY, 1);
            }
            else
            {
                if (CurrentRoute is null || _currentEdgeTiles is null) return;
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
                        _currentEdgeTiles[0].X == _lerpX &&
                        _currentEdgeTiles[0].Y == _lerpY)
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
