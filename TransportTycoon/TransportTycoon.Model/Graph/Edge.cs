using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    public class Edge
    {
        #region Public constants
        /// <summary>
        /// A cost modifier for calculating the cost of an edge.
        /// </summary>
        /// <remarks>
        /// This will likely to change in the future for a more dynamic one.
        /// Which includes bridge speed limits and terrain heights.
        /// </remarks>
        public const double CostModifier = 2.0;
        #endregion

        #region Properties
        /// <summary>
        /// The edge's starting node.
        /// </summary>
        public Node StartNode { get; private set; }
        /// <summary>
        /// The edge's ending node.
        /// </summary>
        public Node EndNode { get; private set; }
        /// <summary>
        /// An enumerable collection of road tiles between the <see cref="StartNode"/> and <see cref="EndNode"/>.
        /// Also including the <see cref="StartNode"/> and <see cref="EndNode"/>.
        /// It is ordered from <see cref="StartNode"/> to <see cref="EndNode"/>.
        /// </summary>
        public IEnumerable<Field> Roads { get; private set; }
        /// <summary>
        /// Gives the cost of the edge.
        /// </summary>
        public double Cost { get; }
        #endregion

        #region Constructors
        public Edge(Node startNode, Node endNode, IEnumerable<Field> roads, double cost)
        {
            StartNode = startNode;
            EndNode = endNode;
            Roads = roads;
            Cost = cost;
        }
        #endregion
    }
}
