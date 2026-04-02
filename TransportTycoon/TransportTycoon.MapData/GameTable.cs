using System.ComponentModel.Design;
using System.Diagnostics;
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

        private INoiseGenerator NoiseGenerator { get; }
        #endregion

        #region Constructors
        public GameTable(int width, int height)
        {
            Width = width;
            Height = height;
            Table = new Field[width, height];

            Pointers = [];
            BuildingIDs = [];

            NoiseGenerator = PerlinNoiseGeneratorFactory.Create(width, height, 0);
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
            float[,] randomMap = NoiseGenerator.GenerateNoise(0.1f);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (randomMap[i, j] < 0.35f)
                    {
                        Table[i, j] = new Water(i, j);          // Bottom 35% of heights become water
                    }
                    else if (randomMap[i, j] < 0.55f)
                    {
                        Table[i, j] = new Terrain(i, j, 1);          // Next 20% become plains
                    }
                    else if (randomMap[i, j] < 0.75f)
                    {
                        Table[i, j] = new Terrain(i, j, 2);          // Next 20% become hills
                    }
                    else if (randomMap[i, j] < 0.90f)
                    {
                        Table[i, j] = new Terrain(i, j, 3);      // Next 15% become mountains
                    }
                    else
                    {
                        Table[i, j] = new Terrain(i, j, 4);  // Top 10% become high mountains
                    }
                }
            }

            GenerateTrees();
        }

        public void GenerateTrees()
        {
            float[,] randomTreeMap = NoiseGenerator.GenerateNoise(0.1f);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Table[i, j] is not Terrain terrain) continue;
                    if (terrain.FieldType == FieldType.HighMountain) continue;

                    if (randomTreeMap[i, j] < 0.5f) continue;

                    if (randomTreeMap[i, j] < 0.75f)
                    {
                        terrain.Trees = 1;
                    }
                    else if (randomTreeMap[i, j] < 0.85f)
                    {
                        terrain.Trees = 2;
                    }
                    else if (randomTreeMap[i, j] < 0.95f)
                    {
                        terrain.Trees = 3;
                    }
                    else
                    {
                        terrain.Trees = 4;
                    }
                }
            }
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
        public List<Field?> NeighboursOfRoadsAndStops(int x, int y)
        {
            List<Field?> result = new() { null, null, null, null };
            if (x - 1 >= 0 && HeightCheck(Table[x - 1, y], Table[x, y]))
            {
                if (Table[x - 1, y] is Bridge bridge && bridge.BridgeType.ToString().Contains("Vertical")) result[0] = Table[x - 1, y];
                else if (Table[x - 1, y] is Road || Table[x - 1, y] is Stop) result[0] = Table[x - 1, y];
            }
            if (y + 1 <= Width - 1 && HeightCheck(Table[x, y + 1], Table[x, y]))
            {
                if (Table[x, y + 1] is Bridge bridge && bridge.BridgeType.ToString().Contains("Horizontal")) result[1] = Table[x, y + 1];
                else if (Table[x, y + 1] is Road || Table[x, y + 1] is Stop) result[1] = Table[x, y + 1];
            }
            if (x + 1 <= Height - 1 && HeightCheck(Table[x + 1, y], Table[x, y]))
            {
                if (Table[x + 1, y] is Bridge bridge && bridge.BridgeType.ToString().Contains("Vertical")) result[2] = Table[x + 1, y];
                else if (Table[x + 1, y] is Road || Table[x + 1, y] is Stop) result[2] = Table[x + 1, y];
            }
            if (y - 1 >= 0 && HeightCheck(Table[x, y - 1], Table[x, y]))
            {
                if (Table[x, y - 1] is Bridge bridge && bridge.BridgeType.ToString().Contains("Horizontal")) result[3] = Table[x, y - 1];
                else if (Table[x, y - 1] is Road || Table[x, y - 1] is Stop) result[3] = Table[x, y - 1];
            }
            return result;
        }
        public bool HeightCheck(Field a, Field b)
        {
            return Math.Abs(a.Height - b.Height) <= 1;
        }
        public RoadType CalculateRoadType(int x, int y)
        {
            List<Field?> neighbourRoads = NeighboursOfRoadsAndStops(x, y);
            RoadType type = RoadType.Vertical;
            switch (neighbourRoads.Count(x => x != null))
            {
                case 1:
                    if (neighbourRoads[1] != null || neighbourRoads[3] != null) type = RoadType.Horizontal;
                    break;
                case 2:
                    if (neighbourRoads[1] != null && neighbourRoads[3] != null) type = RoadType.Horizontal;
                    else if (neighbourRoads[0] != null && neighbourRoads[1] != null) type = RoadType.UpperRightTurn;
                    else if (neighbourRoads[1] != null && neighbourRoads[2] != null) type = RoadType.RightTurn;
                    else if (neighbourRoads[2] != null && neighbourRoads[3] != null) type = RoadType.LeftTurn;
                    else if (neighbourRoads[3] != null && neighbourRoads[0] != null) type = RoadType.UpperLeftTurn;
                    break;
                case 3:
                    int noNeighbour = neighbourRoads.FindIndex(x => x == null);
                    switch (noNeighbour)
                    {
                        case 0:
                            type = RoadType.DownTRoad;
                            break;
                        case 1:
                            type = RoadType.LeftTRoad;
                            break;
                        case 2:
                            type = RoadType.UpperTRoad;
                            break;
                        case 3:
                            type = RoadType.RightTRoad;
                            break;
                        default:
                            break;
                    }
                    break;
                case 4:
                    type = RoadType.XRoad;
                    break;
                default:
                    break;
            }
            return type;
        }
        public BridgeType CalculateBridgeType(int dif, string dir)
        {
            if (dir == "horizontal")
            {
                if (dif <= 13) return BridgeType.HorizontalYellowBridge;
                else if (dif <= 15) return BridgeType.HorizontalGreenBridge;
                else if (dif <= 17) return BridgeType.HorizontalRedBridge;
                else return BridgeType.Null;
            }
            else
            {
                if (dif <= 13) return BridgeType.VerticalYellowBridge;
                else if (dif <= 15) return BridgeType.VerticalGreenBridge;
                else if (dif <= 17) return BridgeType.VerticalRedBridge;
                else return BridgeType.Null;
            }
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
