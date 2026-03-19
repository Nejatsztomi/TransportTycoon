namespace TransportTycoon.MapData
{
    public abstract class Infrastructure : Field
    {
        #region Field
        public int Price { get; protected set; }
        #endregion

        #region Public methods
        public void Place() { }

        // makes a terrain with a height based on infrastructure's height, and replace 
        public void Remove() { }
        #endregion
    }
}
