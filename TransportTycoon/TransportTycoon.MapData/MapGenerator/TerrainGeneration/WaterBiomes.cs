namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterBiomes
    {
        #region Public fields
        public readonly static IWaterBiome Wet = new Wet();
        public readonly static IWaterBiome Normal = new Normal();
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
