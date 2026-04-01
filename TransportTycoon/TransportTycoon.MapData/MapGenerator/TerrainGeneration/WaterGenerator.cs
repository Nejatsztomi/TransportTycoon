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
            bool[,] waterMap = new bool[context.Height, context.Width];

            float[,] noiseMap = NoiseGenerator.GenerateNoise(NoiseScale, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    if (heightMap[i, j] > 1) continue;

                    if (noiseMap[i, j] < 0.4f)
                    {
                        waterMap[i, j] = true;
                    }
                }
            }

            return waterMap;
        }
        #endregion
    }
}
