namespace TransportTycoon.Model
{
    /// <summary>
    /// Provides data for events that reference a specific field location in the Transport Tycoon grid.
    /// </summary>
    /// <remarks>Use this class to access the X and Y coordinates associated with a field-related event.
    /// Instances of this class are typically passed to event handlers to indicate the affected grid position.</remarks>
    public sealed class TransportTycoonFieldEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the value of X.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y-coordinate value.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the TransportTycoonFieldEventArgs class with the specified field coordinates.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the field. Represents the X position.</param>
        /// <param name="y">The vertical coordinate of the field. Represents the Y position.</param>
        public TransportTycoonFieldEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
