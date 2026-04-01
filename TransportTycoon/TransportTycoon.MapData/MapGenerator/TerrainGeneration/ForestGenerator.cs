using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class ForestGeneratorFactory
    {
        public static IForestGenerator Create(INoiseGenerator noiseGenerator) => new ForestGenerator(noiseGenerator);
    }

    internal class ForestGenerator : IForestGenerator
    {
        #region Properties
        private INoiseGenerator NoiseGenerator { get; }
        #endregion

        #region Constructors
        public ForestGenerator(INoiseGenerator noiseGenerator)
        {
            NoiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context)
        {
            int[,] forestMap = new int[context.Width, context.Height];

            float[,] randomTreeNoiseMap = NoiseGenerator.GenerateNoise(0.1f, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Don't use magic number, later on there will a TerrainHeight enum
                    if (heightMap[i, j] >= 4) continue;

                    if (randomTreeNoiseMap[i, j] < 0.5f) continue;

                    if (randomTreeNoiseMap[i, j] < 0.75f)
                    {
                        forestMap[i, j] = 1;
                    }
                    else if (randomTreeNoiseMap[i, j] < 0.85f)
                    {
                        forestMap[i, j] = 2;
                    }
                    else if (randomTreeNoiseMap[i, j] < 0.95f)
                    {
                        forestMap[i, j] = 3;
                    }
                    else
                    {
                        forestMap[i, j] = 4;
                    }
                }
            }

            return forestMap;
        }
        #endregion
    }
}
