namespace TransportTycoon.WPF
{
    /// <summary>
    /// An interface for dynamically bindig a view (UserControl) MinWidth and MinHeight
    /// </summary>
    public interface IViewConstraints
    {
        double MinimumWidth { get; }
        double MinimumHeight { get; }
    }
}
