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
        /// //TODO: maybe we can use this to make the movement smoother, instead of just moving from tile to tile, we can move smoothly between them based on the current speed and the distance to the next tile.
        //private double _tileProgress = 0.0;
        /// <summary>
        /// The current edge's tiles.
        /// </summary>
        private List<IField>? _currentEdgeTiles = null;
        /// <summary>
        /// the vehicles Route, which is a list of stops that the vehicle should follow in order. The vehicle will be moving from one stop to the next in the order they are listed in the prouth. When the vehicle reaches the last stop in the prouth, it will loop back to the first stop and continue following the route indefinitely.
        /// </summary>
        private Prouth? _prouth;
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
            get => _prouth;
            set
            {
                _prouth = value;
                //_currentStopIdx = 0;
                //CurrentRoute = null;
            }
        }
        /// <summary>
        /// The current route of the vehicle, represented as a list of edges.
        /// The route may be null if the vehicle does not have a current route assigned.
        /// The vehicle will be repeating this route, if not given a new one.
        /// </summary>
        public List<Edge>? CurrentRoute { get; protected set; }
        public VehicleType Type { get; protected set; }
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public Direction Direction { get; protected set; }
        public int Price { get; protected set; }
        public int Maintance { get; protected set; }

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
        #endregion

        #region Protected constructor
        protected Vehicle()
        {
            Id = _globalIdCounter++;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Advances the entity along its current route by moving it one step toward the next target tile, updating its
        /// position and direction as needed.
        /// </summary>
        /// <remarks>This method performs a single movement operation for the entity. If the entity is
        /// close enough to the target tile or can reach it within the current speed, it advances to the next tile in
        /// the route. The method has no effect if there is no active route or target tile.</remarks>
        public void Step()
        {
            if (CurrentRoute is null) return;

            IField? targetTile = TargetTile;
            if (targetTile is null) return;

            //update the direction
            UpdateDirection(targetTile);

            //check if we have arrived at the target tile
            double distanceToTarget = Math.Sqrt(Math.Pow(X - targetTile.X, 2) + Math.Pow(Y - targetTile.Y, 2));
            if (distanceToTarget < 0.1 || distanceToTarget <= CurrentSpeed) //if we are close enough to the target tile, we consider that we have arrived
            {
                X = targetTile.X;
                Y = targetTile.Y;
                AdvanceToNextTile();

                targetTile = TargetTile;
                if (targetTile is null) return;

                //update the direction
                UpdateDirection(targetTile);

                return;
            }

            //take the step
            MoveTowardsTarget(targetTile);

            targetTile = TargetTile;
            if (targetTile is null) return;

            //update the direction
            UpdateDirection(targetTile);
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
        /// Sets the current route of the vehicle to the specified list of edges and initializes the indices for tracking the current edge and tile.
        /// </summary>
        public void StartDriving(List<Edge> route)
        {
            CurrentRoute = route;
            _currentEdgeIdx = 0;
            _currentTileIdx = 0;
            //_tileProgress = 0.0;

            if (CurrentRoute.Count > 0)
            {
                _currentEdgeTiles = [.. CurrentRoute[0].Roads];
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
        /// <param name="pathFinder">The path finder to use for finding the route.</param>
        public void GetNextRoute(IPathFinder pathFinder, GhostNodeInjector injector, IField currentTile)
        {
            if (Prouth is null || Prouth.Stops.Count == 0) return;

            //the current destination
            Node destination = Prouth.Stops[_currentStopIdx];

            //if we are in the same tile as the destination, we can skip to the next stop, and get the route to the next stop, this can happen when we are already at the stop, but we don't have a route yet, or when we are at the stop but we are not exactly on the tile of the stop, so we can consider that we have arrived at the stop and move to the next one.
            if (currentTile.X == destination.X && currentTile.Y == destination.Y)
            {
                AdvanceProuth();
                destination = Prouth.Stops[_currentStopIdx];
            }
            CalculatePathToDestination(destination, pathFinder, injector, currentTile);
        }

        /// <summary>
        /// Recalculates the current route.
        /// </summary>
        /// <remarks>
        /// If the current route or edge tiles are not set, or if the next stop node pair cannot be determined, the method does nothing.
        /// If a ghost node cannot be injected, the route is marked as lost.
        /// </remarks>
        /// <param name="pathFinder">The path finder used to compute a new route between nodes.</param>
        /// <param name="injector">The ghost node injector used to manage temporary nodes during route calculation.</param>
        public void RecalculateRoute(IPathFinder pathFinder, GhostNodeInjector injector, IField currentTile)
        {
            if (Prouth == null || Prouth.Stops.Count == 0) return;

            Node destination = Prouth.Stops[_currentStopIdx];

            CalculatePathToDestination(destination, pathFinder, injector, currentTile);
        }
        #endregion

        #region Private method
        /// <summary>
        /// A helper method to be called when the vehicle arrives at a stop.
        /// It resets the current route and edge tiles, and advances the prouth to the next stop.
        /// After this method is called the vehicle should be ready to get the next route to the next stop in the prouth.
        /// </summary>
        private void ArriveAtStop()
        {
            CurrentRoute = null;
            _currentEdgeTiles = null;
            AdvanceProuth();
        }

        /// <summary>
        /// if the target tile is in a different direction than the current one, it updates the direction to face towards the target tile.
        /// </summary>
        /// <param name="target"></param>
        private void UpdateDirection(IField target)
        {
            if (target.X < MapX) Direction = Direction.Up;
            else if (target.X > MapX) Direction = Direction.Down;
            else if (target.Y < MapY) Direction = Direction.Left;
            else if (target.Y > MapY) Direction = Direction.Right;
        }

        /// <summary>
        /// Moves the object toward the specified target field based on the current direction and speed.
        /// </summary>
        /// <remarks>The method updates the object's position by adjusting its coordinates according to
        /// the current direction and speed. The direction is determined by the Direction property, which can be set to
        /// Up, Down, Left, or Right.</remarks>
        /// <param name="target">The field that the object is moving toward. This parameter determines the destination used to update the
        /// object's direction and position.</param>
        private void MoveTowardsTarget(IField target)
        {
            UpdateDirection(target);
            switch (Direction)
            {
                case Direction.Up:
                    X -= CurrentSpeed;
                    break;
                case Direction.Down:
                    X += CurrentSpeed;
                    break;
                case Direction.Right:
                    Y += CurrentSpeed;
                    break;
                case Direction.Left:
                    Y -= CurrentSpeed;
                    break;
            }
        }

        /// <summary>
        /// advances the current tile index to the next tile in the current edge.
        /// </summary>
        private void AdvanceToNextTile()
        {
            if (_currentEdgeTiles == null) return;

            _currentTileIdx++;

            if (_currentTileIdx >= _currentEdgeTiles.Count)
            {
                _currentEdgeIdx++;
                _currentTileIdx = 0;

                //check if we have more edges in the current route
                if (CurrentRoute != null && _currentEdgeIdx < CurrentRoute.Count)
                {
                    _currentEdgeTiles = [.. CurrentRoute[_currentEdgeIdx].Roads];

                    //if we reach a crossroad we should move to the next tile
                    if (_currentEdgeTiles.Count > 0 && _currentEdgeTiles[0].X == this.MapX && _currentEdgeTiles[0].Y == this.MapY)
                    {
                        _currentEdgeTiles.RemoveAt(0);
                    }
                }
                else //if its the last edge, we reached the Stop
                {
                    ArriveAtStop();
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

            int nextIndex = (_currentStopIdx + 1) % Prouth.Stops.Count;

            Node destination = Prouth.Stops[nextIndex];
            return (start, destination);
        }

        /// <summary>
        /// Move the <see cref="_currentEdgeIdx"/> "pointer" forward.
        /// At the end it loops back to <see langword="0"/>.
        /// </summary>
        private void AdvanceProuth()
        {
            if (Prouth is null || Prouth.Stops.Count == 0) return;

            _currentStopIdx = (_currentStopIdx + 1) % Prouth.Stops.Count;
        }
        private void CalculatePathToDestination(Node destination, IPathFinder pathFinder, GhostNodeInjector injector, IField currentTile)
        {
            (Node? startNode, bool isGhost) = injector.GetOrInjectGhostNode(currentTile);

            if (startNode is null)
            {
                IsLost = true;
                CurrentRoute = null;
                return;
            }

            CurrentRoute = pathFinder.FindPath(startNode, destination);

            if (isGhost)
            {
                injector.RemoveGhostNode(startNode);
            }

            if (CurrentRoute != null && CurrentRoute.Count > 0)
            {
                IsLost = false;
                StartDriving(CurrentRoute);
            }
            else
            {
                IsLost = true;
                CurrentRoute = null;
            }
        }
        #endregion
    }
}
