namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// Defines a contract for generating forest data based on a given height map and map generation context.
    /// </summary>
    /// <remarks>Implementations of this interface provide algorithms for placing forests on a map, typically
    /// as part of a procedural map generation pipeline. The generated forest data can be used to influence terrain
    /// features, gameplay, or visual representation in map-based applications.</remarks>
    public interface IForestGenerator : IMapPluginGenerator
    {
        #region Public methods
        /// <summary>
        /// Generates a forest map based on the provided height map and generation context.
        /// </summary>
        /// <remarks>The dimensions of the returned array match those of the input height map. The
        /// specific values in the forest map depend on the rules and options defined in the provided context.</remarks>
        /// <param name="heightMap">A two-dimensional array representing terrain heights. Each element specifies the elevation at a given map
        /// coordinate. Cannot be null.</param>
        /// <param name="context">The context containing configuration and parameters for forest generation. Cannot be null.</param>
        /// <returns>A two-dimensional array representing the generated forest map, where each element indicates the presence or
        /// type of forest at the corresponding coordinate.</returns>
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context);
        #endregion
    }
}
