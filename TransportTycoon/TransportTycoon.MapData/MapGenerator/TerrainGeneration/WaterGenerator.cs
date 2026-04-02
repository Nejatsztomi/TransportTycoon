using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterGeneratorFactory
    {
        public static IWaterGenerator Create(INoiseGenerator noiseGenerator, float noiseScale) => new WaterGenerator(noiseGenerator, noiseScale);
    }

    internal class WaterGenerator : IWaterGenerator
    {
        #region Properties
        private INoiseGenerator NoiseGenerator { get; }
        private float NoiseScale { get; }
        #endregion

        #region Constructors
        public WaterGenerator(INoiseGenerator noiseGenerator, float noiseScale)
        {
            NoiseGenerator = noiseGenerator;
            NoiseScale = noiseScale;
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(int _, int[,] heightMap, MapGenerationContext context)
        {
            bool[,] waterMap = new bool[context.Width, context.Height];

            float[,] noiseMap = NoiseGenerator.GenerateNoise(NoiseScale, context);
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
            if (!(x + 1 < context.Width && heightMap[x + 1, y] <= 2)) return false;
            if (!(0 <= x - 1 && heightMap[x - 1, y] <= 2)) return false;
            if (!(y + 1 < context.Height && heightMap[x, y + 1] <= 2)) return false;
            if (!(0 <= y - 1 && heightMap[x, y - 1] <= 2)) return false;
            return true;
        }
        #endregion
    }
}
