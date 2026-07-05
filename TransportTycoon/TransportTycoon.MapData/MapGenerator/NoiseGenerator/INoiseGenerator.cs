namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    /// <summary>
    /// An interface for generating noise maps used in procedural map generation.
    /// Implementations of this interface can provide different noise algorithms (e.g., Perlin, Simplex) to create varied terrain features.
    /// </summary>
    public interface INoiseGenerator : IMapPluginGenerator
    {
        #region Public methods
        /// <summary>
        /// Generates a noise value for the given coordinates (x, y) using the specified seed.
        /// The noise value is typically a float between 0 and 1, where different values can represent different terrain features (e.g., water, land, mountains).
        /// </summary>
        /// <param name="x">The X coordinate for which to generate the noise value.</param>
        /// <param name="y">The Y coordinate for which to generate the noise value.</param>
        /// <param name="seed">The seed value used to vary the generated noise.</param>
        /// <returns>A noise value between 0 and 1 for the specified coordinates and seed. Must be predictable and consistent for the same input parameters.</returns>
        public float GenerateNoise(float x, float y, int seed);

        /// <summary>
        /// Generates a noise map for the specified dimensions and seed.
        /// </summary>
        /// <param name="width">The width of the noise map.</param>
        /// <param name="height">The height of the noise map.</param>
        /// <param name="seed">The seed value used to vary the generated noise map.</param>
        /// <returns>A 2D array of noise values between 0 and 1. Must be predictable and consistent for the same input parameters.</returns>
        public float[,] GenerateNoiseMap(int width, int height, int seed);
        #endregion
    }
}
