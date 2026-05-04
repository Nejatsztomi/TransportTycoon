using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// Injects a ghost (temporary) <see cref="Node"/> into the graph at the current position of the vehicle if there is no "real" <see cref="Node"/> at that position.
    /// </summary>
    public class GhostNodeInjector
    {
        #region Private fields
        private static readonly (int dx, int dy)[] _directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];

        private readonly GameTable _gameTable;
        private readonly Graph _graph;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GhostNodeInjector"/> class with the specified <see cref="GameTable"/> and <see cref="Graph"/>.
        /// </summary>
        /// <param name="gameTable">The <see cref="GameTable"/> that provides the context or data for node injection. Cannot be <see langword="null"/>.</param>
        /// <param name="graph">The <see cref="Graph"/> in which ghost nodes will be injected. Cannot be <see langword="null"/>.</param>
        public GhostNodeInjector(GameTable gameTable, Graph graph)
        {
            _gameTable = gameTable;
            _graph = graph;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// If there is a real node at the current vehicle tile, return it.
        /// Otherwise, inject a ghost node and connect it to all reachable junctions, then return the ghost node.
        /// </summary>
        /// <param name="currentVehicleTile">The current tile of the vehicle.</param>
        /// <returns>A tuple containing the <see cref="Node"/> to use and a <see langword="bool"> indicating if it is a ghost node.</returns>
        public (Node? nodeToUse, bool isGhost) GetOrInjectGhostNode(IField currentVehicleTile)
        {
            if (_graph.GetNodeAt(currentVehicleTile.X, currentVehicleTile.Y) is Node realNode)
            {
                return (realNode, false);
            }

            Node ghostNode = new(currentVehicleTile.X, currentVehicleTile.Y, currentVehicleTile.GetType());
            var ghostEdges = new List<Edge>();

            foreach ((int dx, int dy) dir in _directions)
            {
                int newX = currentVehicleTile.X + dir.dx;
                int newY = currentVehicleTile.Y + dir.dy;

                if (!IsValidExit(newX, newY)) continue;

                (Node? hitNode, List<IField> pathTaken) = WalkToNextJunction(currentVehicleTile, dir);
                if (hitNode is not null)
                {
                    ghostEdges.Add(new Edge(ghostNode, hitNode, pathTaken, pathTaken.Count));
                }
            }

            // Only add the ghost node if at least one connection is possible
            if (ghostEdges.Count == 0)
            {
                return (null, false);
            }

            _graph.AddNode(ghostNode);
            foreach (var edge in ghostEdges)
            {
                _graph.AddEdge(ghostNode, edge);
            }
            return (ghostNode, true);
        }

        /// <summary>
        /// Removes the specified ghost node from the graph.
        /// Should be called when the vehicle moves off the ghost node's tile or when the ghost node is no longer needed.
        /// </summary>
        /// <param name="ghostNode">The ghost node to remove.</param>
        public void RemoveGhostNode(Node ghostNode)
        {
            _graph.AdjacencyList.Remove(ghostNode);
        }
        #endregion

        #region Private methods
        private bool IsValidExit(int x, int y)
        {
            return _gameTable.IsInBounds(x, y)
                && _gameTable[x, y] is IInfrastructure;
        }

        private (Node? endNode, List<IField> path) WalkToNextJunction(IField startTile, (int X, int Y) initialMomentum)
        {
            List<IField> pathTaken = [startTile];

            IField currentTile = startTile;
            (int dx, int dy) momentum = initialMomentum;

            while (true)
            {
                int nextX = currentTile.X + momentum.dx;
                int nextY = currentTile.Y + momentum.dy;

                if (!_gameTable.IsInBounds(nextX, nextY)) return (null, pathTaken);

                currentTile = _gameTable[nextX, nextY];

                if (currentTile.X == startTile.X && currentTile.Y == startTile.Y)
                    return (null, pathTaken);

                pathTaken.Add(currentTile);
                if (_graph.GetNodeAt(nextX, nextY) is Node node) return (node, pathTaken);

                (int backDx, int backDy) = (-momentum.dx, -momentum.dy);
                (int dx, int dy) nextMomentum = (0, 0);
                int validExitsCount = 0;

                foreach (var dir in _directions)
                {
                    if (dir == (backDx, backDy)) continue;

                    int neighborX = currentTile.X + dir.dx;
                    int neighborY = currentTile.Y + dir.dy;

                    if (!IsValidExit(neighborX, neighborY)) continue;
                    validExitsCount++;
                    nextMomentum = dir;
                }

                if (validExitsCount == 1)
                {
                    momentum = nextMomentum;
                }
                else
                {
                    return (null, pathTaken);
                }
            }
        }
        #endregion
    }
}
