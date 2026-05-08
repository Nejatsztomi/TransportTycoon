namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// A class that provides random number generators for map generation, ensuring that the same plugin ID and base seed combination will always produce the same sequence of random numbers.
    /// This is achieved by combining the base seed with a deterministic hash of the plugin ID to create a unique seed for each plugin generator, allowing for consistent and reproducible map generation across different runs and environments.
    /// The use of a deterministic string hash ensures that the same plugin ID will always yield the same hash value, contributing to the overall consistency of the random number generation process.
    /// </summary>
    public sealed class RandomProvider : IRandomProvider
    {
        #region Public methods
        /// <summary>
        /// Gets the random number generator for the specified plugin ID and base seed.
        /// The same plugin ID and base seed combination will always produce the same sequence of random numbers, ensuring consistent map generation across different runs and environments.
        /// </summary>
        /// <param name="baseSeed">The base seed used to initialize the random number generator. This ensures reproducibility of the generated random numbers.</param>
        /// <param name="pluginId">The unique identifier of the plugin requesting the random number generator. This allows for differentiation between different plugins.</param>
        /// <returns>An instance of an <see cref="IRandom"/> initialized with the specified base seed and associated with the specified plugin ID.</returns>
        public IRandom GetRandom(int baseSeed, string pluginId)
        {
            int pluginIdHash = GetDeterministicStringHash(pluginId);

            // Needed not to throw OverflowException, since this is a hash
            unchecked
            {
                int combinedSeed = (baseSeed * 397) ^ pluginIdHash;
                return new SystemRandomWrapper(combinedSeed);
            }
        }
        #endregion

        #region Private methods
        private int GetDeterministicStringHash(string str)
        {
            unchecked
            {
                int hash = (int)2166136261; // FNV offset basis
                foreach (char c in str)
                {
                    hash = (hash ^ c) * 16777619; // FNV prime
                }
                return hash;
            }
        }
        #endregion
    }
}
