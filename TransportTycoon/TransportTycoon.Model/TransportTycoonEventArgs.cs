namespace TransportTycoon.Model
{
    public sealed class TransportTycoonEventArgs : EventArgs
    {
        #region Fields
        private int gameTime;
        private int maintance;
        private int numberOfVehicles;
        #endregion

        #region Properties
        public int GameTime => gameTime;

        public int Maintance => maintance;

        public int NumberOfVehicles => numberOfVehicles;
        #endregion

        #region Constructors
        public TransportTycoonEventArgs(int gameTime, int numberOfVehicles, int maintance)
        {
            this.gameTime = gameTime;
            this.numberOfVehicles = numberOfVehicles;
            this.maintance = maintance;
        }
        #endregion
    }
}
