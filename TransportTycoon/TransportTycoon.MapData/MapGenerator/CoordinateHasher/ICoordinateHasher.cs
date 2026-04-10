namespace TransportTycoon.MapData.MapGenerator.CoordinateHasher
{
    public interface ICoordinateHasher
    {
        #region Public methods
        /// <summary>
        /// Calculates a deterministic pseudo-random floating-point value from 2D coordinates and a seed.
        /// </summary>
        /// <param name="x">The X-coordinate used as input to the hash function.</param>
        /// <param name="y">The Y-coordinate used as input to the hash function.</param>
        /// <param name="seed">The seed value that influences the generated pseudo-random result.</param>
        /// <returns>A deterministic pseudo-random floating-point value derived from the provided coordinates and seed.</returns>
        public float Hash2D(int x, int y, int seed);

        /// <summary>
        /// Calculates a pseudo-random integer within the specified range based on 2D coordinates and a seed.
        /// </summary>
        /// <param name="x">The X-coordinate used as input to the hash function.</param>
        /// <param name="y">The Y-coordinate used as input to the hash function.</param>
        /// <param name="seed">The seed value that influences the generated pseudo-random result.</param>
        /// <param name="min">The inclusive lower bound of the range for the returned value.</param>
        /// <param name="max">The exclusive upper bound of the range for the returned value.</param>
        /// <returns>An integer greater than or equal to min and less than max, determined by the input coordinates and seed.</returns>
        public int Hash2DRange(int x, int y, int seed, int min, int max);
        #endregion
    }
}
