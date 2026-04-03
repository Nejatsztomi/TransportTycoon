using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    public static class MapGenerationSettingsDefaults
    {
        // Water generation
        public const int RiverCount = 3;
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

    public record MapGenerationSettings
    {
        #region Properties
        #region Water generation
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
        } = MapGenerationSettingsDefaults.RiverCount;
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
        } = MapGenerationSettingsDefaults.MinRiverWidth;
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
        } = MapGenerationSettingsDefaults.MaxRiverWidth;
        public IWaterBiome WaterBiome { get; init; } = MapGenerationSettingsDefaults.WaterBiome;
        public float WaterNoiseScale
        {
            get;
            init
            {
                if (!(0f <= value && value <= 1f))
                {
                    throw new ArgumentOutOfRangeException(nameof(WaterNoiseScale), "WaterNoiseScale a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        } = MapGenerationSettingsDefaults.WaterNoiseScale;
        #endregion

        #region Terrain generation
        public IBiome Biome { get; init; } = MapGenerationSettingsDefaults.Biome;
        public float TerrainNoiseScale
        {
            get;
            init
            {
                if (!(0f <= value && value <= 1f))
                {
                    throw new ArgumentOutOfRangeException(nameof(TerrainNoiseScale), "TerrainNoiseScale a float between 0 and 1 (inclusive)");
                }
                field = value;
            }
        } = MapGenerationSettingsDefaults.TerrainNoiseScale;
        #endregion

        #region Structure generation
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
        } = MapGenerationSettingsDefaults.MinStructure;
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
        } = MapGenerationSettingsDefaults.MaxStructure;
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
        } = MapGenerationSettingsDefaults.MinStructureRange;
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
        } = MapGenerationSettingsDefaults.MaxStructureRange;
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
        } = MapGenerationSettingsDefaults.StructureWidth;
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
        } = MapGenerationSettingsDefaults.StructureHeight;
        #endregion

        #region City generation
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
        } = MapGenerationSettingsDefaults.MinCities;
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
        } = MapGenerationSettingsDefaults.MaxCities;
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
        } = MapGenerationSettingsDefaults.MinCityRange;
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
        } = MapGenerationSettingsDefaults.MaxCityRange;
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
        } = MapGenerationSettingsDefaults.CityWidth;
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
        } = MapGenerationSettingsDefaults.CityHeight;
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
        } = MapGenerationSettingsDefaults.RoadLength;
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
        } = MapGenerationSettingsDefaults.BranchCount;
        #endregion

        #region Forest generation
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
        } = MapGenerationSettingsDefaults.ForestPercentage;

        public float ForestNoiseScale
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
        } = MapGenerationSettingsDefaults.ForestNoiseScale;
        #endregion
        #endregion
    }
}
