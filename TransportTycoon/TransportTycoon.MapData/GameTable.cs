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
        public IField[,] Table { get; private set; }
        public int Width => Context.Width;
        public int Height => Context.Height;

        public List<BuildingEntity> BuildingEntities { get; private set; }

        public ref IField this[int x, int y]
        {
            get => ref Table[x, y];
        }

        /// <summary>
        /// Tells whether the map has been generated or not.
        /// This is used to prevent accessing the map before it is generated, which can cause errors.
        /// </summary>
        public bool IsMapGenerated { get; private set; }
        private IMapGenerator MapGenerator { get; }
        public MapGenerationContext Context { get; set; }
        #endregion

        #region Constructors
        public GameTable(IMapGenerator mapGenerator, MapGenerationContext context)
        {
            Context = context;
            BuildingEntities = [];

            Table = new IField[Width, Height];
            IsMapGenerated = false;
            MapGenerator = mapGenerator;
        }
        #endregion

        #region Public methods
        public void UpdateTable(int x, int y, IField field)
        {
            Table[x, y] = field;
        }

        /// <summary>
        /// Determines whether the specified coordinates are within the valid bounds defined by the current height and
        /// width.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns><see langword="true"/> if both coordinates are within bounds; otherwise, <see langword="false"/>.</returns>
        public bool IsInBounds(int x, int y) => 0 <= x && x < Width && 0 <= y && y < Height;

        public void GenerateMap()
        {
            (Table, BuildingEntities) = MapGenerator.GenerateMap(Context);
            IsMapGenerated = true;
        }

        public List<IField> CheckNeighboringTrees(int x, int y)
        {
            List<IField> neighbours = [];
            List<IField> acceptedNeighbours = [];
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

        public List<IField?> NeighboursOfRoadsAndStops(int x, int y)
        {
            List<IField?> result = [null, null, null, null];
            if (x - 1 >= 0 && HeightCheck(Table[x - 1, y], Table[x, y]))
            {
                if (Table[x - 1, y] is IBridge bridge && bridge.BridgeType.ToString().Contains("Horizontal")) result[3] = Table[x - 1, y];
                else if (Table[x - 1, y] is Road || Table[x - 1, y] is Stop) result[3] = Table[x - 1, y];
            }
            if (y + 1 <= Width - 1 && HeightCheck(Table[x, y + 1], Table[x, y]))
            {
                if (Table[x, y + 1] is IBridge bridge && bridge.BridgeType.ToString().Contains("Vertical")) result[2] = Table[x, y + 1];
                else if (Table[x, y + 1] is Road || Table[x, y + 1] is Stop) result[2] = Table[x, y + 1];
            }
            if (x + 1 <= Height - 1 && HeightCheck(Table[x + 1, y], Table[x, y]))
            {
                if (Table[x + 1, y] is IBridge bridge && bridge.BridgeType.ToString().Contains("Horizontal")) result[1] = Table[x + 1, y];
                else if (Table[x + 1, y] is Road || Table[x + 1, y] is Stop) result[1] = Table[x + 1, y];
            }
            if (y - 1 >= 0 && HeightCheck(Table[x, y - 1], Table[x, y]))
            {
                if (Table[x, y - 1] is IBridge bridge && bridge.BridgeType.ToString().Contains("Vertical")) result[0] = Table[x, y - 1];
                else if (Table[x, y - 1] is Road || Table[x, y - 1] is Stop) result[0] = Table[x, y - 1];
            }
            return result;
        }

        public bool HeightCheck(IField a, IField b)
        {
            return Math.Abs(a.Height - b.Height) <= 1;
        }

        public RoadType CalculateRoadType(int x, int y)
        {
            List<IField?> neighbourRoads = NeighboursOfRoadsAndStops(x, y);
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

        public int CreateHorizontalBridge(int y, int a, int b, BridgeType b_type, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            for (int i = a; i <= b; i++)
            {
                IBridge bridge = b_type
                switch
                {
                    BridgeType.HorizontalYellowBridge => new YellowBridge(i, y, b_type, Table[i, y].Height),
                    BridgeType.HorizontalGreenBridge => new GreenBridge(i, y, b_type, Table[i, y].Height),
                    BridgeType.HorizontalRedBridge => new RedBridge(i, y, b_type, Table[i, y].Height),
                    _ => new RedBridge(i, y, b_type, Table[i, y].Height)
                };

                UpdateTable(x, i, bridge);
                changedFields.Add((x, i));
                cost += bridge switch
                {
                    YellowBridge => YellowBridge.Price,
                    GreenBridge => GreenBridge.Price,
                    RedBridge => RedBridge.Price,
                    _ => 0
                };
            }

            if (Table[a - 1, y] is Road road1)
            {
                road1.ChangeType(CalculateRoadType(a - 1, y));
                UpdateTable(a - 1, y, road1);
                changedFields.Add((a - 1, y));
            }

            if (Table[b + 1, y] is Road road2)
            {
                road2.ChangeType(CalculateRoadType(b + 1, y));
                UpdateTable(b + 1, y, road2);
                changedFields.Add((b + 1, y));
            }

            return cost;
        }

        public int CreateVerticalBridge(int x, int a, int b, BridgeType b_type, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            for (int i = a; i <= b; i++)
            {
                IBridge bridge = b_type
                switch
                {
                    BridgeType.VerticalYellowBridge => new YellowBridge(x, i, b_type, Table[x, i].Height),
                    BridgeType.VerticalGreenBridge => new GreenBridge(x, i, b_type, Table[x, i].Height),
                    BridgeType.VerticalRedBridge => new RedBridge(x, i, b_type, Table[x, i].Height),
                    _ => new RedBridge(x, i, b_type, Table[x, i].Height)
                };

                UpdateTable(i, y, bridge);
                changedFields.Add((i, y));
                cost += bridge switch
                {
                    YellowBridge => YellowBridge.Price,
                    GreenBridge => GreenBridge.Price,
                    RedBridge => RedBridge.Price,
                    _ => 0
                };
            }
            if (Table[x, a - 1] is Road road1)
            {
                road1.ChangeType(CalculateRoadType(x, a - 1));
                UpdateTable(x, a - 1, road1);
                changedFields.Add((x, a - 1));
            }
            if (Table[x, b + 1] is Road road2)
            {
                road2.ChangeType(CalculateRoadType(x, b + 1));
                UpdateTable(x, b + 1, road2);
                changedFields.Add((x, b + 1));
            }
            return cost;
        }

        public int CreateShortBridge(int x, int y, ref List<(int, int)> changedFields)
        {
            int cost = 0;
            if (x - 1 < 0 || y - 1 < 0 || x + 1 > Height - 1 || y + 1 > Width - 1) return 0;
            if ((Table[x, y - 1] is IInfrastructure && Table[x, y - 1].Height == 1 && Table[x, y + 1].Height == 1)
                || (Table[x, y + 1] is IInfrastructure && Table[x, y + 1].Height == 1 && Table[x, y - 1].Height == 1))
            {
                cost = CreateHorizontalBridge(x, y, y, BridgeType.HorizontalYellowBridge, ref changedFields);
            }
            else if ((Table[x - 1, y] is IInfrastructure && Table[x - 1, y].Height == 1 && Table[x + 1, y].Height == 1)
                || (Table[x + 1, y] is IInfrastructure && Table[x + 1, y].Height == 1 && Table[x - 1, y].Height == 1))
            {
                cost = CreateVerticalBridge(y, x, x, BridgeType.VerticalYellowBridge, ref changedFields);
            }
            else if (Table[x, y - 1] is Terrain terrain && terrain.TerrainType == TerrainType.Plain && Table[x, y + 1] is Terrain terrain2 && terrain2.TerrainType == TerrainType.Plain)
            {
                cost = CreateHorizontalBridge(x, y, y, BridgeType.HorizontalYellowBridge, ref changedFields);
            }
            else if (Table[x - 1, y] is Terrain terrain3 && terrain3.TerrainType == TerrainType.Plain && Table[x + 1, y] is Terrain terrain4 && terrain4.TerrainType == TerrainType.Plain)
            {
                cost = CreateVerticalBridge(y, x, x, BridgeType.VerticalYellowBridge, ref changedFields);
            }
            return cost;
        }

        public bool StopEnvironment(int x, int y)
        {
            bool result = false;
            if (NeighboursOfRoadsAndStops(x, y).Any(n => n is Road or IBridge))
            {
                Table[x, y] = new Stop(x, y, Table[x, y].Height);
                result = true;
            }

            if (y - 1 >= 0 && HeightCheck(Table[x, y - 1], Table[x, y]) && Table[x, y - 1] is IBuildingBlocks blocks3)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks3);
                result = true;
            }
            if (x + 1 < Height && HeightCheck(Table[x + 1, y], Table[x, y]) && Table[x + 1, y] is IBuildingBlocks blocks2)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks2);
                result = true;
            }
            if (y + 1 < Width && HeightCheck(Table[x, y + 1], Table[x, y]) && Table[x, y + 1] is IBuildingBlocks blocks1)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks1);
                result = true;
            }
            if (x - 1 >= 0 && HeightCheck(Table[x - 1, y], Table[x, y]) && Table[x - 1, y] is IBuildingBlocks blocks)
            {
                if (!result) Table[x, y] = new Stop(x, y, Table[x, y].Height);
                ((Stop)Table[x, y]).SetBuildingBlocks(blocks);
                result = true;
            }
            return result;
        }

        public void DestroyBridge(int x, int y, ref List<(int, int)> changedFields)
        {
            if (Table[x, y] is IBridge bridge)
            {
                Table[x, y] = new Water(x, y);
                changedFields.Add((x, y));
                if (bridge.BridgeType.ToString().Contains("Horizontal"))
                {
                    int left = x - 1;
                    while (Table[left, y] is IBridge)
                    {
                        Table[left, y] = new Water(left, y);
                        changedFields.Add((left, y));
                        left--;
                    }

                    if (Table[left, y] is Road rl)
                    {
                        rl.ChangeType(CalculateRoadType(left, y));
                        UpdateTable(left, y, rl);
                        changedFields.Add((left, y));
                    }

                    int right = x + 1;
                    while (Table[right, y] is IBridge)
                    {
                        Table[right, y] = new Water(right, y);
                        changedFields.Add((right, y));
                        right++;
                    }

                    if (Table[right, y] is Road rr)
                    {
                        rr.ChangeType(CalculateRoadType(right, y));
                        UpdateTable(right, y, rr);
                        changedFields.Add((right, y));
                    }
                }
                else
                {
                    int up = y - 1;
                    while (Table[x, up] is IBridge)
                    {
                        Table[x, up] = new Water(x, up);
                        changedFields.Add((x, up));
                        up--;
                    }
                    if (Table[x, up] is Road ru)
                    {
                        ru.ChangeType(CalculateRoadType(x, up));
                        UpdateTable(x, up, ru);
                        changedFields.Add((x, up));
                    }
                    int down = y + 1;
                    while (Table[x, down] is IBridge)
                    {
                        Table[x, down] = new Water(x, down);
                        changedFields.Add((x, down));
                        down++;
                    }
                    if (Table[x, down] is Road rd)
                    {
                        rd.ChangeType(CalculateRoadType(x, down));
                        UpdateTable(x, down, rd);
                        changedFields.Add((x, down));
                    }
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
