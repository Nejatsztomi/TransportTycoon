namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    public static class PerlinNoiseGeneratorFactory
    {
        public static INoiseGenerator Create(IRandomProvider randomProvider, MapGenerationContext context) => new PerlinNoiseGenerator(randomProvider, context);
    }

    internal class PerlinNoiseGenerator : INoiseGenerator
    {
        #region Private fields
        private readonly IRandom _random;
        private readonly float _offsetX;
        private readonly float _offsetY;
        private readonly float _noiseScale = 0.05f;
        #endregion

        #region Constructors
        public PerlinNoiseGenerator(IRandomProvider randomProvider, MapGenerationContext context)
        {
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Noise);
            _offsetX = _random.Next(-10000, 10000);
            _offsetY = _random.Next(-10000, 10000);
        }
        #endregion

        #region Public methods
        public float GenerateNoise(float x, float y, int _)
        {
            // Calculate normalized noise (0.0 to 1.0)
            return CalculateNoise((x + _offsetX) * _noiseScale, (y + _offsetY) * _noiseScale);
        }
        #endregion

        #region Private methods
        private float CalculateNoise(float x, float y)
        {
            double val = (Math.Sin(x) + Math.Sin(y) + Math.Sin(x + y) + Math.Cos(x - y)) / 4.0;
            return (float)((val + 1.0) / 2.0);
        }
        #endregion
    }
}
