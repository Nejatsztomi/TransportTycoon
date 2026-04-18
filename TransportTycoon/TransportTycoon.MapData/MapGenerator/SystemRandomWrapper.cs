namespace TransportTycoon.MapData.MapGenerator
{
    public sealed class SystemRandomWrapper : IRandom
    {
        #region Private fields
        private readonly Random _random;
        #endregion

        #region Constructor
        public SystemRandomWrapper(int seed)
        {
            _random = new Random(seed);
        }
        #endregion

        #region Public methods
        public int Next() => _random.Next();
        public int Next(int maxValue) => _random.Next(maxValue);
        public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
        public float NextSingle() => _random.NextSingle();
        public double NextDouble() => _random.NextDouble();
        #endregion
    }
}
