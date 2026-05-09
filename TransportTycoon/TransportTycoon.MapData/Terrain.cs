namespace TransportTycoon.MapData
{
    public enum TerrainType
    {
        Plain = 1, Hill = 2, Mountain = 3, HighMountain = 4
    }
    public struct Terrain : IField
    {
        #region Fields
        public int Trees { set; get; }
        public readonly bool Modifiable => true;
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public TerrainType TerrainType { get; set; }
        public readonly bool IsFull => Trees == 4;

        static public readonly int Price = 200;
        #endregion

        public Terrain(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
            Trees = 0;
            SetFieldType();
        }

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
        /// Gets the total number of trees in the terrain.
        /// </summary>
        /// <returns>The total number of trees as an integer.</returns>
        public readonly int GetTrees() => Trees;
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
