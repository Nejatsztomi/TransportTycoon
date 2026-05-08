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
        /// <summary>
        /// Updates the cell at the specified row and column indices with the provided field.
        /// </summary>
        /// <remarks>Both indices must be within the bounds of the table. Supplying an out-of-range index
        /// will result in an exception.</remarks>
        /// <param name="x">The zero-based row index of the cell to update.</param>
        /// <param name="y">The zero-based column index of the cell to update.</param>
        /// <param name="field">The field to assign to the specified cell.</param>
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
        
        /// <summary>
        /// Generates the map using the specified map generator and context.
        /// </summary>
        public void GenerateMap()
        {
            (Table, BuildingEntities) = MapGenerator.GenerateMap(Context);
            IsMapGenerated = true;
        }

        /// <summary>
        /// Returns a list of adjacent fields that do not contain any trees.
        /// </summary>
        /// <remarks>Only immediate neighbors in the up, down, left, and right directions are considered.
        /// Diagonal neighbors are not included.</remarks>
        /// <param name="x">The zero-based x-coordinate of the field to check. Must be within the bounds of the table.</param>
        /// <param name="y">The zero-based y-coordinate of the field to check. Must be within the bounds of the table.</param>
        /// <returns>A list of neighboring fields that are directly adjacent to the specified position and do not contain any
        /// trees. The list is empty if no such fields exist.</returns>
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

        /// <summary>
        /// Checks if the new field's height is possible based on the surrounding fields.
        /// </summary>
        /// <param name="x">The zero-based x-coordinate of the field to check. Must be within the bounds of the table.</param>
        /// <param name="y">The zero-based y-coordinate of the field to check. Must be within the bounds of the table.</param>
        /// <param name="height">The height to check for the new field.</param>
        /// <returns><see langword="true"/> if the height is possible; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Retrieves the neighboring fields adjacent to the specified coordinates that are roads, stops, or bridges, if
        /// accessible.
        /// </summary>
        /// <remarks>The method checks the height compatibility of neighboring fields to ensure valid
        /// connections. It considers both horizontal and vertical bridges as valid neighbors, in addition to roads and
        /// stops. The returned list contains neighbors in a fixed order corresponding to the four cardinal
        /// directions.</remarks>
        /// <param name="x">The x-coordinate of the field for which to find neighboring roads, stops, or bridges. Must be within the
        /// bounds of the table.</param>
        /// <param name="y">The y-coordinate of the field for which to find neighboring roads, stops, or bridges. Must be within the
        /// bounds of the table.</param>
        /// <returns>A list of up to four neighboring fields that are roads, stops, or bridges. Each element corresponds to a
        /// direction and is null if no valid neighbor exists in that direction.</returns>
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

        /// <summary>
        /// Determines whether the heights of two fields differ by at most one unit.
        /// </summary>
        /// <remarks>Both parameters must be valid instances of IField. Passing a null value for either
        /// parameter may result in an exception.</remarks>
        /// <param name="a">The first field to compare. This parameter must not be null.</param>
        /// <param name="b">The second field to compare. This parameter must not be null.</param>
        /// <returns>true if the absolute difference between the heights of the two fields is less than or equal to one;
        /// otherwise, false.</returns>
        public bool HeightCheck(IField a, IField b)
        {
            return Math.Abs(a.Height - b.Height) <= 1;
        }

        /// <summary>
        /// Determines the type of road at the specified grid coordinates based on adjacent roads and stops.
        /// </summary>
        /// <remarks>The method analyzes neighboring fields to classify the road as straight, a turn, a
        /// T-intersection, or a four-way intersection. The result depends on the arrangement of adjacent roads and
        /// stops.</remarks>
        /// <param name="x">The x-coordinate of the grid position to evaluate.</param>
        /// <param name="y">The y-coordinate of the grid position to evaluate.</param>
        /// <returns>A value of the <see cref="RoadType"/> enumeration that indicates the road configuration at the specified
        /// coordinates.</returns>
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

        /// <summary>
        /// Determines the type of bridge to use based on the specified difference and direction.
        /// </summary>
        /// <remarks>The method categorizes bridges into yellow, green, or red types based on the
        /// difference value and direction. Ensure that the direction parameter is set to either "horizontal" or
        /// "vertical"; other values will default to vertical bridge types.</remarks>
        /// <param name="dif">The difference value that influences the bridge type selection. Must be a non-negative integer. Values
        /// greater than 17 result in no bridge type being returned.</param>
        /// <param name="dir">The direction of the bridge. Specify either "horizontal" or "vertical" to indicate the bridge orientation.</param>
        /// <returns>A BridgeType value representing the appropriate bridge type for the given difference and direction. Returns
        /// BridgeType.Null if the difference exceeds the supported range.</returns>
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

        /// <summary>
        /// Creates a horizontal bridge of the specified type between two horizontal positions on the given row and
        /// updates the affected fields.
        /// </summary>
        /// <remarks>This method also updates the road types of the fields immediately adjacent to the
        /// created bridges to ensure consistency with the new bridge structures.</remarks>
        /// <param name="y">The vertical position (y-coordinate) of the row where the bridge will be created.</param>
        /// <param name="a">The starting horizontal position (x-coordinate) for the bridge creation. Must be less than or equal to
        /// <paramref name="b"/>.</param>
        /// <param name="b">The ending horizontal position (x-coordinate) for the bridge creation. Must be greater than or equal to
        /// <paramref name="a"/>.</param>
        /// <param name="b_type">The type of bridge to create, specified by the <see cref="BridgeType"/> enumeration.</param>
        /// <param name="changedFields">A reference to a list that will be populated with the coordinates of all fields that are modified as a
        /// result of the bridge creation.</param>
        /// <returns>The total cost incurred for creating the bridges between the specified positions.</returns>
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

        /// <summary>
        /// Creates a series of vertical bridges of the specified type along a column within the given y-coordinate
        /// range and updates the list of affected fields.
        /// </summary>
        /// <remarks>This method also updates the road types immediately adjacent to the newly created
        /// bridges to ensure consistency with the surrounding infrastructure. The <paramref name="changedFields"/> list
        /// will include both the bridge locations and any adjacent roads that were updated.</remarks>
        /// <param name="x">The x-coordinate of the column where the vertical bridges are to be created.</param>
        /// <param name="a">The starting y-coordinate of the range in which bridges will be constructed. Must be less than or equal to
        /// <paramref name="b"/>.</param>
        /// <param name="b">The ending y-coordinate of the range in which bridges will be constructed. Must be greater than or equal to
        /// <paramref name="a"/>.</param>
        /// <param name="b_type">The type of bridge to create for each position in the specified range.</param>
        /// <param name="changedFields">A reference to a list that will be populated with the coordinates of all fields modified during the bridge
        /// creation process.</param>
        /// <returns>The total cost incurred for constructing the vertical bridges within the specified range.</returns>
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

        /// <summary>
        /// creates a short bridge at the specified coordinates if the surrounding fields allow for it, and updates the list of
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="changedFields"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a stop at the specified coordinates if the surrounding environment allows for it.
        /// </summary>
        /// <param name="x">The x-coordinate of the location where the stop is to be created.</param>
        /// <param name="y">The y-coordinate of the location where the stop is to be created.</param>
        /// <returns>True if a stop was successfully created; otherwise, false.</returns>
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

        /// <summary>
        /// Destroys the bridge at the specified coordinates and updates the surrounding fields accordingly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="changedFields"></param>
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
