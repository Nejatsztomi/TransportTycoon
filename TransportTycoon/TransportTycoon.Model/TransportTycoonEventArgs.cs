namespace TransportTycoon.Model
{
    public sealed class TransportTycoonEventArgs : EventArgs
    {
        #region Fields
        private readonly ulong _gameTime;
        private readonly int _maintance;
        private readonly int _numberOfVehicles;
        #endregion

        #region Properties
        public ulong GameTime => _gameTime;

        public int Maintance => _maintance;

        public int NumberOfVehicles => _numberOfVehicles;
        #endregion

        #region Constructors
        public TransportTycoonEventArgs(ulong gameTime, int numberOfVehicles, int maintance)
        {
            this._gameTime = gameTime;
            this._numberOfVehicles = numberOfVehicles;
            this._maintance = maintance;
        }
        #endregion
    }
}
