namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// Provides contextual information and data structures used during procedural map generation, including map
    /// dimensions, random seed, generation settings, and various map layers.
    /// </summary>
    /// <remarks>This struct encapsulates all relevant parameters and generated data required for map
    /// generation algorithms. It is typically used to pass state and intermediate results between different stages of
    /// the map generation process. All map layer arrays are initialized based on the specified width and height. The
    /// struct is immutable except for the contents of the arrays, which may be modified during generation.</remarks>
    public readonly record struct MapGenerationContext
    {
        #region Properties
        /// <summary>
        /// The map's width in tiles. Must be a positive integer.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The map's height in tiles. Must be a positive integer.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Represents the initial value used to seed a random number generator or similar operation.
        /// </summary>
        public readonly int Seed;

        /// <summary>
        /// Gets the settings used for map generation.
        /// </summary>
        public readonly MapGenerationSettings Settings;

        /// <summary>
        /// Gets the two-dimensional array representing the generated noise values.
        /// </summary>
        public float[,] NoiseMap { get; }

        /// <summary>
        /// Gets the two-dimensional array representing the height values of the terrain or surface.
        /// </summary>
        /// <remarks>Each element in the array corresponds to the height at a specific coordinate. The
        /// dimensions of the array represent the width and length of the mapped area.</remarks>
        public int[,] HeightMap { get; }

        /// <summary>
        /// Gets the two-dimensional boolean array indicating the presence of water at each coordinate.
        /// </summary>
        public bool[,] WaterMap { get; }

        /// <summary>
        /// Gets the two-dimensional array representing the forest map.
        /// </summary>
        /// <remarks>Each element in the array corresponds to a cell in the forest, where the value may
        /// represent terrain type, occupancy, or other domain-specific information depending on the implementation
        /// context. The dimensions of the array indicate the size of the forest grid.</remarks>
        public int[,] ForestMap { get; }

        /// <summary>
        /// Gets the two-dimensional structure map represented as a Boolean array.
        /// </summary>
        /// <remarks>Each element in the array indicates the presence or absence of a structure at the
        /// corresponding position. The first dimension represents rows, and the second dimension represents
        /// columns.</remarks>
        public bool[,] StructureMap { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MapGenerationContext class with the specified map dimensions, random seed,
        /// and generation settings.
        /// </summary>
        /// <param name="width">The width of the map, in tiles. Must be a non-negative integer.</param>
        /// <param name="height">The height of the map, in tiles. Must be a non-negative integer.</param>
        /// <param name="seed">The random seed used for map generation. Determines the reproducibility of generated maps.</param>
        /// <param name="settings">The settings that control map generation parameters and behavior. Cannot be null.</param>
        /// <exception cref="ArgumentException">Thrown if width or height is negative.</exception>
        public MapGenerationContext(int width, int height, int seed, MapGenerationSettings settings)
        {
            if (width < 0) throw new ArgumentException("Width must be a positive integer.");

            if (height < 0) throw new ArgumentException("Height must be a positive integer.");

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

        /// <summary>
        /// The default constructor initializes a new instance of the MapGenerationContext class with default values for map dimensions, random seed, and generation settings.
        /// The default width and height are set to 100 tiles each, the seed is set to 0, and the settings are initialized with a new instance of MapGenerationSettings. This constructor allows for quick instantiation of a MapGenerationContext with standard parameters, which can be useful for testing or when specific configuration is not required.
        /// </summary>
        public MapGenerationContext() : this(100, 100, 0, new MapGenerationSettings()) { }

        /// <summary>
        /// A conversion constructor that initializes a new instance of the MapGenerationContext class using data from a MapGenerationContextData object.
        /// This constructor allows for easy creation of a MapGenerationContext from a data transfer object (DTO), which can be useful for scenarios such as deserialization or when transferring context data between different layers of an application.
        /// The properties of the MapGenerationContext are set based on the corresponding values in the provided MapGenerationContextData instance.
        /// </summary>
        /// <param name="data"></param>
        public MapGenerationContext(MapGenerationContextData data) : this(data.Width, data.Height, data.Seed, data.Settings) { }
        #endregion
    };

    /// <summary>
    /// A DTO object for MapGenerationContext, used for serialization and deserialization of map generation context data.
    /// </summary>

    public readonly record struct MapGenerationContextData
    {
        #region Properties
        /// <summary>
        /// The map's width in tiles. Must be a positive integer.
        /// </summary>
        public int Width
        {
            get;
            init
            {
                if (value <= 0) throw new ArgumentException("Width must be a positive integer.");
                field = value;
            }
        }

        /// <summary>
        /// The map's height in tiles. Must be a positive integer.
        /// </summary>
        public int Height
        {
            get;
            init
            {
                if (value <= 0) throw new ArgumentException("Height must be a positive integer.");
                field = value;
            }
        }

        /// <summary>
        /// Gets the seed value used for initializing random number generation or reproducible operations.
        /// </summary>
        public int Seed { get; init; }

        /// <summary>
        /// Gets the configuration settings used for map generation.
        /// </summary>
        public MapGenerationSettings Settings { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MapGenerationContextData class with the specified width, height, seed, and settings.
        /// </summary>
        /// <param name="width">The width of the map in tiles. Must be a positive integer.</param>
        /// <param name="height">The height of the map in tiles. Must be a positive integer.</param>
        /// <param name="seed">The seed value for random number generation.</param>
        /// <param name="settings">The configuration settings for map generation.</param>
        public MapGenerationContextData(
            int width,
            int height,
            int seed,
            MapGenerationSettings settings
        )
        {
            Width = width;
            Height = height;
            Seed = seed;
            Settings = settings;
        }

        /// <summary>
        /// A conversion constructor that initializes a new instance of the MapGenerationContextData class using data from a MapGenerationContext object.
        /// This constructor allows for easy creation of a MapGenerationContextData from a MapGenerationContext, which can be useful for scenarios such as serialization or when transferring context data between different layers of an application.
        /// The properties of the MapGenerationContextData are set based on the corresponding values in the provided MapGenerationContext instance.
        /// </summary>
        /// <param name="context">The MapGenerationContext instance from which to initialize the MapGenerationContextData.</param>
        public MapGenerationContextData(MapGenerationContext context) : this(context.Width, context.Height, context.Seed, context.Settings) { }
        #endregion
    }
}
