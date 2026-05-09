namespace TransportTycoon.MapData
{
    public interface IField
    {
        #region Properties
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int Height { get; protected set; }

        public virtual bool Modifiable
        {
            get => true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Retrieves the number of trees on the field. This method can be overridden by implementing classes to provide the actual number of trees.
        /// </summary>
        /// <returns>The number of trees on the field.</returns>
        public virtual int GetTrees() => 0;
        #endregion
    }

    public struct Water : IField
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly bool Modifiable => false;
        #endregion

        public Water(int x, int y)
        {
            X = x;
            Y = y;
            Height = 0;
        }
    }
}
