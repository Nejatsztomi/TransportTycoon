using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A DTO for the result of a walk performed by a <see cref="Walker"/>.
    /// </summary>
    /// <param name="CreatedEdges">The edges created during the walk.</param>
    /// <param name="NewWalkers">The new walkers spawned from the current walk.</param>
    /// <param name="EndNode">The node where the walk ended, if any.</param>
    public record WalkerResult(
        List<Edge> CreatedEdges,
        List<Walker> NewWalkers,
        Node? EndNode
    );

    public class Walker
    {
        #region Private fields
        /// <summary>
        /// The node where we branch off.
        /// </summary>
        private readonly Node _startNode;
        /// <summary>
        /// The field where the walk starts.
        /// </summary>
        private readonly IField _startField;
        /// <summary>
        /// The game table to traverse.
        /// </summary>
        private readonly GameTable _gameTable;
        /// <summary>
        /// The already visited fields. Including roads, bridges and stops.
        /// </summary>
        private readonly HashSet<(int X, int Y)> _visitedFields;
        /// <summary>
        /// The already visited junctions.
        /// This is to prevent respawning <see cref="Walker"/> objects, making an infinite loop.
        /// </summary>
        private readonly HashSet<(int X, int Y)> _visitedJunctions;
        /// <summary>
        /// The recorded roads during the walk.
        /// This is used to create the edges after the walk is finished.
        /// </summary>
        private readonly List<IField> _roads = [];
        /// <summary>
        /// The previous field's coordinates.
        /// </summary>
        private (int X, int Y)? _previousFieldCoords = null;
        /// <summary>
        /// The total cost of traversal.
        /// Records both the forward and backwards cost that can change depending on the height difference of the fields.
        /// </summary>
        private (double forwardCost, double backwardCost) _roadCost = (0.0, 0.0);
        /// <summary>
        /// The field where the walker "stands" on.
        /// </summary>
        private IField _currentField;
        #endregion

        #region Constructors
        /// <summary>
        /// The class is design to perform a walk (or traverse) between two junctions (stops or crossroads) on the <see cref="GameTable"/>, starting from a given field.
        /// During the walk, it records the roads it traverses and calculates the cost of traversal based on the height differences of the fields.
        /// The walk continues until it reaches another junction, at which point it creates edges between the starting node and the terminating node, and potentially spawns new walkers for further exploration.
        /// The walker also keeps track of visited fields and junctions to avoid infinite loops and redundant traversals.
        /// </summary>
        /// <param name="startNode">The node where the road branched off.</param>
        /// <param name="startField">The field where the walk starts.</param>
        /// <param name="gameTable">The game table to traverse.</param>
        /// <param name="visitedFields">The already visited fields.</param>
        /// <param name="visitedJunctions">The already visited junctions.</param>
        public Walker(Node startNode, IField startField, GameTable gameTable, HashSet<(int X, int Y)> visitedFields, HashSet<(int X, int Y)> visitedJunctions)
        {
            _startNode = startNode;
            _startField = startField;
            _currentField = startField;
            _gameTable = gameTable;
            _visitedFields = visitedFields;
            _visitedJunctions = visitedJunctions;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Perfoms a "walk" (traverse) on the given <see cref="GameTable"/>.
        /// It checks each direction (north, east, south, west) for valid fields to step on, and continues the walk until it reaches a terminating field (a stop or a junction).
        /// </summary>
        /// <returns>A <see cref="WalkerResult"/> containing the edges, new walkers, and the end node of the walk.</returns>
        public WalkerResult Walk()
        {
            (int dirx, int diry)[] directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            _roads.Add(_gameTable[_startNode.X, _startNode.Y]);
            Node? terminatingNode = Step(_startField);

            while (terminatingNode is null)
            {
                IField? nextField = null;
                foreach ((int dirx, int diry) in directions)
                {
                    nextField = GetValidField(_currentField.X + dirx, _currentField.Y + diry);
                    if (nextField is not null)
                    {
                        break;
                    }
                }

                if (nextField is null)
                {
                    break;
                }

                terminatingNode = Step(nextField);
            }

            List<Edge> edges = [];
            List<Walker> newWalkers = [];
            Node? endNode = null;

            if (terminatingNode is not null)
            {
                SharedRoadSequence sharedRoadSequence = new(_roads);
                edges.Add(new(_startNode, terminatingNode, sharedRoadSequence.ForwardEnumerator(), _roadCost.forwardCost));
                edges.Add(new(terminatingNode, _startNode, sharedRoadSequence.BackwardEnumerator(), _roadCost.backwardCost));

                if (!_visitedJunctions.Contains((terminatingNode.X, terminatingNode.Y)))
                {
                    _visitedJunctions.Add((terminatingNode.X, terminatingNode.Y));

                    foreach ((int dirx, int diry) in directions)
                    {
                        IField? field = GetValidField(terminatingNode.X + dirx, terminatingNode.Y + diry);
                        if (field is null)
                        {
                            continue;
                        }
                        newWalkers.Add(new Walker(terminatingNode, field, _gameTable, _visitedFields, _visitedJunctions));
                    }
                }

                endNode = terminatingNode;
            }

            return new WalkerResult(edges, newWalkers, endNode);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// A helper method to "step" to walker to the given field.
        /// </summary>
        /// <param name="nextField">The field to step onto.</param>
        /// <returns>The <see cref="Node"/> if the field is a terminating field; otherwise, <see langword="null"/>.</returns>
        private Node? Step(IField nextField)
        {
            _visitedFields.Add((nextField.X, nextField.Y));
            _roads.Add(nextField);

            int heightDifference = nextField.Height - _gameTable.Table[_currentField.X, _currentField.Y].Height;
            CalculateRoadCost(heightDifference);

            if (IsTerminatingField(nextField))
            {
                return new Node(nextField.X, nextField.Y, nextField.GetType());
            }

            _previousFieldCoords = (_currentField.X, _currentField.Y);
            _currentField = nextField;
            return null;
        }

        /// <summary>
        /// Determines whether the specified field type is a valid type for processing.
        /// </summary>
        /// <param name="field">The field type to validate. Only certain field types are considered valid.</param>
        /// <returns><see langword="true"/> if the specified field type is valid; otherwise, <see langword="false"/>.</returns>
        private bool IsValidFieldType(IField field) => field is Road || field is IBridge || field is Stop;

        /// <summary>
        /// Calculates and updates the forward and backward road costs based on the specified height difference.
        /// </summary>
        /// <param name="heightDifference">The difference in height between two points.</param>
        private void CalculateRoadCost(int heightDifference)
        {
            if (heightDifference > 0)
            {
                _roadCost.forwardCost += 2.0;
                _roadCost.backwardCost += 0.8;
            }
            else if (heightDifference < 0)
            {
                _roadCost.forwardCost += 0.8;
                _roadCost.backwardCost += 2.0;
            }
            _roadCost.forwardCost += 1.0;
            _roadCost.backwardCost += 1.0;
        }

        /// <summary>
        /// Get's the valid field for the walker to step onto based on the specified coordinates.
        /// </summary>
        /// <param name="x">The field's X coordinate.</param>
        /// <param name="y">The field's Y coordinate.</param>
        /// <returns>The valid <see cref="IField"/> if available; otherwise, <see langword="null"/>.</returns>
        private IField? GetValidField(int x, int y)
        {
            // Prevent the walker from stepping back onto the starting node.
            if (x == _startNode.X && y == _startNode.Y)
            {
                return null;
            }

            if (!_gameTable.IsInBounds(x, y))
            {
                return null;
            }
            IField nextField = _gameTable.Table[x, y];
            if (!IsValidFieldType(nextField))
            {
                return null;
            }

            if (_previousFieldCoords.HasValue && (x, y) == _previousFieldCoords.Value)
            {
                return null;
            }

            // Height - check
            if (Math.Abs(nextField.Height - _gameTable.Table[_currentField.X, _currentField.Y].Height) > 1)
            {
                return null;
            }

            // Allow the walker to step on junctions again.
            if (!IsTerminatingField(nextField) && _visitedFields.Contains((nextField.X, nextField.Y)))
            {
                return null;
            }
            return nextField;
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
            if (field is not Road road)
            {
                return false;
            }
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
