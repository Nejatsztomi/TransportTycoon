namespace TransportTycoon.Model.MapGenerator
{
    public static class PerlinNoiseMapGeneratorFactory
    {
        public static IMapGenerator Create(int width, int height, int seed)
        {
            return new PerlinNoiseMapGenerator(width, height, seed);
        }
    }

    internal class PerlinNoiseMapGenerator : IMapGenerator
    {
        #region Private constants
        private const int Water = 0;
        private const int Plain = 1;
        private const int Hills = 2;
        private const int Mountains = 3;
        private const int HighMountains = 4;
        #endregion

        #region Properties
        private Random Random { get; }
        private int Height { get; }
        private int Width { get; }
        #endregion

        #region Constructors
        public PerlinNoiseMapGenerator(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Random = new Random(seed);
        }
        #endregion

        #region Public methods
        public int[,] GenerateMap(float noiseScale)
        {
            int[,] map = new int[Width, Height];

            float offsetX = Random.Next(-10000, 10000);
            float offsetY = Random.Next(-10000, 10000);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Calculate normalized noise (0.0 to 1.0)
                    float noiseValue = CalculateNoise((x + offsetX) * noiseScale, (y + offsetY) * noiseScale);

                    if (noiseValue < 0.35f)
                    {
                        map[x, y] = Water;          // Bottom 35% of heights become water
                    }
                    else if (noiseValue < 0.55f)
                    {
                        map[x, y] = Plain;          // Next 20% become plains
                    }
                    else if (noiseValue < 0.75f)
                    {
                        map[x, y] = Hills;          // Next 20% become hills
                    }
                    else if (noiseValue < 0.90f)
                    {
                        map[x, y] = Mountains;      // Next 15% become mountains
                    }
                    else
                    {
                        map[x, y] = HighMountains;  // Top 10% become high mountains
                    }
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
