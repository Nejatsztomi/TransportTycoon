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
        public ulong GameTime => _gameTime;

        public int Maintenance => _maintenance;

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
