using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// A class for defining default values for map generation settings.
    /// This class provides a centralized location for all default parameters related to the generation of various map features, such as water bodies, forests, cities, structures, and terrain.
    /// By using these defaults, the map generation process can ensure consistent and balanced maps while still allowing for customization through the <see cref="MapGenerationSettings"/> record.
    /// </summary>
    public static class MapGenerationSettingsDefaults
    {
        // Water generation
        public const int RiverCount = 10;
        public const int MinRiverWidth = 2;
        public const int MaxRiverWidth = 5;
        public static readonly IWaterBiome WaterBiome = WaterBiomes.Normal;
        public const float WaterNoiseScale = 0.1f;

        // Forest generation
        public const float ForestPercentage = 0.3f;
        public const float ForestNoiseScale = 0.1f;

        // City generation
        public const int MinCities = 2;
        public const int MaxCities = 2;
        public const int MinCityRange = 0;
        public const int MaxCityRange = 0;
        public const int CityWidth = 5;
        public const int CityHeight = 5;
        public const int RoadLength = 10;
        public const int BranchCount = 3;

        // Structure generation
        public const int MinStructure = 1;
        public const int MaxStructure = 3;
        public const int MinStructureRange = 0;
        public const int MaxStructureRange = 0;
        public const int StructureWidth = 2;
        public const int StructureHeight = 2;

        // Terrain generation
        public static readonly IBiome Biome = Biomes.Default;
        public const float TerrainNoiseScale = 0.1f;
    }

    /// <summary>
    /// A record type that encapsulates various settings and parameters used for procedural map generation, including water generation, terrain generation, structure generation, city generation, and forest generation.
    /// </summary>
    public record MapGenerationSettings
    {
        #region Properties
        #region Water generation
        /// <summary>
        /// Gets the number of rivers to generate on the map.
        /// This value must be a non-negative integer, where 0 indicates that no rivers will be generated.
        /// The river generation algorithm will create the specified number of rivers, each with varying widths and characteristics based on the provided settings.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not a non-negative integer.</exception>
        public int RiverCount
        {
            get;
            init
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(RiverCount), "RiverCount must be a non-negative integer.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the minimum width of rivers to be generated on the map.
        /// Must be a positive integer, where a value of 1 indicates that rivers will be at least 1 tile wide.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not a positive integer.</exception>
        public int MinRiverWidth
        {
            get;
            init
            {
                if (!(value > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinRiverWidth), "MinRiverWidth must be a positive integer.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the maximum width of rivers to be generated on the map.
        /// Must be an integer greater than or equal to the value of <see cref="MinRiverWidth"/>, ensuring that the maximum width is not less than the minimum width.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than <see cref="MinRiverWidth"/>.</exception>
        public int MaxRiverWidth
        {
            get;
            init
            {
                if (!(value >= MinRiverWidth))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxRiverWidth), "MaxRiverWidth must be an integer greater than or equal to MinRiverWidth.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the water biome to be used for river generation on the map.
        /// </summary>
        public IWaterBiome WaterBiome { get; init; }

        /// <summary>
        /// Gets the noise scale used for river generation, which influences the variability and distribution of rivers across the map.
        /// Must be a float value between 0 and 1 (inclusive), where lower values result in more uniform river patterns and higher values create more varied and natural-looking river formations.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not between 0 and 1 (inclusive).</exception>
        public float WaterNoiseScale
        {
            get;
            init
            {
                if (!(0.0f <= value && value <= 1.0f))
                {
                    throw new ArgumentOutOfRangeException(nameof(WaterNoiseScale), "WaterNoiseScale a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        }
        #endregion

        #region Terrain generation
        /// <summary>
        /// Gets the biome to be used for terrain generation on the map, which defines the overall characteristics and distribution of different terrain types such as water, plain, hills, mountains, and high mountains.
        /// </summary>
        public IBiome Biome { get; init; }

        /// <summary>
        /// Gets the noise scale used for terrain generation, which influences the variability and distribution of different terrain types across the map.
        /// Must be a float value between 0 and 1 (inclusive), where lower values result in more uniform terrain patterns and higher values create more varied and natural-looking terrain formations.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not between 0 and 1 (inclusive).</exception>
        public float TerrainNoiseScale
        {
            get;
            init
            {
                if (!(0.0f <= value && value <= 1.0f))
                {
                    throw new ArgumentOutOfRangeException(nameof(TerrainNoiseScale), "TerrainNoiseScale a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        }
        #endregion

        #region Structure generation
        /// <summary>
        /// Gets the minimum number of structures to be generated on the map.
        /// Must be a positive integer, where a value of 1 indicates that at least one structure will be generated on the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
        public int MinStructure
        {
            get;
            init
            {
                if (!(value > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinStructure), "MinStructure must be a positive integer.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the maximum number of structures to be generated on the map.
        /// Must be an integer greater than or equal to the value of <see cref="MinStructure"/>, ensuring that the maximum number of structures is not less than the minimum number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than <see cref="MinStructure"/>.</exception>
        public int MaxStructure
        {
            get;
            init
            {
                if (!(value >= MinStructure))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxStructure), "MaxStructure must be an integer greater than or equal to MinStructure.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the minimum range between structures to be generated on the map, which defines the minimum distance (in tiles) that must be maintained between any two structures.
        /// Must be a non-negative integer, where a value of 0 indicates that structures can be placed adjacent to each other without any required spacing. This setting helps to ensure that structures are not clustered too closely together, promoting a more balanced and visually appealing distribution across the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 0.</exception>
        public int MinStructureRange
        {
            get;
            init
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinStructureRange), "MinStructureRange must be an integer greater than or equal to 0.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the maximum allowed structure range value.
        /// Must be an integer greater than or equal to the value of <see cref="MinStructureRange"/>, ensuring that the maximum structure range is not less than the minimum structure range. This setting defines the maximum distance (in tiles) that can be maintained between any two structures, allowing for greater spacing and a more dispersed distribution of structures across the map when set to higher values.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than <see cref="MinStructureRange"/>.</exception>
        public int MaxStructureRange
        {
            get;
            init
            {
                if (!(value >= MinStructureRange))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxStructureRange), "MaxStructureRange must be an integer greater than or equal to MinStructureRange.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the width of structures to be generated on the map, which defines the horizontal size (in tiles) of each structure.
        /// Must be an integer greater than or equal to 2, where a value of 2 indicates that structures will be at least 2 tiles wide.
        /// This setting helps to ensure that structures have a minimum size, contributing to their visual prominence and functional presence on the map.
        /// Larger values will result in wider structures, which can impact the overall layout and design of the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 2.</exception>
        public int StructureWidth
        {
            get;
            init
            {
                if (!(value >= 2))
                {
                    throw new ArgumentOutOfRangeException(nameof(StructureWidth), "StructureWidth must be a greater than or equal to 2.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the height of the structure.
        /// Must be an integer greater than or equal to 2, where a value of 2 indicates that structures will be at least 2 tiles tall.
        /// This setting helps to ensure that structures have a minimum vertical size, contributing to their visual prominence and functional presence on the map.
        /// Larger values will result in taller structures, which can impact the overall layout and design of the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 2.</exception>
        public int StructureHeight
        {
            get;
            init
            {
                if (!(value >= 2))
                {
                    throw new ArgumentOutOfRangeException(nameof(StructureHeight), "StructureHeight must be a greater than or equal to 2.");
                }
                field = value;
            }
        }
        #endregion

        #region City generation
        /// <summary>
        /// Gets the minimum number of cities to be generated on the map.
        /// Must be an integer greater than or equal to 2, where a value of 2 indicates that at least two cities will be generated on the map.
        /// This setting ensures that there are multiple cities present, allowing for more complex interactions and a richer gameplay experience.
        /// Higher values will result in more cities being generated, which can increase the complexity and diversity of the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 2.</exception>
        public int MinCities
        {
            get;
            init
            {
                if (!(value >= 2))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinCities), "MinCities must be an integer greater than or equal to 2.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the maximum number of cities allowed.
        /// Must be an integer greater than or equal to the value of <see cref="MinCities"/>, ensuring that the maximum number of cities is not less than the minimum number of cities.
        /// This setting allows for a variable number of cities to be generated on the map, providing flexibility in map design and gameplay dynamics.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException" >Thrown when the value is less than <see cref="MinCities"/>.</exception>
        public int MaxCities
        {
            get;
            init
            {
                if (!(value >= MinCities))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxCities), "MaxCities must be an integer greater than or equal to MinCities.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the minimum allowed range for a city, in units defined by the application.
        /// Must be a non-negative integer, where a value of 0 indicates that cities can be placed adjacent to each other without any required spacing.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 0.</exception>
        public int MinCityRange
        {
            get;
            init
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinCityRange), "MinCityRange must be an integer greater than or equal to 0.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the maximum allowed range for a city, in units defined by the application.
        /// Must be an integer greater than or equal to the value of <see cref="MinCityRange"/>, ensuring that the maximum city range is not less than the minimum city range.
        /// This setting defines the maximum distance that can be maintained between cities, allowing for greater spacing and a more dispersed distribution of cities across the map when set to higher values.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than <see cref="MinCityRange"/>.</exception>
        public int MaxCityRange
        {
            get;
            init
            {
                if (!(value >= MinCityRange))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxCityRange), "MaxCityRange must be an integer greater than or equal to MinCityRange.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the width of the city in units.
        /// Must be an integer greater than or equal to 3, where a value of 3 indicates that cities will be at least 3 tiles wide.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 3.</exception>
        public int CityWidth
        {
            get;
            init
            {
                if (!(value >= 3))
                {
                    throw new ArgumentOutOfRangeException(nameof(CityWidth), "CityWidth must be a greater than or equal to 3.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the height of the city, which defines the vertical size (in tiles) of each city to be generated on the map.
        /// Must be an integer greater than or equal to 3, where a value of 3 indicates that cities will be at least 3 tiles tall.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 3.</exception>
        public int CityHeight
        {
            get;
            init
            {
                if (!(value > 3))
                {
                    throw new ArgumentOutOfRangeException(nameof(CityHeight), "CityHeight must be a greater than or equal to 3.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the length of the road in units defined by the application.
        /// Must be a positive integer, where a value of 1 indicates that roads will be at least 1 unit long.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not a positive integer.</exception>
        public int RoadLength
        {
            get;
            init
            {
                if (!(value > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(RoadLength), "RoadCount must be a positive integer.");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the number of branches for each road connecting cities, which defines how many additional roads will branch off from the main road connecting two cities.
        /// Must be a positive integer, where a value of 1 indicates that there will be one main road connecting two cities with no additional branches, while higher values will result in more complex road networks with multiple branches extending from the main road, creating a more intricate and interconnected transportation system between cities on the map.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not a positive integer.</exception>
        public int BranchCount
        {
            get;
            init
            {
                if (!(value > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(BranchCount), "BranchCount must be a positive integer.");
                }
                field = value;
            }
        }
        #endregion

        #region Forest generation
        /// <summary>
        /// Gets the percentage of the map that should be covered by forests, which defines the overall density of forested areas on the generated map.
        /// Must be a float value between 0 and 1 (inclusive), where 0 means no forest coverage and 1 means the entire map is covered by forests.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not between 0 and 1 (inclusive).</exception>
        public float ForestPercentage
        {
            get;
            init
            {
                if (!(0f <= value && value <= 1f))
                {
                    throw new ArgumentOutOfRangeException(nameof(ForestPercentage), "ForestCount a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        }

        /// <summary>
        /// Gets the noise scale used for forest generation, which influences the variability and distribution of forests across the map.
        /// Must be a float value between 0 and 1 (inclusive), where lower values result in more uniform forest patterns and higher values create more varied and natural-looking forest formations.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not between 0 and 1 (inclusive).</exception>
        public float ForestNoiseScale
        {
            get;
            init
            {
                if (!(0f <= value && value <= 1f))
                {
                    throw new ArgumentOutOfRangeException(nameof(ForestNoiseScale), "ForestNoiseScale a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        }
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor initializes a new instance of the MapGenerationSettings record with default values for all properties, as defined in the <see cref="MapGenerationSettingsDefaults"/> class.
        /// </summary>
        public MapGenerationSettings()
        {
            RiverCount = MapGenerationSettingsDefaults.RiverCount;
            MinRiverWidth = MapGenerationSettingsDefaults.MinRiverWidth;
            MaxRiverWidth = MapGenerationSettingsDefaults.MaxRiverWidth;
            WaterBiome = MapGenerationSettingsDefaults.WaterBiome;
            WaterNoiseScale = MapGenerationSettingsDefaults.WaterNoiseScale;
            Biome = MapGenerationSettingsDefaults.Biome;
            TerrainNoiseScale = MapGenerationSettingsDefaults.TerrainNoiseScale;
            MinStructure = MapGenerationSettingsDefaults.MinStructure;
            MaxStructure = MapGenerationSettingsDefaults.MaxStructure;
            MinStructureRange = MapGenerationSettingsDefaults.MinStructureRange;
            MaxStructureRange = MapGenerationSettingsDefaults.MaxStructureRange;
            StructureWidth = MapGenerationSettingsDefaults.StructureWidth;
            StructureHeight = MapGenerationSettingsDefaults.StructureHeight;
            MinCities = MapGenerationSettingsDefaults.MinCities;
            MaxCities = MapGenerationSettingsDefaults.MaxCities;
            MinCityRange = MapGenerationSettingsDefaults.MinCityRange;
            MaxCityRange = MapGenerationSettingsDefaults.MaxCityRange;
            CityWidth = MapGenerationSettingsDefaults.CityWidth;
            CityHeight = MapGenerationSettingsDefaults.CityHeight;
            RoadLength = MapGenerationSettingsDefaults.RoadLength;
            BranchCount = MapGenerationSettingsDefaults.BranchCount;
            ForestPercentage = MapGenerationSettingsDefaults.ForestPercentage;
            ForestNoiseScale = MapGenerationSettingsDefaults.ForestNoiseScale;
        }
        #endregion
    }
}
