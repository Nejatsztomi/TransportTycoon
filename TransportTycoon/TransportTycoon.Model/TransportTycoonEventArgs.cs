namespace TransportTycoon.Model
{
    public sealed class TransportTycoonEventArgs : EventArgs
    {
        #region Fields
        private readonly ulong _gameTime;
        private readonly int _Maintenance;
        private readonly int _numberOfVehicles;
        #endregion

        #region Properties
        public ulong GameTime => _gameTime;

        public int Maintenance => _Maintenance;

        public int NumberOfVehicles => _numberOfVehicles;
        #endregion

        #region Constructors
        public TransportTycoonEventArgs(ulong gameTime, int numberOfVehicles, int Maintenance)
        {
            this._gameTime = gameTime;
            this._numberOfVehicles = numberOfVehicles;
            this._Maintenance = Maintenance;
        }
        #endregion
    }
}
