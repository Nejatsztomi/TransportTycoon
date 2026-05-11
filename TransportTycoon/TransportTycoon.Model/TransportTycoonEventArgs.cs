namespace TransportTycoon.Model
{
    /// <summary>
    /// Provides data for Transport Tycoon game-related events, including the current game time, maintenance status, and
    /// the number of vehicles.
    /// </summary>
    /// <remarks>Use this class to access event information relevant to the state of the game at the time the
    /// event is raised. All properties are read-only and reflect the state when the event occurred.</remarks>
    public sealed class TransportTycoonEventArgs : EventArgs
    {
        #region Fields
        private readonly ulong _gameTime;
        private readonly int _maintenance;
        private readonly int _numberOfVehicles;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current game time in milliseconds since the game started.
        /// </summary>
        /// <remarks>This property provides the elapsed time for the game, which can be used for timing
        /// events or animations. The value is updated continuously as the game runs.</remarks>
        public ulong GameTime => _gameTime;

        /// <summary>
        /// Gets the current maintenance value, representing the maintenance status of the system.
        /// </summary>
        public int Maintenance => _maintenance;

        /// <summary>
        /// Returns the players's vehicle numbers
        /// </summary>
        public int NumberOfVehicles => _numberOfVehicles;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TransportTycoonEventArgs class with the specified game time, number of
        /// vehicles, and maintenance value.
        /// </summary>
        /// <param name="gameTime">The current game time, in ticks, at which the event occurs.</param>
        /// <param name="numberOfVehicles">The number of vehicles involved in the event. Must be zero or greater.</param>
        /// <param name="maintenance">The maintenance value associated with the event. Represents the maintenance cost or count, depending on
        /// context.</param>
        public TransportTycoonEventArgs(ulong gameTime, int numberOfVehicles, int maintenance)
        {
            _gameTime = gameTime;
            _numberOfVehicles = numberOfVehicles;
            _maintenance = maintenance;
        }
        #endregion
    }
}
