namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An interface for providing random number generators, which can be used in the map generation process to create randomness and variability in the generated maps.
    /// This interface allows for different implementations of random number generators to be used, such as pseudo-random generators or more complex algorithms for specific use cases, by providing a method to retrieve an instance of an <see cref="IRandom"/> based on a given seed and plugin ID.
    /// The seed can be used to ensure reproducibility of the generated maps, while the plugin ID can be used to differentiate between different plugins that may require their own random number generators.
    /// </summary>
    public interface IRandomProvider
    {
        #region Public methods
        /// <summary>
        /// Gets an instance of an <see cref="IRandom"/> based on the provided seed and plugin ID.
        /// The seed can be used to ensure that the same sequence of random numbers is generated for the same seed, allowing for reproducibility in map generation.
        /// The plugin ID can be used to differentiate between different plugins that may require their own random number generators, allowing for more modular and flexible map generation processes.
        /// </summary>
        /// <param name="seed">The seed value used to initialize the random number generator. This ensures reproducibility of the generated random numbers.</param>
        /// <param name="pluginId">The unique identifier of the plugin requesting the random number generator. This allows for differentiation between different plugins.</param>
        /// <returns>An instance of an <see cref="IRandom"/> initialized with the specified seed and associated with the specified plugin ID.</returns>
        public IRandom GetRandom(int seed, string pluginId);
        #endregion
    }
}
