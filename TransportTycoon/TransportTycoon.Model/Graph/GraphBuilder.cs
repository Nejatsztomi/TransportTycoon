using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    public class GraphBuilder
    {
        #region Private fields
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
