namespace TransportTycoon.MapData.MapGenerator
{
    public readonly record struct MapGenerationContext
    {
        #region Properties
        public int Width { get; } = 100;
        public int Height { get; } = 100;
        public int Seed { get; } = 0;
        public MapGenerationSettings Settings { get; }
        #endregion

        public MapGenerationContext(int width, int height, int seed, MapGenerationSettings settings)
        {
            if (width < 0)
            {
                throw new ArgumentException("Width must be a positive integer.");
            }

            if (height < 0)
            {
                throw new ArgumentException("Height must be a positive integer.");
            }

            Width = width;
            Height = height;
            Seed = seed;
            Settings = settings;
        }
    };
}
