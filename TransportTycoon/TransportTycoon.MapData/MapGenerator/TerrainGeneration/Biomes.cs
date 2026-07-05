namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// A static class to help register and retrieve biomes by their ID.
    /// It provides predefined biome instances and a method to get a biome by its ID, returning a default biome if the ID is not found.
    /// </summary>
    public static class Biomes
    {
        #region Public fields
        /// <summary>
        /// The default biome, which can be used as a fallback when an unknown biome ID is requested.
        /// It has balanced terrain distribution suitable for general use.
        /// </summary>
        public static readonly IBiome Default = new DefaultBiome();

        /// <summary>
        /// The flat biome, characterized by a large proportion of plains and minimal hills and mountains, making it ideal for open landscapes and agricultural areas.
        /// </summary>
        public static readonly IBiome Flat = new FlatBiome();

        /// <summary>
        /// The mountainous biome, characterized by a significant proportion of hills and mountains, with less flat terrain, making it suitable for rugged landscapes and mountainous regions.
        /// </summary>
        public static readonly IBiome Mountainous = new MountainousBiome();
        #endregion

        #region Private fields
        private static readonly Dictionary<string, IBiome> _registry = new()
        {
            { Default.Id, Default },
            { Flat.Id, Flat },
            { Mountainous.Id, Mountainous }
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Gets a biome by its ID.
        /// If the ID is not found in the registry, it returns the default biome.
        /// </summary>
        /// <param name="id">The ID of the biome to retrieve.</param>
        /// <returns>The biome corresponding to the given ID, or the default biome if not found.</returns>
        public static IBiome GetById(string id)
        {
            if (_registry.TryGetValue(id, out var biome))
                return biome;

            return Default;
        }
        #endregion
    }

    internal readonly record struct DefaultBiome : IBiome
    {
        #region Properties
        public readonly string Id => nameof(DefaultBiome);
        public readonly float WaterRange => 0.3f;
        public readonly float PlainRange => 0.55f;
        public readonly float HillRange => 0.75f;
        public readonly float MountainRange => 0.95f;
        public readonly float HighMountainRange => 1f;
        #endregion
    }

    internal readonly record struct FlatBiome : IBiome
    {
        #region Properties
        public readonly string Id => nameof(FlatBiome);
        public readonly float WaterRange => 0.2f;
        public readonly float PlainRange => 0.80f;
        public readonly float HillRange => 0.90f;
        public readonly float MountainRange => 0.95f;
        public readonly float HighMountainRange => 1f;
        #endregion
    }

    internal readonly record struct MountainousBiome : IBiome
    {
        #region Properties
        public readonly string Id => nameof(MountainousBiome);
        public readonly float WaterRange => 0.2f;
        public readonly float PlainRange => 0.38f;
        public readonly float HillRange => 0.70f;
        public readonly float MountainRange => 0.88f;
        public readonly float HighMountainRange => 1f;
        #endregion
    }
}
