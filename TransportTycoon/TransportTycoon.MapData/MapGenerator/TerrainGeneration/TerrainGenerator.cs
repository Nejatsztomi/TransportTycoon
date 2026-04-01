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
        public int[,] GenerateTerrain(MapGenerationContext context) { }
        #endregion
    }
}
