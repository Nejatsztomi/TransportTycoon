namespace TransportTycoon.MapData.MapGenerator
{
    public interface IBiome
    {
        #region Properties
        string Id { get; }
        public float WaterRange { get; }
        public float PlainRange { get; }
        public float HillRange { get; }
        public float MountainRange { get; }
        public float HighMountainRange { get; }
        #endregion
    }
}
