using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// Injects a ghost (temporary) node into the graph at the current position of the vehicle if there is no real node at that position.
    /// </summary>
    public class GhostNodeInjector
    {
        #region Private fields
        private static readonly (int dx, int dy)[] _directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];

        private readonly GameTable _gameTable;
        private readonly Graph _graph;
        #endregion

        #region Constructors
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
            var validExits = new List<(int X, int Y)>(4);
            GetValidExits(currentVehicleTile, validExits);

            var ghostEdges = new List<Edge>();
            foreach ((int x, int y) exit in validExits)
            {
                (Node? hitNode, List<IField> pathTaken) = WalkToNextJunction(currentVehicleTile, exit);
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

        public void RemoveGhostNode(Node ghostNode)
        {
            _graph.AdjacencyList.Remove(ghostNode);
        }
        #endregion

        #region Private methods
        private void GetValidExits(IField currentVehichleTile, List<(int dx, int dy)> validExits)
        {
            validExits.Clear();
            foreach ((int dx, int dy) in _directions)
            {
                int newX = currentVehichleTile.X + dx;
                int newY = currentVehichleTile.Y + dy;

                if (!_gameTable.IsInBounds(newX, newY)) continue;
                if (_gameTable[newX, newY] is not IInfrastructure) continue;

                validExits.Add((dx, dy));
            }
        }

        private (Node? endNode, List<IField> path) WalkToNextJunction(IField startTile, (int X, int Y) initialMomentum)
        {
            List<IField> pathTaken = [startTile];
            IField currentTile = startTile;
            var exits = new List<(int X, int Y)>(4);
            (int dx, int dy) momentum = initialMomentum;
            var visited = new HashSet<(int, int)> { (currentTile.X, currentTile.Y) };

            while (true)
            {
                int nextX = currentTile.X + momentum.dx;
                int nextY = currentTile.Y + momentum.dy;

                if (!_gameTable.IsInBounds(nextX, nextY))
                {
                    return (null, pathTaken);
                }

                // Prevent infinite loops by checking if we've already visited this tile
                if (!visited.Add((nextX, nextY)))
                {
                    return (null, pathTaken);
                }

                currentTile = _gameTable[nextX, nextY];
                pathTaken.Add(currentTile);
                if (_graph.GetNodeAt(nextX, nextY) is Node node)
                {
                    return (node, pathTaken);
                }

                (int backdx, int backdy) = (-momentum.dx, -momentum.dy);
                GetValidExits(currentTile, exits);
                exits.Remove((backdx, backdy));

                if (exits.Count == 1)
                {
                    momentum = exits[0];
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
