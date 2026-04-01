using System.Diagnostics.Metrics;

namespace TransportTycoon.MapData.MapGenerator
{
    public class MapGenerationSettings
    {
        #region Properties
        public int RiverCount
        {
            get;
            private set
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(RiverCount), "RiverCount must be a non-negative integer.");
                }
                field = value;
            }
        }
        public IBiome Biome { get; private set; }
        public int ForestCount
        {
            get;
            private set
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(ForestCount), "ForestCount must be a non-negative integer.");
                }
                field = value;
            }
        }
        public int StructureRange
        {
            get;
            private set
            {
                if (!(value >= 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(StructureRange), "StructureRange must be a non-negative integer.");
                }
                field = value;
            }
        }
        public int MinCities
        {
            get;
            private set
            {
                if (!(value >= 2))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinCities), "MinCities must be an integer greater than or equal to 2.");
                }
                field = value;
            }
        }
        public int MaxCities
        {
            get;
            private set
            {
                if (!(value >= MinCities))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxCities), "MaxCities must be an integer greater than or equal to MinCities.");
                }
                field = value;
            }
        }
        public int MinStructure
        {
            get;
            private set
            {
                if (!(value > 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(MinStructure), "MinStructure must be a positive integer.");
                }
                field = value;
            }
        }
        public int MaxStructure
        {
            get;
            private set
            {
                if (!(value >= MinStructure))
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxStructure), "MaxStructure must be an integer greater than or equal to MinStructure.");
                }
                field = value;
            }
        }
        #endregion

        #region Constructors
        public MapGenerationSettings(int riverCount, IBiome biome, int forestCount, int structureRange, int minCities, int maxCities, int minStructure, int maxStructure)
        {
            RiverCount = riverCount;
            Biome = biome;
            ForestCount = forestCount;
            StructureRange = structureRange;
            MinCities = minCities;
            MaxCities = maxCities;
            MinStructure = minStructure;
            MaxStructure = maxStructure;
        }
        #endregion
    }
}
