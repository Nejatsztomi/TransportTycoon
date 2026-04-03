using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class ForestGeneratorFactory
    {
        public static IForestGenerator Create(INoiseGenerator noiseGenerator) => new ForestGenerator(noiseGenerator);
    }

    internal class ForestGenerator : IForestGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        #endregion

        #region Constructors
        public ForestGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context)
        {
            int[,] forestMap = new int[context.Width, context.Height];

            float[,] randomTreeNoiseMap = GenerateRandomNoiseMap(context);
            float forestPercentage = 1 - context.Settings.ForestPercentage;
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Don't use magic number, later on there will a TerrainHeight enum
                    if (heightMap[i, j] >= 4) continue;

                    if (randomTreeNoiseMap[i, j] < forestPercentage) continue;

                    if (randomTreeNoiseMap[i, j] < forestPercentage + forestPercentage / 4)
                    {
                        forestMap[i, j] = 1;
                    }
                    else if (randomTreeNoiseMap[i, j] < forestPercentage + 2 * forestPercentage / 4)
                    {
                        forestMap[i, j] = 2;
                    }
                    else if (randomTreeNoiseMap[i, j] < forestPercentage + 3 * forestPercentage / 4)
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

        #region Private methods
        private float[,] GenerateRandomNoiseMap(MapGenerationContext context)
        {
            float[,] noiseMap = new float[context.Width, context.Height];
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    noiseMap[i, j] = _noiseGenerator.GenerateNoise(i, j, context.Seed + 50);
                }
            }
            return noiseMap;
        }
        #endregion
    }
}
