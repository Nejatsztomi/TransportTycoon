using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class ForestGeneratorFactory
    {
        public static IForestGenerator Create(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context) => new ForestGenerator(noiseGenerator, randomProvider, context);
    }

    internal class ForestGenerator : IForestGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public ForestGenerator(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _noiseGenerator = noiseGenerator;
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Forests);
        }
        #endregion

        #region Public methods
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context)
        {
            int[,] forestMap = new int[context.Width, context.Height];

            float[,] randomTreeNoiseMap = _noiseGenerator.GenerateNoise(context.Settings.ForestNoiseScale, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Don't use magic number, later on there will a TerrainHeight enum
                    if (heightMap[i, j] >= 4) continue;

                    if (randomTreeNoiseMap[i, j] < context.Settings.ForestPercentage) continue;

                    if (randomTreeNoiseMap[i, j] < context.Settings.ForestPercentage + context.Settings.ForestPercentage / 4)
                    {
                        forestMap[i, j] = 1;
                    }
                    else if (randomTreeNoiseMap[i, j] < context.Settings.ForestPercentage + 2 * context.Settings.ForestPercentage / 4)
                    {
                        forestMap[i, j] = 2;
                    }
                    else if (randomTreeNoiseMap[i, j] < context.Settings.ForestPercentage + 3 * context.Settings.ForestPercentage / 4)
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
