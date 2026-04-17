namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class TerraingGeneratorFactory
    {
        public static ITerrainGenerator Create() => new TerrainGenerator();
    }

    internal class TerrainGenerator : ITerrainGenerator
    {
        #region Public properties
        public GenerationPhase Phase => GenerationPhase.BaseTerrain;
        #endregion

        #region Constructors
        public TerrainGenerator() { }
        #endregion

        #region Public methods
        public int[,] GenerateTerrain(float[,] noiseMap, MapGenerationContext context)
        {
            int[,] heightMap = new int[context.Width, context.Height];

            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    // TODO: Replaces magic numbers with TerrainHight enum
                    if (noiseMap[i, j] < context.Settings.Biome.WaterRange)
                    {
                        heightMap[i, j] = 0;
                    }
                    else if (noiseMap[i, j] < context.Settings.Biome.PlainRange)
                    {
                        heightMap[i, j] = 1;
                    }
                    else if (noiseMap[i, j] < context.Settings.Biome.HillRange)
                    {
                        heightMap[i, j] = 2;
                    }
                    else if (noiseMap[i, j] < context.Settings.Biome.MountainRange)
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
