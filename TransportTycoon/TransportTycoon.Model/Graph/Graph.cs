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
        #endregion
    }
}
