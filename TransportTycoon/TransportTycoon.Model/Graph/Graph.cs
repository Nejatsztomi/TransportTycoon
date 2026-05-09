namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// The data structure for the graph representation of the <see cref="GameTable"/>.
    /// </summary>
    public class Graph
    {
        #region Properties
        /// <summary>
        /// The adjacency list representing the graph.
        /// </summary>
        public Dictionary<Node, List<Edge>> AdjacencyList { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Graph class with the specified adjacency list.
        /// </summary>
        /// <param name="adjacencyList">
        /// A dictionary representing the adjacency list of the graph,
        /// where each key is a <see cref="Node"/> and the value is a list of outgoing <see cref="Edge"/> from that node.
        /// </param>
        /// <remarks>
        /// Each value in the adjacency list is a list of outgoing edges from the corresponding node.
        /// Because of the game limitations, there will be at most 4 outgoing edges from a node.
        /// </remarks>
        public Graph(Dictionary<Node, List<Edge>> adjacencyList)
        {
            AdjacencyList = adjacencyList;
        }

        public Graph(List<Node> nodes, List<Edge> edges)
        {
            AdjacencyList = [];
            CreateGraphFromNodesNodesAndEdges(nodes, edges);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Retrieves the node located at the specified X and Y coordinates, if it exists.
        /// </summary>
        /// <param name="x">The X-coordinate of the node to retrieve.</param>
        /// <param name="y">The Y-coordinate of the node to retrieve.</param>
        /// <returns>The <see cref="Node"/> at the specified coordinates, or <see langword="null"/> if no such node exists.</returns>
        public Node? GetNodeAt(int x, int y)
        {
            return AdjacencyList.Keys.FirstOrDefault(node => node.X == x && node.Y == y);
        }

        /// <summary>
        /// Adds a node to the graph if it does not already exist in the adjacency list.
        /// </summary>
        /// <param name="node">The node to add to the graph.</param>
        public void AddNode(Node node)
        {
            if (!AdjacencyList.ContainsKey(node))
            {
                AdjacencyList[node] = [];
            }
        }

        /// <summary>
        /// Adds an edge to the adjacency list for the specified node if the node exists in the graph.
        /// </summary>
        /// <remarks>
        /// If the specified node does not exist in the adjacency list, the method does nothing.
        /// This method does not check for duplicate edges.
        /// </remarks>
        /// <param name="node">The node to which the edge will be added.</param>
        /// <param name="edge">The edge to add to the specified node's adjacency list.</param>
        public void AddEdge(Node node, Edge edge)
        {
            if (AdjacencyList.ContainsKey(node))
            {
                AdjacencyList[node].Add(edge);
            }
        }

        /// <summary>
        /// Determines whether the graph contains the specified node.
        /// </summary>
        /// <param name="node">The node to check for existence in the graph.</param>
        /// <returns><see langword="true"/> if the graph contains the specified node; otherwise, <see langword="false"/>.</returns>
        public bool ContainsNode(Node node)
        {
            return AdjacencyList.ContainsKey(node);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates a directed, weighted graph from the given nodes and edges.
        /// </summary>
        /// <param name="nodes">The graph's nodes.</param>
        /// <param name="edges">The graph's directed and weighted edges.</param>
        private void CreateGraphFromNodesNodesAndEdges(List<Node> nodes, List<Edge> edges)
        {
            foreach (Node node in nodes)
            {
                AdjacencyList[node] = [];
            }

            foreach (Edge edge in edges)
            {
                AdjacencyList[edge.StartNode].Add(edge);
                AdjacencyList[edge.EndNode].Add(edge);
            }
        }
        #endregion
    }
}
