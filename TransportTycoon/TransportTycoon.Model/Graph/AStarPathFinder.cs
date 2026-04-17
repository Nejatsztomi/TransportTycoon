namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A record representing a key for caching routes between two nodes in the graph.
    /// It consists of a start node and an end node.
    /// </summary>
    /// <param name="Start">The starting node of the route.</param>
    /// <param name="End">The ending node of the route.</param>
    public record RouteKey(Node Start, Node End);

    /// <summary>
    /// A path finder class that implements the A* algorithm to find the shortest path between two nodes in a graph.
    /// </summary>
    public class AStarPathfinder : IPathFinder
    {
        #region Private fields
        private readonly Graph _graph;
        /// <summary>
        /// The cache of routes.
        /// </summary>
        private readonly Dictionary<RouteKey, List<Edge>> _routeCache = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the AStarPathfinder class with the specified graph.
        /// </summary>
        /// <param name="graph">The graph to traverse.</param>
        public AStarPathfinder(Graph graph)
        {
            _graph = graph;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Return the shortest path between the start <see cref="Node"/> and the end <see cref="Node"/> as a list of <see cref="Edge"/> objects.
        /// Uses caching for already exisiting paths to improve performance.
        /// If the path is not found in the cache, it runs the A* algorithm to find the path and stores it in the cache before returning it.
        /// </summary>
        /// <param name="startNode">The starting node of the path.</param>
        /// <param name="endNode">The ending node of the path.</param>
        /// <returns>A list of <see cref="Edge"/> objects representing the shortest path, or <see langword="null"/> if no path is found.</returns>
        public List<Edge>? FindPath(Node startNode, Node endNode)
        {
            RouteKey key = new(startNode, endNode);

            // Try to get the route from the cache first
            if (_routeCache.TryGetValue(key, out List<Edge>? cachedRoute))
            {
                return cachedRoute;
            }

            // Try the A* algorithm
            List<Edge>? path = RunAStarAlgorithm(startNode, endNode);

            // If we found a path, cache it for future use
            if (path is not null)
            {
                _routeCache[key] = path;
            }
            return path;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Finds the shortest path from the start <see cref="Node"/> to the end <see cref="Node"/> using the A* algorithm.
        /// The algorithm's pseudocode: <seealso href="https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode"/>
        /// </summary>
        /// <param name="startNode">The starting node.</param>
        /// <param name="endNode">The ending node.</param>
        /// <returns>A list of <see cref="Edge"/> objects representing the shortest path from the start <see cref="Node"/> to the end <see cref="Node"/>, or <see langword="null"/> if no path is found.</returns>
        private List<Edge>? RunAStarAlgorithm(Node startNode, Node endNode)
        {
            PriorityQueue<Node, double> openSet = new();
            Dictionary<Node, Edge> cameFrom = [];
            Dictionary<Node, double> gScores = [];

            gScores[startNode] = 0;
            // node, f(n)
            openSet.Enqueue(startNode, CalculateHeuristic(startNode, endNode));

            while (openSet.Count > 0)
            {
                Node current = openSet.Dequeue();

                if (current.Equals(endNode))
                {
                    return ReconstructPath(cameFrom, endNode);
                }

                if (!_graph.AdjacencyList.TryGetValue(current, out List<Edge>? neighbours))
                {
                    continue;
                }

                foreach (Edge edge in neighbours)
                {
                    Node neighbourNode = edge.EndNode;

                    // g(n) - Dijsktra's cost
                    double tentativeGScore = gScores[current] + edge.Cost;

                    if (!gScores.TryGetValue(neighbourNode, out double currentGScore) || tentativeGScore < currentGScore)
                    {
                        cameFrom[neighbourNode] = edge;
                        gScores[neighbourNode] = tentativeGScore;

                        // f(n) = g(n) + h(n)
                        double fScore = tentativeGScore + CalculateHeuristic(neighbourNode, endNode);

                        // node, f(n)
                        openSet.Enqueue(neighbourNode, fScore);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates the heuristic (estimated cost) from the current node to the goal node.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="goal">The goal node.</param>
        /// <returns>The estimated cost from the current node to the goal node.</returns>
        private double CalculateHeuristic(Node current, Node goal)
        {
            int dx = Math.Abs(current.X - goal.X);
            int dy = Math.Abs(current.Y - goal.Y);
            return (dx + dy) * 0.8;
        }

        /// <summary>
        /// Reconstructs the path from the start node to the goal node using the "cameFrom" dictionary.
        /// </summary>
        /// <param name="cameFrom">A dictionary mapping each node to the edge that led to it.</param>
        /// <param name="endNode">The end node of the path.</param>
        /// <returns>A list of edges representing the path from the start node to the goal node.</returns>
        private List<Edge> ReconstructPath(Dictionary<Node, Edge> cameFrom, Node endNode)
        {
            List<Edge> path = [];

            while (cameFrom.TryGetValue(endNode, out Edge? edge))
            {
                path.Add(edge);
                endNode = edge.StartNode;
            }

            path.Reverse();
            return path;
        }
        #endregion
    }
}
