using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    public static class MapGenerationSettingsDefaults
    {
        public const int RiverCount = 3;
        public const float ForestPercentage = 0.3f;
        public const int StructureRange = 0;
        public const int MinCities = 2;
        public const int MaxCities = 2;
        public const int MinStructure = 1;
        public const int MaxStructure = 3;
        public const float TerrainNoiseScale = 0.1f;
        public const float ForestNoiseScale = 0.1f;
        public const float WaterNoiseScale = 0.1f;

        public static readonly IBiome Biome = Biomes.Default;
    }

    public class MapGenerationSettings
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
        public int StructureRange
        {
            get;
            init
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(StructureRange), "StructureRange must be a non-negative integer.");
                }
                field = value;
            }
        } = MapGenerationSettingsDefaults.StructureRange;
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
