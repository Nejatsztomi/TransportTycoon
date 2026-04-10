using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A class for building the graph from the <see cref="GameTable"/>.
    /// </summary>
    public class GraphBuilder
    {
        #region Private fields
        /// <summary>
        /// A global, shared set of visited fields for the entire graph building process.
        /// </summary>
        private HashSet<Field> _visitedFields;
        #endregion

        #region Constructors
        public GraphBuilder()
        {
            _visitedFields = [];
        }
        #endregion

        #region Public methods
        // public Graph BuildGraph(GameTable table) { }
        #endregion
    }
}
