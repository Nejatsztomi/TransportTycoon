using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class TerraingGeneratorFactory
    {
        public static ITerrainGenerator Create(INoiseGenerator noiseGenerator) => new TerrainGenerator(noiseGenerator);
    }

    internal class TerrainGenerator : ITerrainGenerator
    {
        #region Private fields
        private readonly INoiseGenerator _noiseGenerator;
        #endregion

        #region Constructors
        public TerrainGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public int[,] GenerateTerrain(MapGenerationContext context)
        {
            int[,] heightMap = new int[context.Width, context.Height];

            float[,] randomNoiseMap = GenerateRandomNoiseMap(context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Replaces magic numbers with TerrainHight enum
                    if (randomNoiseMap[i, j] < context.Settings.Biome.WaterRange)
                    {
                        heightMap[i, j] = 0;
                    }
                    else if (randomNoiseMap[i, j] < context.Settings.Biome.PlainRange)
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

        #region Private methods
        private float[,] GenerateRandomNoiseMap(MapGenerationContext context)
        {
            float[,] noiseMap = new float[context.Width, context.Height];
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    noiseMap[i, j] = _noiseGenerator.GenerateNoise(i, j, context.Seed + 10);
                }
            }
            return noiseMap;
        }
        #endregion
    }
}
