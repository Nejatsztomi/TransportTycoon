namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    /// <summary>
    /// A factory class for creating instances of <see cref="ITerrainGenerator"/>.
    /// This class provides a method to create a new instance of a terrain generator, which is responsible for generating terrain data based on a noise map and map generation context. The factory pattern is used here to encapsulate the creation logic and allow for easy substitution of different terrain generator implementations in the future if needed.
    /// </summary>
    public static class TerraingGeneratorFactory
    {
        /// <summary>
        /// Creates a new instance of an ITerrainGenerator for generating terrain based on the provided noise map and map generation context.
        /// </summary>
        /// <returns>A new instance of <see cref="ITerrainGenerator"/>.</returns>
        public static ITerrainGenerator Create() => new TerrainGenerator();
    }

    internal class TerrainGenerator : ITerrainGenerator
    {
        #region Public properties
        public GenerationPhase Phase => GenerationPhase.BaseTerrain;
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
