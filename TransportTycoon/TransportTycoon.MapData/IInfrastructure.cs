namespace TransportTycoon.MapData
{
    public interface IInfrastructure : IField
    {

        #region Static Fields
        static abstract int Price { get; }
        #endregion

        #region Public methods
        public void Place() { }

        public void Remove() { }
        #endregion
    }
}
