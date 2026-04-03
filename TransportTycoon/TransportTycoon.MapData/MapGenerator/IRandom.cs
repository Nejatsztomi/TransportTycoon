namespace TransportTycoon.MapData.MapGenerator
{
    public interface IRandom
    {
        #region Public methods
        public int Next();
        public int Next(int maxValue);
        public int Next(int maxValue, int minValue);
        public float NextSingle();
        public double NextDouble();
        #endregion
    }
}
