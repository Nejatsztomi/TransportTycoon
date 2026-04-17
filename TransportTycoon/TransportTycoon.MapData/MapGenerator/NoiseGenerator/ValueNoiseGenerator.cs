using System.Runtime.CompilerServices;
using TransportTycoon.MapData.MapGenerator.CoordinateHasher;

namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    public static class ValueNoiseGeneratorFactory
    {
        public static INoiseGenerator Create(float frequency) => new ValueNoiseGenerator(frequency);
    }

    internal class ValueNoiseGenerator : INoiseGenerator
    {
        #region Private fields
        private readonly float _frequency;
        private readonly ICoordinateHasher _hasher = new BasicCoordinateHasher();
        #endregion

        #region Constructors
        public ValueNoiseGenerator(float frequency = 0.05f)
        {
            _frequency = frequency;
        }
        #endregion

        #region Public methods
        public float GenerateNoise(float x, float y, int seed)
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
        #endregion

        #region Private methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Lerp(float a, float b, float t) => a + t * (b - a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Fade(float t) => t * t * (3f - 2f * t);
        #endregion
    }
}
