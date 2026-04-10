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

        #region Constructors
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

        public MapGenerationContext()
        {
            Width = 100;
            Height = 100;
            Seed = 0;
            Settings = new();
        }
        #endregion
    };
}
