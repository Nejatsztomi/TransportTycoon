using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class TerraingGeneratorFactory
    {
        public static ITerrainGenerator Create(INoiseGenerator noiseGenerator) => new TerrainGenerator(noiseGenerator);
    }

    internal class TerrainGenerator : ITerrainGenerator
    {
        #region Properties
        private INoiseGenerator NoiseGenerator { get; }
        #endregion

        #region Constructors
        public TerrainGenerator(INoiseGenerator noiseGenerator)
        {
            NoiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public int[,] GenerateTerrain(IBiome biome, MapGenerationContext context)
        {
            int[,] heightMap = new int[context.Width, context.Height];

            float[,] randomNoiseMap = NoiseGenerator.GenerateNoise(0.1f, context);
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Replaces magic numbers with TerrainHight enum
                    if (randomNoiseMap[i, j] < biome.PlainRange)
                    {
                        heightMap[i, j] = 1;
                    }
                    else if (randomNoiseMap[i, j] < biome.HillRange)
                    {
                        heightMap[i, j] = 2;
                    }
                    else if (randomNoiseMap[i, j] < biome.MountainRange)
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
