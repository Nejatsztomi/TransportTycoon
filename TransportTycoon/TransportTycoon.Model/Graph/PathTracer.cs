using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    public enum TraceStatus : byte
    {
        FoundIntersection,
        DeadEnd,
        ClosedCircle,
    }

    public readonly record struct TraceResult(
        IField? EndTile,
        List<IField> PathTaken,
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
        public PathTracer(GameTable gameTable)
        {
            _gameTable = gameTable;
        }
        #endregion

        #region Public methods
        public TraceResult TraceSegment(IField startTile, (int dx, int dy) initialMomentum)
        {
            List<IField> pathTaken = [startTile];
            IField currentTile = startTile;
            var momentum = initialMomentum;

            double forwardCost = 0.0;
            double backwardCost = 0.0;

            while (true)
            {
                int nextX = currentTile.X + momentum.dx;
                int nextY = currentTile.Y + momentum.dy;

                if (!IsValidRoad(nextX, nextY)) return new TraceResult(null, pathTaken, 0.0, 0.0, TraceStatus.DeadEnd);

                IField nextTile = _gameTable[nextX, nextY];
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
                        IField neighbor = _gameTable[neighborX, neighborY];

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
            return _gameTable.IsInBounds(x, y) && _gameTable[x, y] is IInfrastructure;
        }

        private bool CanMoveTo(IField currentTile, IField nextTile, (int dx, int dy) momentum)
        {
            if (currentTile is not IInfrastructure currentRoad || nextTile is not IInfrastructure nextRoad) return false;

            if (!HasExit(currentRoad, momentum)) return false;

            (int enterDx, int enterDy) = (-momentum.dx, -momentum.dy);
            if (!HasExit(nextRoad, (enterDx, enterDy))) return false;

            if (Math.Abs(nextTile.Height - currentTile.Height) > 1) return false;

            return true;
        }

        private bool HasExit(IInfrastructure infrastructure, (int dx, int dy) direction)
        {
            // Stops can be entered and exited from any direction
            if (infrastructure is Stop) return true;
            if (infrastructure is not Road r) return true;

            var up = (0, -1);
            var down = (0, 1);
            var right = (1, 0);
            var left = (-1, 0);

            return r.RoadType switch
            {
                RoadType.Vertical => direction == up || direction == down,
                RoadType.Horizontal => direction == right || direction == left,

                RoadType.UpperLeftTurn => direction == up || direction == right,
                RoadType.LeftTurn => direction == down || direction == left,
                RoadType.RightTurn => direction == down || direction == right,
                RoadType.UpperRightTurn => direction == up || direction == left,

                // Junctions
                RoadType.DownTRoad => direction == left || direction == down || direction == right,
                RoadType.UpperTRoad => direction == left || direction == up || direction == right,
                RoadType.LeftTRoad => direction == up || direction == down || direction == left,
                RoadType.RightTRoad => direction == up || direction == down || direction == right,

                RoadType.XRoad => true, // All directions are valid

                _ => false
            };
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
        private bool IsJunction(IField field)
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
        private bool IsTerminatingField(IField field)
        {
            return field is Stop || IsJunction(field);
        }
        #endregion
    }
}
