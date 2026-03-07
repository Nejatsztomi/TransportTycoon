namespace TransportTycoon.MapData
{
    public class GameTable
    {
        #region Private constants
        private const int DefaultWidth = 100;
        private const int DefaultHeight = 100;
        #endregion

        #region Fields
        public Field[,] Table { get; }
        public int Width { get; }
        public int Height { get; }

        public List<(int, int)> Pointers { get; }
        public List<(int, int)> BuildingIDs { get; }
        #endregion

        #region Constructors
        public GameTable(int width, int height)
        {
            Width = width;
            Height = height;
            Table = new Field[width, height];

            Pointers = [];
            BuildingIDs = [];
        }

        public GameTable() : this(DefaultWidth, DefaultHeight) { }
        #endregion

        #region Public methods
        // TODO: Megcsinálni
        public void CheckNeighboringTrees(int x, int y) { }
        #endregion

        #region Public methods
        public void GenerateMap()
        {
            for (int i = 0; i < Table.GetLength(0); i++)
            {
                for (int j = 0; j < Table.GetLength(1); j++)
                {
                    Table[i, j] = new Plain(i, j, 0, 0);
                }
            }
        }
        #endregion

        #region Private methods
        #endregion
    }
}
