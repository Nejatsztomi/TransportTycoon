using TransportTycoon.Model.MapGenerator;

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

        private IMapGenerator MapGenerator { get; }
        #endregion

        #region Constructors
        public GameTable(int width, int height)
        {
            Width = width;
            Height = height;
            Table = new Field[width, height];

            Pointers = [];
            BuildingIDs = [];

            MapGenerator = PerlinNoiseMapGeneratorFactory.Create(width, height, 0);
        }
        public GameTable() : this(DefaultWidth, DefaultHeight) { }
        #endregion

        #region Public methods
        public List<Field> CheckNeighboringTrees(int x, int y)
        {
            List<Field> neighbours = new List<Field>();
            List<Field> acceptedNeighbours = new List<Field>();
            if (x - 1 >= 0) neighbours.Add(Table[x - 1, y]);
            if (y + 1 <= Width - 1) neighbours.Add(Table[x, y + 1]);
            if (x + 1 <= Height - 1) neighbours.Add(Table[x + 1, y]);
            if (y - 1 >= 0) neighbours.Add(Table[x, y - 1]);
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i] is Terrain terrain && terrain.Trees == 0) acceptedNeighbours.Add(neighbours[i]);
            }
            return acceptedNeighbours;
        }

        public void GenerateMap()
        {
            int[,] randomMap = MapGenerator.GenerateMap(0.1f);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    switch (randomMap[i, j])
                    {
                        case 0:
                            Table[i, j] = new Water(i, j);
                            break;
                        case 1:
                            Table[i, j] = new Plain(i, j);
                            break;
                        case 2:
                            Table[i, j] = new Hill(i, j);
                            break;
                        case 3:
                            Table[i, j] = new Mountin(i, j);
                            break;
                        case 4:
                            Table[i, j] = new HightMountin(i, j);
                            break;
                    }
                }
            }
        }

        public bool IsTileHeightPossible(int x, int y, int height)
        {
            if (x < 0 || x >= Height || y < 0 || y >= Width) return false;

            //Up
            if (x > 0 && Math.Abs(height - Table[x - 1, y].Height) > 2) return false;

            // Down 
            if (x < Height - 1 && Math.Abs(height - Table[x + 1, y].Height) > 2) return false;

            // Left
            if (y > 0 && Math.Abs(height - Table[x, y - 1].Height) > 2) return false;

            // Right
            if (y < Width - 1 && Math.Abs(height - Table[x, y + 1].Height) > 2) return false;

            return true;
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
