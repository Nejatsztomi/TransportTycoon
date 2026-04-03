using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.MapData
{
    public class GameTable
    {
        #region Constants
        public const int DefaultWidth = 100;
        public const int DefaultHeight = 100;
        #endregion

        #region Properties
        public Field[,] Table { get; private set; }
        public int Width { get; }
        public int Height { get; }

        public List<BuildingEntity> BuildingEntities { get; }

        public Field this[int x, int y]
        {
            get => Table[x, y];
            set => Table[x, y] = value;
        }

        private IMapGenerator MapGenerator { get; }
        private MapGenerationSettings GenerationSettings { get; }
        private MapGenerationContext GenerationContext { get; }
        #endregion

        #region Constructors
        public GameTable(int width, int height)
        {
            Width = width;
            Height = height;
            Table = new Field[width, height];

            BuildingEntities = [];

            GenerationSettings = new()
            {
                ForestPercentage = 0.4f,
                ForestNoiseScale = 0.1f,
                TerrainNoiseScale = 0.072f,
                WaterNoiseScale = 0.059f,
                MinCities = 2,
                MaxCities = 3,
                MinStructure = 6,
                MaxStructure = 8,
            };
            GenerationContext = new(width, height, 42, GenerationSettings);

            MapGenerator = MapGeneratorFactory.CreateMapGenerator(GenerationContext);
        }
        public GameTable() : this(DefaultWidth, DefaultHeight) { }
        #endregion

        #region Public methods
        public void GenerateMap()
        {
            Table = MapGenerator.GenerateMap(GenerationContext);
        }

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

        //Checks if the new field is possible
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
        public List<int> NeighbourRoadsCount(int x, int y)
        {
            List<int> result = new List<int> { 0, 0, 0, 0, 0 };//neighbour count,up,right,down,left
            if (x - 1 >= 0 && Table[x - 1, y] is Infrastructure) result[1] = 1;
            if (y + 1 <= Width - 1 && Table[x, y + 1] is Infrastructure) result[2] = 1;
            if (x + 1 <= Height - 1 && Table[x + 1, y] is Infrastructure) result[3] = 1;
            if (y - 1 >= 0 && Table[x, y - 1] is Infrastructure) result[4] = 1;

            result[0] = result.Count(x => x != 0);
            return result;
        }
        public List<(int, int)> NeighbourRoadsCoord(int x, int y)
        {
            List<(int, int)> result = new List<(int, int)>();
            if (x - 1 >= 0 && Table[x - 1, y] is Road) result.Add((Table[x - 1, y].X, Table[x - 1, y].Y));
            if (y + 1 <= Width - 1 && Table[x, y + 1] is Road) result.Add((Table[x, y + 1].X, Table[x, y + 1].Y));
            if (x + 1 <= Height - 1 && Table[x + 1, y] is Road) result.Add((Table[x + 1, y].X, Table[x + 1, y].Y));
            if (y - 1 >= 0 && Table[x, y - 1] is Road) result.Add((Table[x, y - 1].X, Table[x, y - 1].Y));
            return result;
        }
        public List<(int, int)> StopEnvironment(int x, int y)
        {
            List<(int, int)> result = NeighbourRoadsCoord(x, y);
            return result;
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
