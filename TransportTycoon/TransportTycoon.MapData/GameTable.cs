using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

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
                WaterBiome = WaterBiomes.Dry,
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
        public int CreateHorizontalBridge(int x, int a, int b, BridgeType b_type, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            for (int i = a; i <= b; i++)
            {
                switch (b_type)
                {
                    case BridgeType.HorizontalYellowBridge:
                        Table[x, i] = new YellowBridge(x, i, b_type, Table[x, i].Height);
                        break;
                    case BridgeType.HorizontalGreenBridge:
                        Table[x, i] = new GreenBridge(x, i, b_type, Table[x, i].Height);
                        break;
                    case BridgeType.HorizontalRedBridge:
                        Table[x, i] = new RedBridge(x, i, b_type, Table[x, i].Height);
                        break;
                }
                changedFields.Add((x, i));
                cost += ((Bridge)Table[x, i]).Price;
            }
            if (Table[x, a - 1] is Road road1)
            {
                road1.ChangeType(CalculateRoadType(x, a - 1));
                changedFields.Add((x, a - 1));
            }
            if (Table[x, b + 1] is Road road2)
            {
                road2.ChangeType(CalculateRoadType(x, b + 1));
                changedFields.Add((x, b + 1));
            }
            return cost;
        }
        public int CreateVerticalBridge(int y, int a, int b, BridgeType b_type, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            for (int i = a; i <= b; i++)
            {
                switch (b_type)
                {
                    case BridgeType.VerticalYellowBridge:
                        Table[i, y] = new YellowBridge(i, y, b_type, Table[i, y].Height);
                        break;
                    case BridgeType.VerticalGreenBridge:
                        Table[i, y] = new GreenBridge(i, y, b_type, Table[i, y].Height);
                        break;
                    case BridgeType.VerticalRedBridge:
                        Table[i, y] = new RedBridge(i, y, b_type, Table[i, y].Height);
                        break;
                    default:
                        break;
                }
                changedFields.Add((i, y));
                cost += ((Bridge)Table[i, y]).Price;
            }
            if (Table[a - 1, y] is Road road1)
            {
                road1.ChangeType(CalculateRoadType(a - 1, y));
                changedFields.Add((a - 1, y));
            }
            if (Table[b + 1, y] is Road road2)
            {
                road2.ChangeType(CalculateRoadType(b + 1, y));
                changedFields.Add((b + 1, y));
            }
            return cost;
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
