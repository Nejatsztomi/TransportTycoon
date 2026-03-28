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
        #region Private constants
        private const int InitialInterval = 1_000;

        private const int InitialBalance = 1_000;
        private const Difficulty InitialDifficulty = Difficulty.Easy;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        #endregion

        #region Properties
        public GameTable Map { get; private set; }
        public int Balance { get; private set; }
        public int GameTime { get; private set; }

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
        public event EventHandler? GameTicked;
        public event EventHandler<List<Tuple<int, int>>>? GameAdvanced;
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
        public bool IncreaseHeight(int x, int y)
        {
            Field field = Map[x, y];

            if (field is Terrain terrain)
            {
                int nextHeight = terrain.Height + 1;

                if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.Trees == 0)
                {
                    terrain.IncreaseHeight();
                    return true;
                }
            }

            return false;
        }

        public bool DecreaseHeight(int x, int y)
        {
            Field field = Map[x, y];

            if (field is Terrain terrain)
            {
                int nextHeight = terrain.Height - 1;

                if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.Trees == 0)
                {
                    terrain.DecreaseHeight();
                    return true;
                }
            }

            return false;
        }
        public void BuildRoad(int x,int y)
        {
            if (Map[x, y] is not Terrain) return;
            
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
            GameOver?.Invoke(this, new TransportTycoonEventArgs(GameTime, NumberOfVehicles));
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
