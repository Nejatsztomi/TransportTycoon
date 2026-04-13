namespace TransportTycoon.Model.Graph
{
    public class AStarPathfinder
    {
        #region Private fields
        private readonly Graph _graph;
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
        /// Finds the shortest path from the start node to the end node using the A* algorithm.
        /// The algorithm's pseudocode: <seealso href="https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode"/>
        /// </summary>
        /// <param name="startNode">The starting node.</param>
        /// <param name="endNode">The ending node.</param>
        /// <returns>A list of edges representing the shortest path from the start node to the end node, or null if no path is found.</returns>
        public List<Edge>? FindPath(Node startNode, Node endNode)
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
        #endregion

        #region Private methods
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
