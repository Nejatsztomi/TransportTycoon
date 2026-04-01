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
        public int[,] GenerateForests(MapGenerationContext context) { }
        #endregion
    }
}
