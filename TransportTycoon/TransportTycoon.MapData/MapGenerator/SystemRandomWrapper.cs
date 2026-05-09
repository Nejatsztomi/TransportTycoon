namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// A class for wrapping the standard <see cref="Random"/> class to implement the <see cref="IRandom"/> interface, allowing for consistent random number generation in the map generation process.
    /// </summary>
    public sealed class SystemRandomWrapper : IRandom
    {
        #region Private fields
        private readonly Random _random;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the <see cref="SystemRandomWrapper"/> class, initializing the underlying <see cref="Random"/> instance with the specified seed to ensure reproducibility of random number generation.
        /// </summary>
        /// <param name="seed">The seed value used to initialize the random number generator. This ensures reproducibility of the generated random numbers.</param>
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
