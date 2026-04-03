using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class TerraingGeneratorFactory
    {
        public static ITerrainGenerator Create(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context) => new TerrainGenerator(noiseGenerator, randomProvider, context);
    }

    internal class TerrainGenerator : ITerrainGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public TerrainGenerator(INoiseGenerator noiseGenerator, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _noiseGenerator = noiseGenerator;
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Terrain);
        }
        #endregion

        #region Public methods
        public int[,] GenerateTerrain(MapGenerationContext context)
        {
            int[,] heightMap = new int[context.Width, context.Height];

            float[,] randomNoiseMap = _noiseGenerator.GenerateNoise(context.Settings.TerrainNoiseScale, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Replaces magic numbers with TerrainHight enum
                    if (randomNoiseMap[i, j] < context.Settings.Biome.PlainRange)
                    {
                        heightMap[i, j] = 1;
                    }
                    else if (randomNoiseMap[i, j] < context.Settings.Biome.HillRange)
                    {
                        heightMap[i, j] = 2;
                    }
                    else if (randomNoiseMap[i, j] < context.Settings.Biome.MountainRange)
                    {
                        heightMap[i, j] = 3;
                    }
                    else
                    {
                        heightMap[i, j] = 4;
                    }
                }
            }

            return heightMap;
        }
        #endregion
    }
}
