namespace TransportTycoon.MapData
{
    public interface IInfrastructure : IField
    {

        #region Static Fields
        static abstract int Price { get; }
        #endregion

        #region Public methods
        public void Place() { }

        // makes a terrain with a height based on infrastructure's height, and replace 
        public void Remove() { }
        #endregion
    }
}
