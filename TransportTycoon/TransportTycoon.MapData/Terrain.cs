namespace TransportTycoon.MapData
{
    /// <summary>
    /// Specifies the types of terrain available for classification or processing.
    /// </summary>
    /// <remarks>Use this enumeration to represent or distinguish between different terrain categories, such
    /// as plains, hills, and mountains, in mapping, simulation, or geographic applications. The values are ordered by
    /// increasing elevation and ruggedness.</remarks>
    public enum TerrainType
    {
        Plain = 1,
        Hill = 2,
        Mountain = 3,
        HighMountain = 4,
    }

    /// <summary>
    /// Represents a terrain field with configurable height, tree count, and terrain type properties.
    /// </summary>
    /// <remarks>The Terrain class provides methods and properties to manage the state of a terrain field,
    /// including its position, height, number of trees, and terrain type. It supports operations to increase or
    /// decrease height within defined limits and to manage tree growth. The class exposes read-only properties to
    /// indicate whether the terrain is modifiable and whether it has reached its maximum tree capacity. Use this class
    /// to model and manipulate terrain elements within a field-based environment.</remarks>
    public class Terrain : Field
    {
        #region Static fields
        /// <summary>
        /// Represents the default price value used by the application.
        /// </summary>
        public static readonly int Price = 200;
        #endregion

        #region Public properties
        public override bool Modifiable => true;

        /// <summary>
        /// Gets or sets the number of trees.
        /// </summary>
        public int Trees { set; get; }

        /// <summary>
        /// Gets or sets the type of terrain associated with this instance.
        /// </summary>
        public TerrainType TerrainType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the collection contains the maximum number of trees.
        /// </summary>
        public bool IsFull => Trees == 4;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Terrain class with the specified coordinates and height.
        /// </summary>
        /// <param name="x">The X-coordinate of the terrain location.</param>
        /// <param name="y">The Y-coordinate of the terrain location.</param>
        /// <param name="height">The height value to assign to the terrain.</param>
        public Terrain(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
            Trees = 0;
            SetFieldType();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Increases the height of the terrain by one unit, up to a maximum value of 4.
        /// </summary>
        /// <remarks>If the height is already at its maximum value of 4, this method has no effect.
        /// Invokes the field type update to reflect the new height.</remarks>
        public void IncreaseHeight()
        {
            if (Height == 4) return;
            Height++;
            SetFieldType();
        }

        /// <summary>
        /// Decreases the height of the object by one unit, ensuring that the height does not fall below one.
        /// </summary>
        /// <remarks>If the height is already at its minimum value of one, this method performs no action.
        /// After decreasing the height, the field type is updated to reflect the new state.</remarks>
        public void DecreaseHeight()
        {
            if (Height == 1) return;
            Height--;
            SetFieldType();
        }

        /// <summary>
        /// Attempts to increase the number of trees by one, if the current capacity allows.
        /// </summary>
        /// <remarks>This method checks whether the collection has reached its maximum capacity before
        /// incrementing the tree count. Use this method to safely add a tree without exceeding the allowed
        /// limit.</remarks>
        /// <returns>true if the tree count was successfully increased; otherwise, false if the capacity is already full.</returns>
        public bool Grow()
        {
            if (IsFull) return false;
            Trees++;
            return true;
        }

        /// <summary>
        /// Set the field's number of trees to 1
        /// </summary>
        public void SpreadForest()
        {
            Trees = 1;
        }

        /// <summary>
        /// Gets the number of trees associated with the current instance.
        /// </summary>
        /// <returns>The number of trees as an integer.</returns>
        public override int GetTrees()
        {
            return Trees;
        }
        #endregion

        #region Private methods
        private void SetFieldType()
        {
            if (Height == 1) TerrainType = TerrainType.Plain;
            else if (Height == 2) TerrainType = TerrainType.Hill;
            else if (Height == 3) TerrainType = TerrainType.Mountain;
            else TerrainType = TerrainType.HighMountain;
        }
        #endregion
    }
}
