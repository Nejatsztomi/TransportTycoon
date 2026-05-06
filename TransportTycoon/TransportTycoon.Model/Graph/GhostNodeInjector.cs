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

        private readonly Graph _graph;
        private readonly PathTracer _pathTracer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GhostNodeInjector"/> class with the specified <see cref="PathTracer"/> and <see cref="Graph"/>.
        /// </summary>
        /// <param name="graph">The <see cref="Graph"/> in which ghost nodes will be injected. Cannot be <see langword="null"/>.</param>
        /// <param name="pathTracer">The <see cref="PathTracer"/> used to trace paths from the ghost node to real nodes. Cannot be <see langword="null"/>.</param>
        public GhostNodeInjector(Graph graph, PathTracer pathTracer)
        {
            _graph = graph;
            _pathTracer = pathTracer;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// If there is a real node at the current vehicle tile, return it.
        /// Otherwise, inject a ghost node and connect it to all reachable junctions, then return the ghost node.
        /// </summary>
        /// <param name="currentTile">The current tile of the vehicle.</param>
        /// <returns>A tuple containing the <see cref="Node"/> to use and a <see langword="bool"> indicating if it is a ghost node.</returns>
        public (Node? nodeToUse, bool isGhost) GetOrInjectGhostNode(IField currentTile)
        {
            if (_graph.GetNodeAt(currentTile.X, currentTile.Y) is Node realNode) return (realNode, false);

            Node ghostNode = new(currentTile.X, currentTile.Y, currentTile.GetType());
            var ghostEdges = new List<Edge>();

            foreach ((int dx, int dy) dir in _directions)
            {
                var result = _pathTracer.TraceSegment(currentTile, dir);

                if (result.Status != TraceStatus.FoundIntersection) continue;
                var endTile = result.EndTile;
                if (endTile is null) continue;

                if (_graph.GetNodeAt(endTile.X, endTile.Y) is not Node destinationNode) continue;

                ghostEdges.Add(new Edge(ghostNode, destinationNode, result.PathTaken, result.ForwardCost));
            }

            // Only add the ghost node if at least one connection is possible
            if (ghostEdges.Count == 0) return (null, false);

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
    }
}
