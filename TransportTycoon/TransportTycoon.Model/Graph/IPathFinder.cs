namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// An interface for pathfinding algorithms that find the shortest path between two nodes in a graph.
    /// </summary>
    public interface IPathFinder
    {
        #region Public methods
        /// <summary>
        /// Tries to find the shortest path between the start and end nodes.
        /// The path finding algorithm should work on directed, weighted graphs, where the edges have a cost associated with them.
        /// </summary>
        /// <param name="startNode">The starting <see cref="Node"/> of the path.</param>
        /// <param name="endNode">The ending <see cref="Node"/> of the path.</param>
        /// <returns>
        /// If a path is found, it returns a list of <see cref="Edge"/> that represent the path.
        /// If no path is found, it returns <see langword="null"/>.
        /// </returns>
        public List<Edge>? FindPath(Node startNode, Node endNode);
        #endregion
    }
}
