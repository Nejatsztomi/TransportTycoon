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
        public int Width => Context.Width;
        public int Height => Context.Height;

        public List<BuildingEntity> BuildingEntities { get; }

        public Field this[int x, int y]
        {
            get => Table[x, y];
            set => Table[x, y] = value;
        }

        /// <summary>
        /// Tells whether the map has been generated or not.
        /// This is used to prevent accessing the map before it is generated, which can cause errors.
        /// </summary>
        public bool IsMapGenerated { get; private set; }
        private IMapGenerator MapGenerator { get; }
        public MapGenerationContext Context { get; set; }
        private MapGenerationSettings GenerationSettings => Context.Settings;
        #endregion

        #region Constructors
        public GameTable(IMapGenerator mapGenerator, MapGenerationContext context)
        {
            Context = context;
            BuildingEntities = [];

            Table = new Field[Width, Height];
            IsMapGenerated = false;
            MapGenerator = mapGenerator;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the specified coordinates are within the valid bounds defined by the current height and
        /// width.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns><see langword="true"/> if both coordinates are within bounds; otherwise, <see langword="false"/>.</returns>
        public bool IsInBounds(int x, int y) => 0 <= x && x < Height && 0 <= y && y < Width;

        public void GenerateMap()
        {
            Table = MapGenerator.GenerateMap(Context);
            IsMapGenerated = true;
        }

        public List<Field> CheckNeighboringTrees(int x, int y)
        {
            List<Field> neighbours = [];
            List<Field> acceptedNeighbours = [];
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

            // up 
            if (x < Height - 1 && Math.Abs(height - Table[x + 1, y].Height) > 2) return false;

            // Left
            if (y > 0 && Math.Abs(height - Table[x, y - 1].Height) > 2) return false;

            // Right
            if (y < Width - 1 && Math.Abs(height - Table[x, y + 1].Height) > 2) return false;

            return true;
        }
        public List<Field?> NeighboursOfRoadsAndStops(int x, int y)
        {
            List<Field?> result = [null, null, null, null];
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
            switch (neighbourRoads.Count(x => x is not null))
            {
                case 1:
                    if (neighbourRoads[1] is not null || neighbourRoads[3] is not null) type = RoadType.Horizontal;
                    break;
                case 2:
                    if (neighbourRoads[1] is not null && neighbourRoads[3] is not null) type = RoadType.Horizontal;
                    else if (neighbourRoads[0] is not null && neighbourRoads[1] is not null) type = RoadType.UpperRightTurn;
                    else if (neighbourRoads[1] is not null && neighbourRoads[2] is not null) type = RoadType.RightTurn;
                    else if (neighbourRoads[2] is not null && neighbourRoads[3] is not null) type = RoadType.LeftTurn;
                    else if (neighbourRoads[3] is not null && neighbourRoads[0] is not null) type = RoadType.UpperLeftTurn;
                    break;
                case 3:
                    int noNeighbour = neighbourRoads.FindIndex(x => x is null);
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
        public int CreateShortBridge(int x, int y, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            if (x - 1 < 0 || y - 1 < 0 || x + 1 > Height - 1 || y + 1 > Width - 1) return 0;
            if ((Table[x, y - 1] is Infrastructure && Table[x, y - 1].Height == 1 && Table[x, y + 1].Height == 1)
                || (Table[x, y + 1] is Infrastructure && Table[x, y + 1].Height == 1 && Table[x, y - 1].Height == 1))
            {
                cost = CreateHorizontalBridge(x, y, y, BridgeType.HorizontalYellowBridge, ref changedFields);
            }
            else if ((Table[x - 1, y] is Infrastructure && Table[x - 1, y].Height == 1 && Table[x + 1, y].Height == 1)
                || (Table[x + 1, y] is Infrastructure && Table[x + 1, y].Height == 1 && Table[x - 1, y].Height == 1))
            {
                cost = CreateVerticalBridge(y, x, x, BridgeType.VerticalYellowBridge, ref changedFields);
            }
            else if (Table[x, y - 1].FieldType == FieldType.Plain && Table[x, y + 1].FieldType == FieldType.Plain)
            {
                cost = CreateHorizontalBridge(x, y, y, BridgeType.HorizontalYellowBridge, ref changedFields);
            }
            else if (Table[x - 1, y].FieldType == FieldType.Plain && Table[x + 1, y].FieldType == FieldType.Plain)
            {
                cost = CreateVerticalBridge(y, x, x, BridgeType.VerticalYellowBridge, ref changedFields);
            }
            return cost;
        }
        public bool StopEnvironment(int x, int y)
        {
            bool result = false;
            if (NeighboursOfRoadsAndStops(x, y).Any(n => n is Road or Bridge))
            {
                Table[x, y] = new Stop(x, y, Table[x, y].Height);
                result = true;
            }
            if (x - 1 >= 0 && HeightCheck(Table[x - 1, y], Table[x, y]) && Table[x - 1, y] is BuildingBlocks blocks)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks);
                result = true;
            }
            else if (y + 1 < Width && HeightCheck(Table[x, y + 1], Table[x, y]) && Table[x, y + 1] is BuildingBlocks blocks1)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks1);
                result = true;
            }
            else if (x + 1 < Height && HeightCheck(Table[x + 1, y], Table[x, y]) && Table[x + 1, y] is BuildingBlocks blocks2)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks2);
                result = true;
            }
            else if (y - 1 >= 0 && HeightCheck(Table[x, y - 1], Table[x, y]) && Table[x, y - 1] is BuildingBlocks blocks3)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks3);
                result = true;
            }
            return result;
        }
        public void DestroyBridge(int x, int y, ref List<(int, int)> changedFields)
        {
            if (Table[x, y] is Bridge bridge)
            {
                Table[x, y] = new Water(x, y);
                changedFields.Add((x, y));
                if (bridge.BridgeType.ToString().Contains("Horizontal"))
                {
                    int left = y - 1;
                    while (Table[x, left] is Bridge)
                    {
                        Table[x, left] = new Water(x, left);
                        changedFields.Add((x, left));
                        left--;
                    }
                    if (Table[x, left] is Road rl) { rl.ChangeType(CalculateRoadType(x, left)); changedFields.Add((x, left)); }
                    int right = y + 1;
                    while (Table[x, right] is Bridge)
                    {
                        Table[x, right] = new Water(x, right);
                        changedFields.Add((x, right));
                        right++;
                    }
                    if (Table[x, right] is Road rr) { rr.ChangeType(CalculateRoadType(x, right)); changedFields.Add((x, right)); }
                }
                else
                {
                    int up = x - 1;
                    while (Table[up, y] is Bridge)
                    {
                        Table[up, y] = new Water(up, y);
                        changedFields.Add((up, y));
                        up--;
                    }
                    if (Table[up, y] is Road ru) { ru.ChangeType(CalculateRoadType(up, y)); changedFields.Add((up, y)); }
                    int down = x + 1;
                    while (Table[down, y] is Bridge)
                    {
                        Table[down, y] = new Water(down, y);
                        changedFields.Add((down, y));
                        down++;
                    }
                    if (Table[down, y] is Road rd) { rd.ChangeType(CalculateRoadType(down, y)); changedFields.Add((down, y)); }
                }
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
