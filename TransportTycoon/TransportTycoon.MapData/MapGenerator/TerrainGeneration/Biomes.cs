namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class Biomes
    {
        public static IBiome Default { get; } = new DefaultBiome();
    }

    internal class DefaultBiome : IBiome
    {
        public float PlainRange => 0.55f;
        public float HillRange => 0.75f;
        public float MountainRange => 0.95f;
        public float HighMountainRange => 1f;
    }
}
