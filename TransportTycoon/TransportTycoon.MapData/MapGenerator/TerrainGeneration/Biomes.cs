namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class Biomes
    {
        public static readonly IBiome Default = new DefaultBiome();
        public static readonly IBiome Flat = new FlatBiome();
        public static readonly IBiome Mountainous = new MountainousBiome();
    }

    internal readonly record struct DefaultBiome : IBiome
    {
        public float WaterRange => 0.3f;
        public float PlainRange => 0.55f;
        public float HillRange => 0.75f;
        public float MountainRange => 0.95f;
        public float HighMountainRange => 1f;
    }

    internal readonly record struct FlatBiome : IBiome
    {
        public float WaterRange => 0.2f;
        public float PlainRange => 0.80f;
        public float HillRange => 0.90f;
        public float MountainRange => 0.95f;
        public float HighMountainRange => 1f;
    }

    internal readonly record struct MountainousBiome : IBiome
    {
        public float WaterRange => 0.2f;
        public float PlainRange => 0.38f;
        public float HillRange => 0.70f;
        public float MountainRange => 0.88f;
        public float HighMountainRange => 1f;
    }
}
