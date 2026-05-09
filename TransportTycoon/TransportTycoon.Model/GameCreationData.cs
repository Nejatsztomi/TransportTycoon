using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Model
{
    /// <summary>
    /// A record struct that encapsulates the necessary data for creating a new map in the game.
    /// This includes the save name, difficulty level, and starting balance for the player.
    /// It is designed to be immutable and can be easily passed around within the application when initializing a new game session.
    /// </summary>
    public readonly record struct GameCreationData
    {
        #region Properties
        /// <summary>
        /// The name to use when saving the map.
        /// This should be a non-empty string and is used to identify the saved game file.
        /// It is important for organizing and retrieving saved games within the application.
        /// </summary>
        public string SaveName { get; }

        /// <summary>
        /// Gets the difficulty level for the current context.
        /// </summary>
        public Difficulty Difficulty { get; }

        /// <summary>
        /// Gets the current balance.
        /// </summary>
        public int Balance { get; }

        /// <summary>
        /// Gets the context information used during map generation.
        /// </summary>
        public MapGenerationContext MapGenerationContext { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCreationData"/> record struct with the specified save name, map generation
        /// context, difficulty, and starting balance.
        /// </summary>
        /// <param name="context">The context information used for map generation.</param>
        /// <param name="saveName">The name to use when saving the map. Cannot be an empty string.</param>
        /// <param name="difficulty">The difficulty level to apply to the map. If not specified, the <see cref="GameModel.DefaultDifficulty"/> is used.</param>
        /// <param name="balance">The starting balance for the map. Must be a non-negative integer. If not specified, the <see cref="GameModel.DefaultBalance"/> is used.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="saveName"/> is an empty string or if <paramref name="balance"/> is negative.</exception>
        public GameCreationData(MapGenerationContext context, string saveName, Difficulty difficulty = GameModel.DefaultDifficulty, int balance = GameModel.DefaultBalance)
        {
            if (saveName.Length <= 0 || string.IsNullOrWhiteSpace(saveName)) throw new ArgumentException("Invalid save name.");
            if (balance < 0) throw new ArgumentException("Balance must be a non-negative integer.");

            SaveName = saveName;
            MapGenerationContext = context;
            Difficulty = difficulty;
            Balance = balance;
        }

        /// <summary>
        /// The default constructor for the <see cref="GameCreationData"/> record struct, which initializes the properties with default values.
        /// </summary>
        public GameCreationData() : this(new(), DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")) { }
        #endregion
    }
}
