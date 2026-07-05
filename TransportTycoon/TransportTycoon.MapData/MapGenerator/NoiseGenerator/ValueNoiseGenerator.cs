using System.Runtime.CompilerServices;
using TransportTycoon.MapData.MapGenerator.CoordinateHasher;

namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    /// <summary>
    /// A factory class for creating instances of <see cref="INoiseGenerator"/> that generate value noise.
    /// </summary>
    public static class ValueNoiseGeneratorFactory
    {
        /// <summary>
        /// Creates a new instance of an object that generates value noise using the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency of the value noise. Higher values result in more rapid changes in the noise pattern.</param>
        /// <returns>An instance of <see cref="INoiseGenerator"/> that generates value noise.</returns>
        public static INoiseGenerator Create(float frequency) => new ValueNoiseGenerator(frequency);
    }

    internal class ValueNoiseGenerator : INoiseGenerator
    {
        #region Private fields
        private readonly float _frequency;
        private readonly ICoordinateHasher _hasher = new BasicCoordinateHasher();
        #endregion

        #region Properties
        public GenerationPhase Phase => GenerationPhase.Noise;
        #endregion

        #region Constructors
        internal ValueNoiseGenerator(float frequency = 0.05f)
        {
            _frequency = frequency;
        }
        #endregion

        #region Public methods
        public float GenerateNoise(float x, float y, int seed)
        {
            return CalculateNoise(x, y, seed);
        }

        public float[,] GenerateNoiseMap(int width, int height, int seed)
        {
            float[,] noiseMap = new float[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noiseMap[i, j] = CalculateNoise(i, j, seed);
                }
            }
            return noiseMap;
        }
        #endregion

        #region Private methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalculateNoise(float x, float y, int seed)
        {
            float scaledX = x * _frequency;
            float scaledY = y * _frequency;

            int xInt = (int)Math.Floor(scaledX);
            int yInt = (int)Math.Floor(scaledY);

            float xFrac = scaledX - xInt;
            float yFrac = scaledY - yInt;

            float top_left = _hasher.Hash2D(xInt, yInt, seed);
            float top_right = _hasher.Hash2D(xInt + 1, yInt, seed);
            float bottom_left = _hasher.Hash2D(xInt, yInt + 1, seed);
            float bottom_right = _hasher.Hash2D(xInt + 1, yInt + 1, seed);

            float u = Fade(xFrac);
            float v = Fade(yFrac);

            float top_blend = Lerp(top_left, top_right, u);
            float bottom_blend = Lerp(bottom_left, bottom_right, u);

            return Lerp(top_blend, bottom_blend, v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Lerp(float a, float b, float t) => a + t * (b - a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Fade(float t) => t * t * (3f - 2f * t);
        #endregion
    }
}
