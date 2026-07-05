namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// An interface that defines the contract for generating terrain data based on a noise map and map generation context.
    /// </summary>
    public interface ITerrainGenerator : IMapPluginGenerator
    {
        #region Public methods
        /// <summary>
        /// Generates terrain data based on the provided noise map and map generation context.
        /// The method returns a 2D array of integers representing the terrain types or heights for each point on the map.
        /// </summary>
        /// <param name="noiseMap">A two-dimensional array representing the noise values for terrain generation. Each element specifies the noise value at a given map coordinate. Cannot be null.</param>
        /// <param name="context">The context containing configuration and parameters for terrain generation. Cannot be null.</param>
        /// <returns>A two-dimensional array representing the generated terrain, where each element indicates the terrain type or height at the corresponding coordinate.</returns>
        public int[,] GenerateTerrain(float[,] noiseMap, MapGenerationContext context);
        #endregion
    }
}
