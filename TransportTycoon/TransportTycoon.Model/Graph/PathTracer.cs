using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// The status of a path tracing operation, indicating the outcome of the trace.
    /// </summary>
    public enum TraceStatus : byte
    {
        FoundIntersection,
        DeadEnd,
        ClosedCircle,
    }

    /// <summary>
    /// Represents the result of a trace operation, including the path taken, costs, and final status.
    /// </summary>
    /// <param name="EndTile">The field where the trace operation ended, or null if the trace did not reach a valid end point.</param>
    /// <param name="PathTaken">The ordered list of fields traversed during the trace operation. The list may be empty if no path was found.</param>
    /// <param name="ForwardCost">The total cost accumulated when tracing from the start to the end tile. The value is typically non-negative.</param>
    /// <param name="BackwardCost">The total cost accumulated when tracing from the end tile back to the start, if applicable. The value is typically non-negative.</param>
    /// <param name="Status">The final status of the trace operation, indicating success, failure, or other relevant outcome.</param>
    public readonly record struct TraceResult(
        Field? EndTile,
        List<Field> PathTaken,
        double ForwardCost,
        double BackwardCost,
        TraceStatus Status
    );

    /// <summary>
    /// A stateless engine responsible strictly for navigating a single road segment 
    /// according to the physical rules of the game world.
    /// </summary>
    public class PathTracer
    {
        #region Private fields
        private static readonly (int dx, int dy)[] _directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];

        private readonly GameTable _gameTable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PathTracer class using the specified game table.
        /// </summary>
        /// <param name="gameTable">The GameTable instance that provides the context for path tracing operations. Cannot be null.</param>
        public PathTracer(GameTable gameTable)
        {
            _gameTable = gameTable;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Traces a path segment starting from the specified tile and initial momentum, following valid road
        /// connections until an intersection, dead end, or closed loop is encountered.
        /// </summary>
        /// <remarks>The method follows the path in the direction of the initial momentum, only proceeding
        /// through valid road connections. The trace stops if it reaches an intersection, a dead end, or forms a closed
        /// loop back to the starting tile. The returned TraceResult provides details about the outcome and the path
        /// taken.</remarks>
        /// <param name="startTile">The starting tile from which to begin tracing the segment. Must be a valid field on the game table.</param>
        /// <param name="initialMomentum">The initial movement direction, represented as a tuple of (dx, dy), indicating the change in X and Y
        /// coordinates per step.</param>
        /// <returns>A TraceResult containing information about the traced path, including the endpoint (if found), the sequence
        /// of tiles traversed, accumulated forward and backward costs, and the status indicating how the trace
        /// terminated.</returns>
        public TraceResult TraceSegment(Field startTile, (int dx, int dy) initialMomentum)
        {
            List<Field> pathTaken = [startTile];
            Field currentTile = startTile;
            var momentum = initialMomentum;

            double forwardCost = 0.0;
            double backwardCost = 0.0;

            while (true)
            {
                int nextX = currentTile.X + momentum.dx;
                int nextY = currentTile.Y + momentum.dy;

                if (!IsValidRoad(nextX, nextY)) return new TraceResult(null, pathTaken, 0.0, 0.0, TraceStatus.DeadEnd);

                Field nextTile = _gameTable[nextX, nextY];
                if (!CanMoveTo(currentTile, nextTile, momentum)) return new TraceResult(null, pathTaken, 0.0, 0.0, TraceStatus.DeadEnd);

                if (nextTile.X == startTile.X && nextTile.Y == startTile.Y) return new TraceResult(null, pathTaken, 0.0, 0.0, TraceStatus.ClosedCircle);

                CalculateRoadCost(nextTile.Height - currentTile.Height, ref forwardCost, ref backwardCost);
                pathTaken.Add(nextTile);
                currentTile = nextTile;

                if (IsTerminatingField(currentTile)) return new TraceResult(currentTile, pathTaken, forwardCost, backwardCost, TraceStatus.FoundIntersection);

                (int backDx, int backDy) = (-momentum.dx, -momentum.dy);
                (int dx, int dy) nextMomentum = (0, 0);
                int validExits = 0;

                foreach (var dir in _directions)
                {
                    if (dir == (backDx, backDy)) continue;

                    int neighborX = nextTile.X + dir.dx;
                    int neighborY = nextTile.Y + dir.dy;

                    if (_gameTable.IsInBounds(neighborX, neighborY))
                    {
                        Field neighbor = _gameTable[neighborX, neighborY];

                        if (CanMoveTo(currentTile, neighbor, dir))
                        {
                            validExits++;
                            nextMomentum = dir;
                        }
                    }
                }

                if (validExits == 1) momentum = nextMomentum;
                else return new TraceResult(null, pathTaken, 0.0, 0.0, TraceStatus.DeadEnd);
            }
        }
        #endregion

        #region Private methods
        private bool IsValidRoad(int x, int y)
        {
            return _gameTable.IsInBounds(x, y) && _gameTable[x, y] is Infrastructure;
        }

        private bool CanMoveTo(Field currentTile, Field nextTile, (int dx, int dy) momentum)
        {
            if (Math.Abs(nextTile.Height - currentTile.Height) > 1) return false;

            bool canExitCurrent = currentTile switch
            {
                Road currentRoad => HasExit(currentRoad.RoadType, momentum),
                Stop => true,
                Bridge currentBridge => IsValidBridgeDirection(currentBridge, momentum),
                _ => false
            };

            if (!canExitCurrent) return false;

            (int enterDx, int enterDy) = (-momentum.dx, -momentum.dy);
            bool canEnterNext = nextTile switch
            {
                Road nextRoad => HasExit(nextRoad.RoadType, (enterDx, enterDy)),
                Stop => true,
                Bridge nextBridge => IsValidBridgeDirection(nextBridge, (enterDx, enterDy)),
                _ => false
            };

            return canEnterNext;
        }

        private bool HasExit(RoadType roadType, (int dx, int dy) direction)
        {
            var up = (0, -1);
            var down = (0, 1);
            var right = (1, 0);
            var left = (-1, 0);

            return roadType switch
            {
                RoadType.Vertical => direction == up || direction == down,
                RoadType.Horizontal => direction == right || direction == left,

                RoadType.UpperLeftTurn => direction == up || direction == left,
                RoadType.LeftTurn => direction == down || direction == left,
                RoadType.RightTurn => direction == down || direction == right,
                RoadType.UpperRightTurn => direction == up || direction == right,

                // Junctions
                RoadType.DownTRoad => direction == left || direction == down || direction == right,
                RoadType.UpperTRoad => direction == left || direction == up || direction == right,
                RoadType.LeftTRoad => direction == up || direction == down || direction == left,
                RoadType.RightTRoad => direction == up || direction == down || direction == right,

                RoadType.XRoad => true, // All directions are valid

                _ => false
            };
        }

        private bool IsValidBridgeDirection(Bridge bridge, (int dx, int dy) direction)
        {
            var up = (0, -1);
            var down = (0, 1);
            var right = (1, 0);
            var left = (-1, 0);

            if (bridge.BridgeType == BridgeType.VerticalGreenBridge || bridge.BridgeType == BridgeType.VerticalYellowBridge || bridge.BridgeType == BridgeType.VerticalRedBridge)
            {
                return direction == up || direction == down;
            }
            else
            {
                return direction == left || direction == right;
            }
        }

        /// <summary>
        /// Calculates and updates the forward and backward road costs based on the specified height difference.
        /// </summary>
        /// <param name="heightDifference">The difference in height between two points.</param>
        private void CalculateRoadCost(int heightDifference, ref double forwardCost, ref double backwardCost)
        {
            forwardCost += 1.0;
            backwardCost += 1.0;

            if (heightDifference > 0)
            {
                forwardCost += 1.0;
                backwardCost -= 0.2;
            }
            else if (heightDifference < 0)
            {
                forwardCost -= 0.2;
                backwardCost += 1.0;
            }
        }

        /// <summary>
        /// Determines whether the specified field represents a road junction.
        /// </summary>
        /// <remarks>
        /// A field is considered a junction if it is a road with a road type of XRoad, DownTRoad, UpperTRoad, LeftTRoad, or RightTRoad.
        /// </remarks>
        /// <param name="field">The field to evaluate for junction status.</param>
        /// <returns><see langword="true"/> if the field is a road and its road type represents a junction; otherwise, <see langword="false"/>.</returns>
        private bool IsJunction(Field field)
        {
            if (field is not Road road) return false;

            return road.RoadType == RoadType.XRoad
                || road.RoadType == RoadType.DownTRoad
                || road.RoadType == RoadType.UpperTRoad
                || road.RoadType == RoadType.LeftTRoad
                || road.RoadType == RoadType.RightTRoad;
        }

        /// <summary>
        /// Determines whether the specified field is a terminating one.
        /// </summary>
        /// <param name="field">The field to evaluate for termination status.</param>
        /// <returns><see langword="true"/> if the field is a stop or a junction; otherwise, <see langword="false"/>.</returns>
        private bool IsTerminatingField(Field field)
        {
            return field is Stop || IsJunction(field);
        }
        #endregion
    }
}
