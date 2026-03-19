namespace TransportTycoon.Model
{
    public class TransportTycoonEventArgs(int gameTime, int numberOfVehicles) : EventArgs
    {
        #region Properties
        public int GameTime => gameTime;

        public int NumberOfVehicles => numberOfVehicles;
        #endregion
    }
}
