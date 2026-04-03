using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterGeneratorFactory
    {
        public static IWaterGenerator Create(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context) => new WaterGenerator(noiseGenerator, randomProvider, context);
    }

    internal class WaterGenerator : IWaterGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public WaterGenerator(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _noiseGenerator = noiseGenerator;
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Rivers);
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(int[,] heightMap, MapGenerationContext context)
        {
            bool[,] waterMap = new bool[context.Width, context.Height];

            float[,] noiseMap = _noiseGenerator.GenerateNoise(context.Settings.WaterNoiseScale, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    if (heightMap[i, j] >= 2) continue;

                    if (noiseMap[i, j] < 0.5f)
                    {
                        waterMap[i, j] = IsValidNeighbouringHeights(i, j, heightMap, context);
                    }
                }
            }

            return waterMap;
        }
        #endregion

        #region Private methods
        private bool IsValidNeighbouringHeights(int x, int y, int[,] heightMap, MapGenerationContext context)
        {
            // TODO: Replace magic number with TerrainHeight enum
            bool valid = true;
            if (x + 1 < context.Width) valid &= heightMap[x + 1, y] <= 2;
            if (0 <= x - 1) valid &= heightMap[x - 1, y] <= 2;
            if (y + 1 < context.Height) valid &= heightMap[x, y + 1] <= 2;
            if (0 <= y - 1) valid &= heightMap[x, y - 1] <= 2;
            return valid;
        }
        #endregion
    }
}
