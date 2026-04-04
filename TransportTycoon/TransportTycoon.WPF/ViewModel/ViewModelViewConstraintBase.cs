namespace TransportTycoon.WPF.ViewModel
{
    public abstract class ViewModelViewConstraintBase : ViewModelBase, IViewConstraints
    {
        /// <summary>
        /// Gets the minimum allowable width for the element.
        /// </summary>
        public abstract double? MinimumWidth { get; }
        /// <summary>
        /// Gets the minimum allowable height for the element.
        /// </summary>
        public abstract double? MinimumHeight { get; }
    }
}
