using System.Text.RegularExpressions;

namespace TransportTycoon.MapData
{
    public class GameTable
    {
        #region Private constants
        private const int DefaultWidth = 100;
        private const int DefaultHeight = 100;
        #endregion

        #region Properties
        public Field[,] Table { get; }
        public int Width { get; }
        public int Height { get; }

        public List<(int, int)> Pointers { get; }
        public List<(int, int)> BuildingIDs { get; }



        public Field this[int x, int y]
        {
            get => Table[x, y];
            set => Table[x, y] = value;
        }
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
                    Table[i, j] = new Plain(i, j);
                }
            }
        }
        public bool IsTileHeightPossible(int x, int y, int height)
        {
            if (x < 0 || x >= Height || y < 0 || y >= Width) return false;

            bool isValid = true;

            //Up
            if (x > 0 && Math.Abs(height - Table[x - 1, y].Height) > 2) return false;

            // Down 
            if (x < Height - 1 && Math.Abs(height - Table[x + 1, y].Height) > 2) return false;

            // Left
            if (y > 0 && Math.Abs(height - Table[x, y - 1].Height) > 2) return false;

            // Right
            if (y < Width - 1 && Math.Abs(height - Table[x, y + 1].Height) > 2) return false;


            return isValid;

        }
        #endregion

        #region Private methods


        private bool IsMapAccurate() 
        {
            bool isAccurate = true;
            for (int i = 0; i < Table.GetLength(0); i++)
            {
                for (int j = 0; j < Table.GetLength(1); j++)
                {
                    isAccurate = isAccurate && IsTileHeightPossible(i, j, Table[i, j].Height);
                }
            }
            return isAccurate;
        }

        


        #endregion
    }
}
