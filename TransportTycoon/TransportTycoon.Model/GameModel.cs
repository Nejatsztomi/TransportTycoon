using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public enum GameMode { Run, Paused, Editor }
    public enum TimeSpeed { Normal = 1, Fast = 2, SuperFast = 3 }
    public enum Difficulty { Easy, Medium, Hard }

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
        private static readonly Difficulty InitialDifficulty = Difficulty.Easy;
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
        #endregion

        #region Events
        public event EventHandler? NewGameCreated;
        public event EventHandler<GameMode>? GameModeChanged;
        public event EventHandler<TimeSpeed>? TimeSpeedChanged;
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

                if (Map.IsTileHeightPossible(x, y, nextHeight))
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

                if (Map.IsTileHeightPossible(x, y, nextHeight))
                {
                    terrain.DecreaseHeight();
                    return true;
                }
            }

            return false;
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
        #endregion

        #region Private event Methods
        #endregion

        #region Timer event handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            GameTime++;
        }
        #endregion

    }
}
