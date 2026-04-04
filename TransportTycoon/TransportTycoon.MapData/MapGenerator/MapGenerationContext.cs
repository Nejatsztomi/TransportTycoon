namespace TransportTycoon.MapData.MapGenerator
{
    public readonly record struct MapGenerationContext
    {
        #region Properties
        public int Width { get; }
        public int Height { get; }
        public int Seed { get; }
        public MapGenerationSettings Settings { get; }
        #endregion

        public MapGenerationContext(int width, int height, int seed, MapGenerationSettings settings)
        {
            if (width < 0)
            {
                throw new ArgumentException("");
            }

            if (height < 0)
            {
                throw new ArgumentException("");
            }

            Width = width;
            Height = height;
            Seed = seed;
            Settings = settings;
        }
    };
}
