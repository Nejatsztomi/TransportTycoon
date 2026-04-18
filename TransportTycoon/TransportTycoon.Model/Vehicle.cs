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
        private List<Field>? _currentEdgeTiles = null;
        #endregion

        #region Properties
        public double TopSpeed { get; protected set; }
        public double CurrentSpeed { get; protected set; }
        public Load? CurrentLoad { get; protected set; }
        public int MaxCapacity { get; protected set; }
        public int CurrentCapacity { get; protected set; }
        public Prouth? Prouth { get; set; }
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
        public Field? TargetTile
        {
            get
            {
                if (_currentEdgeTiles is null || _currentTileIdx >= _currentEdgeTiles.Count) return null;
                return _currentEdgeTiles[_currentTileIdx];
            }
        }
        #endregion

        #region Public methods

        public void Step()
        {
            if(CurrentRoute is null) return;

            Field? targetTile = TargetTile;
            if (targetTile == null) return;

            //update the direction
            UpdateDirection(targetTile);

            //check if we have arrived at the target tile
            double distanceToTarget = Math.Sqrt(Math.Pow(X - targetTile.X, 2) + Math.Pow(Y - targetTile.Y, 2));
            if (distanceToTarget < 0.1 || distanceToTarget <=CurrentSpeed) //if we are close enough to the target tile, we consider that we have arrived
            {
                X = targetTile.X;
                Y = targetTile.Y;
                AdvanceToNextTile();

                Field? targetTile3 = TargetTile;
                if (targetTile3 == null) return;

                //update the direction
                UpdateDirection(targetTile);

                return;
            }

            //take the step
            MoveTowardsTarget(targetTile);

            Field? targetTile2 = TargetTile;
            if (targetTile2 == null) return;

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
        public void StartDriving(List<Edge> route)
        {
            CurrentRoute = route;
            _currentEdgeIdx = 0;
            _currentTileIdx = 0;
            _tileProgress = 0.0;

            if (CurrentRoute.Count > 0)
            {
                // Cache the tiles for the very first edge
                _currentEdgeTiles= CurrentRoute[0].Roads.ToList();
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
        public void GetNextRoute(IPathFinder pathFinder)
        {
            if (GetNextStopNodePair() is (Node start, Node end))
            {
                CurrentRoute = pathFinder.FindPath(start, end);
            }
            StartDriving(CurrentRoute ?? new List<Edge>());
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
        public void RecalculateRoute(IPathFinder pathFinder, GhostNodeInjector injector)
        {
            if (CurrentRoute is null || _currentEdgeTiles is null) return;
            if (GetNextStopNodePair() is not (Node _, Node end)) return;

            Field currentTile = _currentEdgeTiles[_currentTileIdx];

            (Node? startNode, bool isGhost) = injector.GetOrInjectGhostNode(currentTile);

            if (startNode is null)
            {
                IsLost = true;
                return;
            }

            List<Edge>? newRoute = pathFinder.FindPath(startNode, end);

            if (isGhost)
            {
                injector.RemoveGhostNode(startNode);
            }

            CurrentRoute = newRoute;
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
        private void UpdateDirection(Field target) 
        {
            if(target.X<MapX) Direction = Direction.Up;
            else if(target.X>MapX) Direction = Direction.Down;
            else if(target.Y<MapY) Direction = Direction.Left;
            else if(target.Y>MapY) Direction = Direction.Right;
        }

        private void MoveTowardsTarget(Field target) 
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
        private void AdvanceToNextTile() 
        {
            if (_currentEdgeTiles == null) return;

            _currentTileIdx++;

            if(_currentTileIdx>= _currentEdgeTiles.Count)
            {
                _currentEdgeIdx++;
                if (CurrentRoute != null && _currentEdgeIdx + 1 < CurrentRoute.Count)
                {
                    ArriveAtStop();
                }
                else //if its the last edge, but there are more stops in the prouth, we need to get the next route
                {
                    _currentEdgeIdx++;
                    _currentTileIdx = 0;
                    if (CurrentRoute != null) 
                    {
                        _currentEdgeTiles = CurrentRoute[_currentEdgeIdx].Roads.ToList();
                    }
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

        
        #endregion
    }
}
