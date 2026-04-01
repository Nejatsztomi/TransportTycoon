namespace TransportTycoon.MapData.MapGenerator.NoiseGeneration
{
    public static class PerlinNoiseGeneratorFactory
    {
        public static INoiseGenerator Create(int width, int height, int seed)
        {
            return new PerlinNoiseGenerator(width, height, seed);
        }
    }

    internal class PerlinNoiseGenerator : INoiseGenerator
    {
        #region Properties
        private Random Random { get; }
        private int Height { get; }
        private int Width { get; }
        #endregion

        #region Constructors
        public PerlinNoiseGenerator(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Random = new(seed);
        }
        #endregion

        #region Public methods
        public float[,] GenerateNoise(float noiseScale)
        {
            float[,] map = new float[Width, Height];

            float offsetX = Random.Next(-10000, 10000);
            float offsetY = Random.Next(-10000, 10000);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
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
