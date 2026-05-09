namespace TransportTycoon.Model
{
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
        public TransportTycoonEventArgs(ulong gameTime, int numberOfVehicles, int maintenance)
        {
            _gameTime = gameTime;
            _numberOfVehicles = numberOfVehicles;
            _maintenance = maintenance;
        }
        #endregion
    }
}
