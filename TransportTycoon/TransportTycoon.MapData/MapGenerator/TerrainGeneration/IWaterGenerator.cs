namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// An interface that defines the contract for generating water maps based on a noise map, an existing water map, and a map generation context.
    /// </summary>
    public interface IWaterGenerator : IMapPluginGenerator
    {
        #region Public methods
        /// <summary>
        /// Generates a water map by determining which locations in the provided noise map should be classified as
        /// water.
        /// </summary>
        /// <param name="noiseMap">A two-dimensional array of noise values representing terrain elevation or features. Each value typically
        /// corresponds to a point on the map.</param>
        /// <param name="waterMap">A two-dimensional boolean array indicating the initial water state for each location. This array may be used
        /// as input and updated with the generated water map.</param>
        /// <param name="context">The context containing configuration and state information relevant to the map generation process. May
        /// include thresholds, settings, or other data required for water map generation.</param>
        /// <returns>A two-dimensional boolean array where each element is <see langword="true"/> if the corresponding location
        /// is classified as water; otherwise, <see langword="false"/>.</returns>
        public bool[,] GenerateWaterMap(float[,] noiseMap, bool[,] waterMap, MapGenerationContext context);
        #endregion
    }
}
