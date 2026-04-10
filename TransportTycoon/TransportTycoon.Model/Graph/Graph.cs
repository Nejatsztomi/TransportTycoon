namespace TransportTycoon.Model.Graph
{
    public class Graph
    {
        #region Properties
        public Dictionary<Node, List<Edge>> AdjacencyList { get; private set; }
        #endregion

        #region Constructors
        public Graph(Dictionary<Node, List<Edge>> adjacencyList)
        {
            AdjacencyList = adjacencyList;
        }
        #endregion
    }
}
