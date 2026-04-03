using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class ForestGeneratorFactory
    {
        public static IForestGenerator Create(INoiseGenerator noiseGenerator, float noiseScale, float forestPercentage, IRandomProvider randomProvider, MapGenerationContext context) => new ForestGenerator(noiseGenerator, noiseScale, forestPercentage, randomProvider, context);
    }

    internal class ForestGenerator : IForestGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        private readonly float _noiseScale;
        private readonly float _forestPercentage;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public ForestGenerator(INoiseGenerator noiseGenerator, float noiseScale, float forestPercentage, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _noiseGenerator = noiseGenerator;
            _noiseScale = noiseScale;
            _forestPercentage = 1 - forestPercentage;
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Forests);
        }
        #endregion

        #region Public methods
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context)
        {
            int[,] forestMap = new int[context.Width, context.Height];

            float[,] randomTreeNoiseMap = _noiseGenerator.GenerateNoise(_noiseScale, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Don't use magic number, later on there will a TerrainHeight enum
                    if (heightMap[i, j] >= 4) continue;

                    if (randomTreeNoiseMap[i, j] < _forestPercentage) continue;

                    if (randomTreeNoiseMap[i, j] < _forestPercentage + _forestPercentage / 4)
                    {
                        forestMap[i, j] = 1;
                    }
                    else if (randomTreeNoiseMap[i, j] < _forestPercentage + 2 * _forestPercentage / 4)
                    {
                        forestMap[i, j] = 2;
                    }
                    else if (randomTreeNoiseMap[i, j] < _forestPercentage + 3 * _forestPercentage / 4)
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
