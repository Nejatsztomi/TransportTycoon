namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An interface representing a water biome in the context of map generation.
    /// It defines properties that provide information about the water level and other characteristics specific to water biomes.
    /// </summary>
    public interface IWaterBiome
    {
        #region Properties
        /// <summary>
        /// The name or ID of the water biome.
        /// This property can be used to identify the type of water biome.
        /// It provides a way to differentiate between various water biomes based on their characteristics or intended use in the map generation process.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the current water level as a floating-point value.
        /// </summary>
        public float WaterLevel { get; }
        #endregion
    }
}
