namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An interface representing a random number generator, which provides methods for generating random integers, floating-point numbers, and doubles.
    /// This interface serves as an abstraction for random number generation, allowing for different implementations of random number generators to be used in the map generation process, such as pseudo-random generators or more complex algorithms for specific use cases.
    /// </summary>
    public interface IRandom
    {
        #region Public methods
        /// <summary>
        /// Gets a random integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>A random integer.</returns>
        public int Next();

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum value.
        /// </summary>
        /// <remarks>If <paramref name="maxValue"/> is less than or equal to zero, an exception is thrown.
        /// This method is commonly used to generate random numbers within a specific range.</remarks>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. Must be greater than zero.</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to zero, and less than <paramref name="maxValue"/>.</returns>
        public int Next(int maxValue);

        /// <summary>
        /// Returns a random integer that is greater than or equal to the specified minimum value and less than the
        /// specified maximum value.
        /// </summary>
        /// <remarks>If <paramref name="minValue"/> equals <paramref name="maxValue"/>, the method returns
        /// <paramref name="minValue"/>. If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>, an
        /// exception is thrown.</remarks>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. Must be greater than <paramref name="minValue"/>.</param>
        /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref
        /// name="maxValue"/>.</returns>
        public int Next(int minValue, int maxValue);

        /// <summary>
        /// Returns a random single-precision floating-point number that is greater than or equal to 0.0, and less than
        /// 1.0.
        /// </summary>
        /// <remarks>This method is typically used when a random value between 0.0 (inclusive) and 1.0
        /// (exclusive) is required, such as for probabilistic calculations or simulations.</remarks>
        /// <returns>A single-precision floating-point number in the range [0.0, 1.0).</returns>
        public float NextSingle();

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating-point number greater than or equal to 0.0 and less than 1.0.</returns>
        public double NextDouble();
        #endregion
    }
}
