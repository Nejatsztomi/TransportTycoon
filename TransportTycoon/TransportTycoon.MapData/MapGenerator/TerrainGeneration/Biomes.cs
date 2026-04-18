namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class Biomes
    {
        #region Public fields
        public static readonly IBiome Default = new DefaultBiome();
        public static readonly IBiome Flat = new FlatBiome();
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
