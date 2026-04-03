namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterBiomes
    {
        #region Biomes
        public static IWaterBiome Wet { get; } = new Wet();
        public static IWaterBiome Normal { get; } = new Normal();
        public static IWaterBiome Dry { get; } = new Dry();
        #endregion
    }

    internal class Wet : IWaterBiome
    {
        #region Properties
        public float WaterLevel { get; } = 0.75f;
        #endregion
    }

    internal class Normal : IWaterBiome
    {
        #region Properties
        public float WaterLevel { get; } = 0.5f;
        #endregion
    }

    internal class Dry : IWaterBiome
    {
        #region Properties
        public float WaterLevel { get; } = 0.25f;
        #endregion
    }
}
