using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class WaterGeneratorFactory
    {
        public static IWaterGenerator Create(INoiseGenerator noiseGenerator) => new WaterGenerator(noiseGenerator);
    }

    internal class WaterGenerator : IWaterGenerator
    {
        #region Properties
        private INoiseGenerator NoiseGenerator { get; }
        #endregion

        #region Constructors
        public WaterGenerator(INoiseGenerator noiseGenerator)
        {
            NoiseGenerator = noiseGenerator;
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(int riverCount, int[,] heightMap, MapGenerationContext context) { }
        #endregion
    }
}
