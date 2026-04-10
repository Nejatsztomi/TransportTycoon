using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A class representing a node in the graph.
    /// </summary>
    public class Node
    {
        #region Properties
        /// <summary>
        /// The X coordinate of the node on the map.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The Y coordinate of the node on the map.
        /// </summary>
        public int Y { get; private set; }
        /// <summary>
        /// The node's fieldtype on the map.
        /// </summary>
        /// <remarks>
        /// Can be either <see cref="FieldType.Stop"/> or <see cref="FieldType.Road"/>.
        /// </remarks>
        public FieldType Type { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this field is a valid destination for a vehicle.
        /// </summary>
        /// <remarks>
        /// This indicates that the path finding algorithms can consider this node as a valid destination for a vehicle.
        /// </remarks>
        public bool IsValidDestination => Type == FieldType.Stop;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of this class with the specified coordinates and <see cref="FieldType"/>.
        /// </summary>
        /// <param name="x">The X coordinate of the node.</param>
        /// <param name="y">The Y coordinate of the node.</param>
        /// <param name="type">The type of field represented by the node.</param>
        public Node(int x, int y, FieldType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        #endregion
    }
}
