using System.Windows.Media;

namespace TransportTycoon.WPF.Utils
{
    public static class ColorConverterUtil
    {
        #region Public static methods
        /// <summary>
        /// Converts a <see cref="System.Windows.Media.Color"/> to an <see cref="System.UInt32"/> in ARGB format.
        /// </summary>
        /// <param name="color">The <see cref="System.Windows.Media.Color"/> object to convert.</param>
        /// <returns>The color's ARGB representation in <see cref="System.UInt32"/>.</returns>
        public static UInt32 ColorToUInt32(Color color)
        {
            return (UInt32)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }

        /// <summary>
        /// Converts the <see cref="System.Byte"/> values of an ARGB color to an <see cref="System.UInt32"/> in ARGB format.
        /// </summary>
        /// <param name="a">The color's Alpha value.</param>
        /// <param name="r">The color's Red value.</param>
        /// <param name="g">The color's Green value.</param>
        /// <param name="b">The color's Blue value.</param>
        /// <returns>The color's ARGB representation in <see cref="System.UInt32"/>.</returns>
        public static UInt32 RgbaBytesToUInt32(Byte a, Byte r, Byte g, Byte b)
        {
            return (UInt32)((a << 24) | (r << 16) | (g << 8) | b);
        }
        #endregion
    }
}
