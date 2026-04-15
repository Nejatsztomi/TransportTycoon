using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        #region Properties
        private Point? DragStartPoint { get; set; } = null;
        private Point? DragStartOffset { get; set; } = null;
        #endregion

        #region Constructors
        public Map()
        {
            InitializeComponent();
        }
        #endregion

        #region Private event methods
        private void MapScrollViewer_PreviewMouseWheel(object? _1, MouseWheelEventArgs e)
        {
            e.Handled = true;
            // Zoom with touchpad: Pinch-to-zoom
            // Zoom with mouse: Ctrl+Scroll wheel
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;

                if (DataContext is GameViewModel vm)
                {
                    if (e.Delta > 0 && vm.ZoomLevel < 3.0)
                    {
                        vm.ZoomLevel += 0.1;
                    }
                    else if (e.Delta < 0 && vm.ZoomLevel > 0.4)
                    {
                        vm.ZoomLevel -= 0.1;
                    }
                }
            }
        }

        private void MapScrollViewer_PreviewMouseRightButtonDown(object? _1, MouseButtonEventArgs e)
        {
            DragStartPoint = e.GetPosition(MapScrollViewer);
            DragStartOffset = new(MapScrollViewer.HorizontalOffset, MapScrollViewer.VerticalOffset);
            MapScrollViewer.CaptureMouse();
        }

        private void MapScrollViewer_PreviewMouseMove(object? _1, MouseEventArgs e)
        {
            if (DragStartPoint.HasValue && DragStartOffset.HasValue)
            {
                Point currentPoint = e.GetPosition(MapScrollViewer);

                double deltaX = currentPoint.X - DragStartPoint.Value.X;
                double deltaY = currentPoint.Y - DragStartPoint.Value.Y;

                MapScrollViewer.ScrollToHorizontalOffset(DragStartOffset.Value.X - deltaX);
                MapScrollViewer.ScrollToVerticalOffset(DragStartOffset.Value.Y - deltaY);
            }
        }

        private void MapScrollViewer_PreviewMouseRightButtonUp(object? _1, MouseButtonEventArgs _2)
        {
            if (DragStartPoint.HasValue)
            {
                MapScrollViewer.ReleaseMouseCapture();
                DragStartPoint = null;
            }
        }
        #endregion
    }
}
