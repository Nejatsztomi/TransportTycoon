using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A shared road sequence between two edges in the graph.
    /// It is sequenced in the order of the starting edge to the finishing edge.
    /// </summary>
    public class SharedRoadSequence
    {
        #region Private fields
        /// <summary>
        /// The underlying data.
        /// </summary>
        private readonly IField[] _fields;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of this class with the specified collection of fields.
        /// </summary>
        /// <param name="fields">The list of fields to include in the sequence.</param>
        public SharedRoadSequence(List<IField> fields)
        {
            _fields = [.. fields];
        }
        #endregion

        #region Enumerators
        /// <summary>
        /// Returns an enumerable that iterates through the collection of fields in forward order.
        /// </summary>
        /// <returns>An enumerable for the collection of fields, starting from the first field and proceeding in order to the last field.</returns>
        public IEnumerable<IField> ForwardEnumerator()
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                yield return _fields[i];
            }
        }

        /// <summary>
        /// Returns an enumerable that iterates through the collection of fields in reversed order.
        /// </summary>
        /// <returns>An enumerable for the collection of fields, starting from the last field and proceeding in reverse order to the starting field.</returns>
        public IEnumerable<IField> BackwardEnumerator()
        {
            for (int i = _fields.Length - 1; i >= 0; i--)
            {
                yield return _fields[i];
            }
        }
        #endregion
    }
}
