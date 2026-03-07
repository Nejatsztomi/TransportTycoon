using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public enum GameMode { Run, Paused, Editor }
    public enum TimeSpeed { Normal, Fast, SuperFast }
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
        private const int InitialBalance = 1_000;
        private static readonly Difficulty InitialDifficulty = Difficulty.Easy;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        #endregion

        #region Properties
        public GameTable Map { get; private set; }
        public int Balance { get; private set; }

        public GameMode Mode { get; private set; }
        public TimeSpeed TimeSpeed { get; private set; }
        public Difficulty Difficulty { get; private set; }
        #endregion

        #region Events
        #endregion

        #region Constructor
        public GameModel(Difficulty difficulty, int balance, ITimer timer)
        {
            Difficulty = difficulty;
            Balance = balance;
            _timer = timer;

            Mode = GameMode.Run;
            TimeSpeed = TimeSpeed.Normal;
        }

        public GameModel(int balance, ITimer timer) : this(InitialDifficulty, balance, timer) { }

        public GameModel(Difficulty difficulty, ITimer timer) : this(difficulty, InitialBalance, timer) { }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void SetTax()
        {
            Goods.SetGlobalTax(this.Difficulty);
        }
        #endregion

        #region Private event Methods
        #endregion

        #region Timer event handlers
        #endregion

    }
}
