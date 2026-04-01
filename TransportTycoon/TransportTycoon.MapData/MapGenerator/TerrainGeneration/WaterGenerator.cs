using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterGeneratorFactory
    {
        public static IWaterGenerator Create(INoiseGenerator noiseGenerator) => new WaterGenerator(noiseGenerator);
    }

    internal class WaterGenerator : IWaterGenerator
    {
        #region Properties
        private INoiseGenerator NoiseGenerator { get; }
        #endregion

        #region Constructors
        public WaterGenerator(INoiseGenerator noiseGenerator)
        {
            NoiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(int _, int[,] heightMap, MapGenerationContext context)
        {
            bool[,] waterMap = new bool[context.Height, context.Width];

            float[,] noiseMap = NoiseGenerator.GenerateNoise(0.1f, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    if (heightMap[i, j] > 1) continue;

                    if (noiseMap[i, j] < 0.5f)
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
