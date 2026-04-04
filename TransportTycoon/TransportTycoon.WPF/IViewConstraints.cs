namespace TransportTycoon.WPF
{
    /// <summary>
    /// An interface for dynamically bindig a view (UserControl) MinWidth and MinHeight
    /// </summary>
    public interface IViewConstraints
    {
        /// <summary>
        /// Gets the minimum allowable width for the element.
        /// </summary>
        double? MinimumWidth { get; }
        /// <summary>
        /// Gets the maximum allowable width for the element.
        /// </summary>
        double? MinimumHeight { get; }
    }
}
