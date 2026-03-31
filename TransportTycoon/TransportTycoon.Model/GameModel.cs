using System.Diagnostics;
using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public enum GameMode { Run, Paused, Editor }
    public enum TimeSpeed { Normal = 1, Fast = 2, SuperFast = 3 }
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2 }

    //Mintázat az összes osztályban
    #region Fields
    #endregion
    #region Properties
    #endregion
    #region Constructor
    #endregion
    #region Public Methods
    #endregion
    #region Private Methods
    #endregion
    #region Private event Methods
    #endregion

    public class GameModel
    {
        #region Constants
        public const int InitialInterval = 1_000;

        public const int InitialBalance = 1_000;
        public const Difficulty InitialDifficulty = Difficulty.Easy;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        #endregion

        #region Properties
        public GameTable Map { get; private set; }
        public Field? SelectedField { get; private set; }

        public int Balance { get; private set; }
        public int GameTime { get; private set; }
        public int Maintance { get; private set; }

        public GameMode Mode { get; private set; }
        public TimeSpeed TimeSpeed { get; private set; }
        public Difficulty Difficulty { get; private set; }

        public bool IsGameOver
        {
            get
            {
                return Balance <= 0;
            }
        }

        public List<Vehicle> Vehicles { get; private set; } = [];

        public int NumberOfVehicles => Vehicles.Count;
        #endregion

        #region Events
        public event EventHandler? NewGameCreated;
        public event EventHandler<GameMode>? GameModeChanged;
        public event EventHandler<TimeSpeed>? TimeSpeedChanged;
        public event EventHandler<TransportTycoonEventArgs>? GameOver;
        public event EventHandler<TransportTycoonFieldEventArgs>? FieldChanged;
        public event EventHandler? BalanceChanged;
        public event EventHandler? GameTicked;
        public event EventHandler<List<Tuple<int, int>>>? GameAdvanced;
        public event EventHandler<List<(int, int)>>? InfrastructureBuilt;
        public event EventHandler<(int, int)>? SelectedFieldChanged;
        #endregion

        #region Constructor
        public GameModel(Difficulty difficulty, int balance, ITimer timer)
        {
            Difficulty = difficulty;
            Balance = balance;
            _timer = timer;
            _timer.Elapsed += Timer_Tick;

            Mode = GameMode.Run;
            TimeSpeed = TimeSpeed.Normal;
            GameTime = 0;
            SelectedField = null!;
            Map = new();
        }

        public GameModel(int balance, ITimer timer) : this(InitialDifficulty, balance, timer) { }

        public GameModel(Difficulty difficulty, ITimer timer) : this(difficulty, InitialBalance, timer) { }
        #endregion

        #region Public Methods
        public void NewGame()
        {
            Balance = InitialBalance;

            Map.GenerateMap();
            _timer.Start();
            NewGameCreated?.Invoke(this, EventArgs.Empty);
        }

        public void SetTimeSpeed(TimeSpeed timeSpeed)
        {
            TimeSpeed = timeSpeed;
            _timer.Interval = InitialInterval / (double)(timeSpeed);
            TimeSpeedChanged?.Invoke(this, timeSpeed);
        }

        public void SetMode(GameMode mode)
        {
            Mode = mode;
            if (mode == GameMode.Paused || mode == GameMode.Editor)
            {
                _timer.Stop();
            }
            else
            {
                _timer.Start();
            }
            GameModeChanged?.Invoke(this, mode);
        }

        public void SetSelectedField(int x, int y)
        {
            if (x == -1 && y == -1) SelectedField = null;
            else SelectedField = Map[x, y];
            SelectedFieldChanged?.Invoke(this, (x, y));
        }

        public void IncreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height + 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.FieldType != FieldType.Road)
                    {
                        if (field.Height == 4) return;
                        if (terrain.Trees > 0)
                        {
                            Balance -= 50;
                        }
                        Balance -= 100;
                        terrain.IncreaseHeight();
                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);

                    }
                }
            }
        }

        public void DecreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height - 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.FieldType != FieldType.Road)
                    {
                        if (field.Height == 1) return;
                        if (terrain.Trees > 0)
                        {
                            Balance -= 50;
                        }
                        Balance -= 100;
                        terrain.DecreaseHeight();
                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);

                    }
                }
            }
        }
        public void BuildRoad(int x, int y)
        {
            if (Map[x, y] is not Terrain) return;
            List<(int, int)> changedFields = new List<(int, int)>();

            RoadType type = CalculateRoadType(x, y);
            Map[x, y] = new Road(x, y, type, Map[x, y].Height);
            changedFields.Add((x, y));

            List<(int, int)> neighbourRoads = Map.NeighbourRoadsCoord(x, y);
            foreach (var e in neighbourRoads)
            {
                RoadType e_type = CalculateRoadType(e.Item1, e.Item2);
                ((Road)Map[e.Item1, e.Item2]).ChangeType(e_type);//ChangeType method of Road
                changedFields.Add((e.Item1, e.Item2));
            }
            InfrastructureBuilt?.Invoke(this, changedFields);
        }
        public void BuildBridge(int x, int y)
        {

            if (Map[x, y] is not Water)
            {
                SetSelectedField(-1, -1);
                return;
            }
            if (SelectedField == null) SetSelectedField(x, y);
            else
            {
                List<(int, int)> changedFields = new List<(int, int)>();
                if (SelectedField.X != x && SelectedField.Y != y)
                {
                    SetSelectedField(-1, -1);
                    return;
                }
                else if (SelectedField.X == x)
                {
                    if (Math.Min(SelectedField.Y, y) - 1 < 0 || (Map[x, Math.Min(SelectedField.Y, y) - 1].FieldType != FieldType.Plain &&
                        Map[x, Math.Min(SelectedField.Y, y) - 1].FieldType!=FieldType.Road && Map[x, Math.Min(SelectedField.Y, y) - 1].FieldType != FieldType.Stop) ||
                        Math.Max(SelectedField.Y, y) + 1 >= Map.Width || (Map[x, Math.Max(SelectedField.Y, y) + 1].FieldType != FieldType.Plain &&
                        Map[x, Math.Max(SelectedField.Y, y) + 1].FieldType != FieldType.Road && Map[x, Math.Max(SelectedField.Y, y) + 1].FieldType != FieldType.Stop))
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    BridgeType b_type;
                    int dif = Math.Abs(SelectedField.Y - y);
                    if (dif <= 13) b_type = BridgeType.HorizontalYellowBridge;
                    else if (dif <= 15) b_type = BridgeType.HorizontalBlueBridge;
                    else if (dif <= 17) b_type = BridgeType.HorizontalRedBridge;
                    else
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    for (int i = Math.Min(SelectedField.Y, y) + 1; i < Math.Max(SelectedField.Y, y); i++)
                    {
                        if (Map[x, i] is not Water)
                        {
                            SetSelectedField(-1, -1);
                            return;
                        }
                    }
                    for (int i = Math.Min(SelectedField.Y, y); i <= Math.Max(SelectedField.Y, y); i++)
                    {
                        switch (b_type)
                        {
                            case BridgeType.HorizontalYellowBridge:
                                Map[x, i] = new YellowBridge(x, i, b_type, Map[x, i].Height);
                                break;
                            case BridgeType.HorizontalBlueBridge:
                                Map[x, i] = new BlueBridge(x, i, b_type, Map[x, i].Height);
                                break;
                            case BridgeType.HorizontalRedBridge:
                                Map[x, i] = new RedBridge(x, i, b_type, Map[x, i].Height);
                                break;
                        }
                        changedFields.Add((x, i));
                        Balance -= ((Bridge)Map[x, i]).Price;
                    }
                    if (Map[x, Math.Min(SelectedField.Y, y) - 1] is Road road1) road1.ChangeType(CalculateRoadType(x, Math.Min(SelectedField.Y, y) - 1));
                    changedFields.Add((x, Math.Min(SelectedField.Y, y) - 1));
                    if (Map[x, Math.Max(SelectedField.Y, y) - 1] is Road road2) road2.ChangeType(CalculateRoadType(x, Math.Max(SelectedField.Y, y) - 1));
                    changedFields.Add((x, Math.Max(SelectedField.Y, y) - 1));
                }
                else if (SelectedField.Y == y)
                {
                    if (Math.Min(SelectedField.X, x) - 1 < 0 || (Map[Math.Min(SelectedField.X, x) - 1, y].FieldType != FieldType.Plain &&
                        Map[Math.Min(SelectedField.X, x) - 1, y].FieldType != FieldType.Road && Map[Math.Min(SelectedField.X, x) - 1, y].FieldType != FieldType.Stop) ||
                        Math.Max(SelectedField.X, x) + 1 >= Map.Height || (Map[Math.Max(SelectedField.X, x) + 1, y].FieldType != FieldType.Plain &&
                        Map[Math.Max(SelectedField.X, x) + 1, y].FieldType != FieldType.Road && Map[Math.Min(SelectedField.X, x) - 1, y].FieldType != FieldType.Stop))
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    BridgeType b_type;
                    int dif = Math.Abs(SelectedField.X - x);
                    if (dif <= 13) b_type = BridgeType.VerticalYellowBridge;
                    else if (dif <= 15) b_type = BridgeType.VerticalBlueBridge;
                    else if (dif <= 17) b_type = BridgeType.VerticalRedBridge;
                    else
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    for (int i = Math.Min(SelectedField.X, x); i <= Math.Max(SelectedField.X, x); i++)
                    {
                        if (Map[i, y] is not Water)
                        {
                            SetSelectedField(-1, -1);
                            return;
                        }
                    }
                    for (int i = Math.Min(SelectedField.X, x); i <= Math.Max(SelectedField.X, x); i++)
                    {
                        switch (b_type)
                        {
                            case BridgeType.VerticalYellowBridge:
                                Map[i, y] = new YellowBridge(i, y, b_type, Map[i, y].Height);
                                break;
                            case BridgeType.VerticalBlueBridge:
                                Map[i, y] = new BlueBridge(i, y, b_type, Map[i, y].Height);
                                break;
                            case BridgeType.VerticalRedBridge:
                                Map[i, y] = new RedBridge(i, y, b_type, Map[i, y].Height);
                                break;
                            default:
                                break;
                        }
                        changedFields.Add((i, y));
                        Balance -= ((Bridge)Map[i, y]).Price;
                    }
                    if (Map[Math.Min(SelectedField.X, x), y] is Road road1) road1.ChangeType(CalculateRoadType(Math.Min(SelectedField.X, x), y));
                    changedFields.Add((Math.Min(SelectedField.X, x), y));
                    if (Map[Math.Max(SelectedField.X, x), y] is Road road2) road2.ChangeType(CalculateRoadType(Math.Max(SelectedField.X, x), y));
                    changedFields.Add((Math.Max(SelectedField.X, x), y));
                }
                SetSelectedField(-1, -1);
                InfrastructureBuilt?.Invoke(this, changedFields);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private Methods
        private void SetTax()
        {
            int tax = 30;
            switch (this.Difficulty)
            {
                case Difficulty.Easy:
                    tax = 10;
                    break;
                case Difficulty.Medium:
                    tax = 30;
                    break;
                case Difficulty.Hard:
                    tax = 50;
                    break;

            }
            Goods.SetGlobalTax(tax);
        }
        private List<Tuple<int, int>> ForestGrowing()
        {
            List<Tuple<int, int>> grownTrees = [];

            Random rnd = new();
            HashSet<Field> spreadedFields = [];
            for (int i = 0; i < Map.Height; i++)
            {
                for (int j = 0; j < Map.Width; j++)
                {
                    if (Map[i, j] is Terrain terrain && terrain.Trees > 0 && !terrain.IsFull)
                    {
                        if (rnd.Next(1, 101) <= 10)
                        {
                            if (terrain.Grow())
                            {
                                grownTrees.Add(new(i, j));
                            }

                            if (terrain.IsFull)
                            {
                                spreadedFields.UnionWith(Map.CheckNeighboringTrees(i, j));
                            }
                        }
                    }
                }
            }

            foreach (Field field in spreadedFields)
            {
                if (field is Terrain terrain && rnd.Next(1, 101) <= 100)
                {
                    terrain.SpreadForest();
                    grownTrees.Add(new(terrain.X, terrain.Y));
                }
            }

            return grownTrees;
        }
        private RoadType CalculateRoadType(int x, int y)
        {
            List<int> neighbourCountAndWhere = Map.NeighbourRoadsCount(x, y);
            RoadType type = RoadType.Vertical;
            switch (neighbourCountAndWhere[0])
            {
                case 1:
                    if (neighbourCountAndWhere[2] == 1 || neighbourCountAndWhere[4] == 1) type = RoadType.Horizontal;
                    break;
                case 2:
                    if (neighbourCountAndWhere[2] == 1 && neighbourCountAndWhere[4] == 1) type = RoadType.Horizontal;
                    else if (neighbourCountAndWhere[1] == 1 && neighbourCountAndWhere[2] == 1) type = RoadType.UpperRightTurn;
                    else if (neighbourCountAndWhere[2] == 1 && neighbourCountAndWhere[3] == 1) type = RoadType.RightTurn;
                    else if (neighbourCountAndWhere[3] == 1 && neighbourCountAndWhere[4] == 1) type = RoadType.LeftTurn;
                    else if (neighbourCountAndWhere[4] == 1 && neighbourCountAndWhere[1] == 1) type = RoadType.UpperLeftTurn;
                    break;
                case 3:
                    int noNeighbour = neighbourCountAndWhere.FindIndex(x => x == 0);
                    switch (noNeighbour)
                    {
                        case 1:
                            type = RoadType.DownTRoad;
                            break;
                        case 2:
                            type = RoadType.LeftTRoad;
                            break;
                        case 3:
                            type = RoadType.UpperTRoad;
                            break;
                        case 4:
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
        #endregion

        #region Private event Methods
        private void OnGameOver()
        {
            _timer.Stop();
            GameOver?.Invoke(this, new TransportTycoonEventArgs(GameTime, NumberOfVehicles, Maintance));
        }
        #endregion

        #region Timer event handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            GameTime++;
            if (GameTime > 0 && GameTime % 10 == 0)
            {
                var grownTrees = ForestGrowing();
                GameAdvanced?.Invoke(this, grownTrees);
            }
            GameTicked?.Invoke(this, EventArgs.Empty);


        }
        #endregion

    }
}
