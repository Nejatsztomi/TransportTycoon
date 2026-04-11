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
        /// <summary>
        /// Represents the default interval value, in milliseconds, used for timing operations.
        /// </summary>
        public const int DefaultInterval = 1_000;

        /// <summary>
        /// Default starting balance for new game.
        /// </summary>
        public const int DefaultBalance = 1_000_000;
        /// <summary>
        /// Default starting difficulty for new game.
        /// </summary>
        public const Difficulty DefaultDifficulty = Difficulty.Medium;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        #endregion

        #region Properties
        public GameTable Map { get; private set; }
        public Field? SelectedField { get; private set; }
        public List<Field>? SelectedStopFields { get; private set; }//
        public int Balance { get; private set; }
        public int GameTime { get; private set; }
        public int Maintance { get; private set; }

        public GameMode Mode
        {
            get;
            set
            {
                if (value == GameMode.Paused || value == GameMode.Editor)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                }
                GameModeChanged?.Invoke(this, value);
                field = value;
            }
        }
        public TimeSpeed TimeSpeed
        {
            get;
            set
            {
                _timer.Interval = DefaultInterval / (double)(value);
                TimeSpeedChanged?.Invoke(this, value);
                field = value;
            }
        }
        public Difficulty Difficulty { get; }

        public bool IsGameOver => Balance <= 0;

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
        public event EventHandler<Field>? SelectedStopFieldsChanged;//
        #endregion

        #region Constructor
        public GameModel(GameTable map, ITimer timer, Difficulty difficulty = DefaultDifficulty, int balance = DefaultBalance)
        {
            Difficulty = difficulty;
            Balance = balance;
            Map = map;
            _timer = timer;
            _timer.Elapsed += Timer_Tick;

            Mode = GameMode.Run;
            TimeSpeed = TimeSpeed.Normal;
            GameTime = 0;
        }
        #endregion

        #region Public Methods
        public void NewGame()
        {
            Balance = DefaultBalance;

            Map.GenerateMap();
            _timer.Start();
            NewGameCreated?.Invoke(this, EventArgs.Empty);
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
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }
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
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }

                    }
                }
            }
        }
        public void BuildRoad(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;
            List<(int, int)> changedFields = [];

            int oldTrees = Map[x, y].GetTrees();
            Map[x, y] = new Road(x, y, Map.CalculateRoadType(x, y), Map[x, y].Height);
            changedFields.Add((x, y));

            if (oldTrees == 0) Balance -= ((Road)Map[x, y]).Price;
            else Balance -= ((Road)Map[x, y]).Price * 2;

            foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
            {
                if (e != null && e is Road road)
                {
                    road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                    changedFields.Add((e.X, e.Y));
                }
            }
            if (IsGameOver) OnGameOver();
            InfrastructureBuilt?.Invoke(this, changedFields);
            BalanceChanged?.Invoke(this, EventArgs.Empty);
        }
        public void BuildBridge(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Water) { SetSelectedField(-1, -1); return; }
            if (SelectedField == null) SetSelectedField(x, y);
            else
            {
                List<(int, int)> changedFields = [];
                if (SelectedField.X != x && SelectedField.Y != y) { SetSelectedField(-1, -1); return; }
                else if (SelectedField.X == x && SelectedField.Y == y)
                {
                    Balance -= Map.CreateShortBridge(x, y, ref changedFields);
                }
                else if (SelectedField.X == x)
                {
                    if (Math.Min(SelectedField.Y, y) - 1 < 0 || Map[x, Math.Min(SelectedField.Y, y) - 1].Height != 1 ||
                        Math.Max(SelectedField.Y, y) + 1 >= Map.Width || Map[x, Math.Max(SelectedField.Y, y) + 1].Height != 1)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    int dif = Math.Abs(SelectedField.Y - y);
                    BridgeType b_type = Map.CalculateBridgeType(dif, "horizontal");
                    if (b_type == BridgeType.Null) { SetSelectedField(-1, -1); return; }

                    for (int i = Math.Min(SelectedField.Y, y) + 1; i < Math.Max(SelectedField.Y, y); i++)
                    {
                        if (Map[x, i] is not Water) { SetSelectedField(-1, -1); return; }
                    }
                    Balance -= Map.CreateHorizontalBridge(x, Math.Min(SelectedField.Y, y), Math.Max(SelectedField.Y, y), b_type, ref changedFields);
                }
                else if (SelectedField.Y == y)
                {
                    if (Math.Min(SelectedField.X, x) - 1 < 0 || Map[Math.Min(SelectedField.X, x) - 1, y].Height != 1 ||
                        Math.Max(SelectedField.X, x) + 1 >= Map.Height || Map[Math.Max(SelectedField.X, x) + 1, y].Height != 1)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }

                    int dif = Math.Abs(SelectedField.X - x);
                    BridgeType b_type = Map.CalculateBridgeType(dif, "vertical");
                    if (b_type == BridgeType.Null) { SetSelectedField(-1, -1); return; }

                    for (int i = Math.Min(SelectedField.X, x); i <= Math.Max(SelectedField.X, x); i++)
                    {
                        if (Map[i, y] is not Water) { SetSelectedField(-1, -1); return; }
                    }
                    Balance -= Map.CreateVerticalBridge(y, Math.Min(SelectedField.X, x), Math.Max(SelectedField.X, x), b_type, ref changedFields);
                }
                SetSelectedField(-1, -1);
                if (IsGameOver) OnGameOver();
                InfrastructureBuilt?.Invoke(this, changedFields);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void BuildStop(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;
            List<(int, int)> changedFields = [];

            int oldTrees = Map[x, y].GetTrees();
            if (Map.StopEnvironment(x, y))
            {
                changedFields.Add((x, y));

                if (oldTrees == 0) Balance -= ((Stop)Map[x, y]).Price;
                else Balance -= ((Stop)Map[x, y]).Price * 2;

                foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
                {
                    if (e != null && e is Road road)
                    {
                        road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                        changedFields.Add((e.X, e.Y));
                    }
                }
                if (IsGameOver) OnGameOver();
                InfrastructureBuilt?.Invoke(this, changedFields);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void Destroy(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Infrastructure || (Map[x, y] is Road r && r.InCity())
                || Vehicles.Any(v => v.X == x && v.Y == y)) return;
            List<(int, int)> changedFields = [];

            if (Map[x, y] is Road || Map[x, y] is Stop)
            {
                Map[x, y] = new Terrain(x, y, Map[x, y].Height);
                changedFields.Add((x, y));

                foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
                {
                    if (e != null && e is Road road)
                    {
                        road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                        changedFields.Add((e.X, e.Y));
                    }
                }
            }
            else
            {
                Map.DestroyBridge(x, y, ref changedFields);
            }
            InfrastructureBuilt?.Invoke(this, changedFields);
        }

        //Create a new Vehicle based on the given type and coordinates, and add it to the player's collection if they have enough balance. Returns the created Vehicle.
        public Vehicle? BuyVehicle(int x, int y, VehicleType type)
        {
            if (Map[x, y] is not Stop) return null;

            Vehicle vehicle = type switch
            {
                VehicleType.Van => new Van(x, y, Direction.Up),
                VehicleType.Pickup => new Pickup(x, y, Direction.Up),
                VehicleType.Truck => new Truck(x, y, Direction.Up),
                VehicleType.LiquidTruck => new LiquidTruck(x, y, Direction.Up),
                VehicleType.SmallBus => new SmallBus(x, y, Direction.Up),
                VehicleType.BigBus => new BigBus(x, y, Direction.Up),
                _ => throw new ArgumentException("Invalid vehicle type", nameof(type)),
            };

            if (Balance >= vehicle.Price)
            {
                Balance -= vehicle.Price;
                Vehicles.Add(vehicle);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
                if (IsGameOver)
                {
                    OnGameOver();
                }
            }
            return vehicle;
        }
        public void DefineRoute(int x,int y)//
        {

        }
        public void QueryRoute(int x, int y)//
        {

        }
        public void AssignRoute(int x, int y)//
        {

        }
        public void DeleteRoute(int x, int y)//
        {

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
        #endregion

        #region Private event Methods
        private void OnGameOver()
        {
            _timer.Stop();
            GameModeChanged?.Invoke(this, GameMode.Paused);
            GameOver?.Invoke(this, new TransportTycoonEventArgs(GameTime, NumberOfVehicles, Maintance));
        }
        #endregion

        #region Timer event handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (IsGameOver)
            {
                OnGameOver();
                return;
            }
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
