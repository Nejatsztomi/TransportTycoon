namespace TransportTycoon.Model
{
    public class TransportTycoonEventArgs(int gameTime, int numberOfVehicles, int maintance, int balance) : EventArgs
    {
        #region Properties
        public int GameTime => gameTime;

        public int Maintance => maintance;
        
        public int Balance => balance;

        public int NumberOfVehicles => numberOfVehicles;
        #endregion
    }
}
