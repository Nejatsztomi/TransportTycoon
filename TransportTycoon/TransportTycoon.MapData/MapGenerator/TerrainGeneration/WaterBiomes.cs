namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// A static class for creating and retrieving predefined water biome instances.
    /// It provides a registry of available water biomes and a method to get a biome by its ID.
    /// If the ID is not found in the registry, it defaults to returning the <see cref="Normal"/> biome.
    /// This class serves as a central point for managing water biome instances and their retrieval based on identifiers.
    /// </summary>
    public static class WaterBiomes
    {
        #region Public fields
        /// <summary>
        /// A predefined water biome instance representing a wet water biome with a higher water level.
        /// </summary>
        public readonly static IWaterBiome Wet = new Wet();

        /// <summary>
        /// The default water biome instance representing a normal water biome with a moderate water level.
        /// </summary>
        public readonly static IWaterBiome Normal = new Normal();

        /// <summary>
        /// A predefined water biome instance representing a dry water biome with a lower water level.
        /// </summary>
        public readonly static IWaterBiome Dry = new Dry();
        #endregion

        #region Private fields
        private static readonly Dictionary<string, IWaterBiome> _registry = new()
        {
            { Wet.Id, Wet },
            { Normal.Id, Normal },
            { Dry.Id, Dry }
        };
        #endregion

        #region Public methods
        /// <summary>
        /// A method to retrieve a water biome instance by its ID.
        /// If the ID is not found in the registry, it returns the <see cref="Normal"/> biome as a default.
        /// </summary>
        /// <param name="id">The ID of the water biome to retrieve.</param>
        /// <returns>The water biome instance corresponding to the specified ID, or the <see cref="Normal"/> biome if the ID is not found.</returns>
        public static IWaterBiome GetById(string id)
        {
            if (_registry.TryGetValue(id, out var biome))
                return biome;

            return Normal;
        }
        #endregion
    }

    internal readonly record struct Wet : IWaterBiome
    {
        #region Properties
        public readonly string Id => nameof(Wet);
        public readonly float WaterLevel => 0.75f;
        #endregion
    }

    internal readonly record struct Normal : IWaterBiome
    {
        #region Properties
        public readonly string Id => nameof(Normal);
        public readonly float WaterLevel => 0.5f;
        #endregion
    }

    internal readonly record struct Dry : IWaterBiome
    {
        #region Properties
        public readonly string Id => nameof(Dry);
        public readonly float WaterLevel => 0.25f;
        #endregion
    }
}
