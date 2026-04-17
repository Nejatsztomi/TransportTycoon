using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class LakeGeneratorFactory
    {
        public static IWaterGenerator Create(INoiseGenerator noiseGenerator) => new LakeGenerator(noiseGenerator);
    }

    internal class LakeGenerator : IWaterGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        #endregion

        #region Constructors
        public LakeGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(float[,] heightMap, bool[,] waterMap, MapGenerationContext context)
        {
            float[,] noiseMap = GenerateRandomNoiseMap(context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    if (waterMap[i, j]) continue;
                    if (noiseMap[i, j] >= 2) continue;

                    if (noiseMap[i, j] < context.Settings.WaterBiome.WaterLevel)
                    {
                        waterMap[i, j] = IsValidNeighbouringHeights(i, j, heightMap, context);
                    }
                }
            }

            return waterMap;
        }
        #endregion

        #region Private methods
        private bool IsValidNeighbouringHeights(int x, int y, float[,] heightMap, MapGenerationContext context)
        {
            // TODO: Replace magic number with TerrainHeight enum
            bool valid = true;
            if (x + 1 < context.Width) valid &= heightMap[x + 1, y] <= context.Settings.Biome.HillRange;
            if (0 <= x - 1) valid &= heightMap[x - 1, y] <= context.Settings.Biome.HillRange;
            if (y + 1 < context.Height) valid &= heightMap[x, y + 1] <= context.Settings.Biome.HillRange;
            if (0 <= y - 1) valid &= heightMap[x, y - 1] <= context.Settings.Biome.HillRange;
            return valid;
        }

        private float[,] GenerateRandomNoiseMap(MapGenerationContext context)
        {
            float[,] noiseMap = new float[context.Width, context.Height];
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    noiseMap[i, j] = _noiseGenerator.GenerateNoise(i, j, context.Seed + 100);
                }
            }
            return noiseMap;
        }
        #endregion
    }
}
