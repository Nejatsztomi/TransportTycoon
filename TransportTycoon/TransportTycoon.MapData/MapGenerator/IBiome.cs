namespace TransportTycoon.MapData.MapGenerator
{
    public interface IBiome
    {
        public float PlainRange { get; }
        public float HillRange { get; }
        public float MountainRange { get; }
        public float HighMountainRange { get; }
    }
}
