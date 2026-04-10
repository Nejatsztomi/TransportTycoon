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
        /// The edge's starting vertex.
        /// </summary>
        public Node StartVertex { get; private set; }
        /// <summary>
        /// The edge's ending vertex.
        /// </summary>
        public Node EndVertex { get; private set; }
        /// <summary>
        /// List of the road tiles between the <see cref="StartVertex"/> and <see cref="EndVertex"/>.
        /// Also including the <see cref="StartVertex"/> and <see cref="EndVertex"/>.
        /// It is ordered from <see cref="StartVertex"/> to <see cref="EndVertex"/>.
        /// </summary>
        public List<Field> Roads { get; private set; }
        /// <summary>
        /// Gives the cost of the edge.
        /// </summary>
        /// <remarks>
        /// This is calculated my multiplying the number of road tiles with the <see cref="CostModifier"/>.
        /// </remarks>
        public double Cost => Roads.Count * CostModifier;
        #endregion

        #region Constructors
        public Edge(Node startVertex, Node endVertex, List<Field> roads)
        {
            StartVertex = startVertex;
            EndVertex = endVertex;
            Roads = roads;
        }
        #endregion
    }
}
