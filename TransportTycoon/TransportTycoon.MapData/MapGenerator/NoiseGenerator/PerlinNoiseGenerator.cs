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
        #endregion

        #region Constructors
        public PerlinNoiseGenerator(IRandomProvider randomProvider, MapGenerationContext context)
        {
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Noise);
        }
        #endregion

        #region Public methods
        public float[,] GenerateNoise(float noiseScale, MapGenerationContext context)
        {
            float[,] map = new float[context.Width, context.Height];

            float offsetX = _random.Next(-10000, 10000);
            float offsetY = _random.Next(-10000, 10000);

            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
                {
                    // Calculate normalized noise (0.0 to 1.0)
                    map[x, y] = CalculateNoise((x + offsetX) * noiseScale, (y + offsetY) * noiseScale);
                }
            }
            return map;
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
