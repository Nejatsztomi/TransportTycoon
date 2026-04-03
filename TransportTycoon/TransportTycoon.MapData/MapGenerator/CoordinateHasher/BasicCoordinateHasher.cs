namespace TransportTycoon.MapData.MapGenerator.CoordinateHasher
{
    public class BasicCoordinateHasher : ICoordinateHasher
    {
        #region Constants
        private const uint PRIME1 = 198491317;
        private const uint PRIME2 = 6542989;
        private const uint PRIME3 = 357239;
        #endregion

        #region Public methods
        /// <summary>
        /// Computes a deterministic 2D hash value for the specified coordinates and seed.
        /// </summary>
        /// <param name="x">The X coordinate to hash.</param>
        /// <param name="y">The Y coordinate to hash.</param>
        /// <param name="seed">The seed used to vary the generated hash.</param>
        /// <returns>A pseudo-random floating-point value in the range <c>0.0f</c> to <c>1.0f</c>.</returns>
        public float Hash2D(int x, int y, int seed)
        {
            unchecked
            {
                uint hash = (uint)seed;
                hash ^= (uint)x * PRIME1;
                hash ^= (uint)y * PRIME2;

                hash ^= hash >> 16;
                hash *= PRIME3;
                hash ^= hash >> 15;
                hash *= PRIME1;
                hash ^= hash >> 16;

                // Convert to float between 0.0f and 1.0f
                return (float)hash / uint.MaxValue;
            }
        }

        /// <summary>
        /// Computes a deterministic pseudo-random integer for the specified 2D coordinates and seed, constrained to the provided range.
        /// </summary>
        /// <param name="x">The X coordinate used as part of the hash input.</param>
        /// <param name="y">The Y coordinate used as part of the hash input.</param>
        /// <param name="seed">The seed value used to vary the generated result deterministically.</param>
        /// <param name="min">The inclusive lower bound of the target range.</param>
        /// <param name="max">The exclusive upper bound of the target range.</param>
        /// <returns>
        /// A deterministic pseudo-random integer derived from <paramref name="x"/>, <paramref name="y"/>,
        /// and <paramref name="seed"/>, mapped to the range from <paramref name="min"/> to
        /// <paramref name="max"/> (upper bound exclusive).
        /// </returns>
        public int Hash2DRange(int x, int y, int seed, int min, int max)
        {
            float hashFloat = Hash2D(x, y, seed);
            return min + (int)(hashFloat * (max - min));
        }
    }
        #endregion
}
