namespace TransportTycoon.MapData.MapGenerator
{
    public readonly record struct MapGenerationContext
    {
        #region Properties
        public readonly int Width;
        public readonly int Height;
        public readonly int Seed;
        public readonly MapGenerationSettings Settings;

        public float[,] NoiseMap { get; }
        public int[,] HeightMap { get; }
        public bool[,] WaterMap { get; }
        public int[,] ForestMap { get; }
        public bool[,] StructureMap { get; }
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

            NoiseMap = new float[width, height];
            HeightMap = new int[width, height];
            WaterMap = new bool[width, height];
            ForestMap = new int[width, height];
            StructureMap = new bool[width, height];
        }

        public MapGenerationContext() : this(100, 100, 0, new MapGenerationSettings()) { }
        public MapGenerationContext(MapGenerationContextData data) : this(data.Width, data.Height, data.Seed, data.Settings) { }
        #endregion
    };

    /// <summary>
    /// A DTO object for MapGenerationContext, used for serialization and deserialization of map generation context data.
    /// </summary>
    /// <param name="Width"></param>
    /// <param name="Height"></param>
    /// <param name="Seed"></param>
    /// <param name="Settings"></param>

    public readonly record struct MapGenerationContextData(
        int Width,
        int Height,
        int Seed,
        MapGenerationSettings Settings
    )
    {
        #region Constructors
        public MapGenerationContextData(MapGenerationContext context) : this(context.Width, context.Height, context.Seed, context.Settings) { }
        #endregion
    }
}
