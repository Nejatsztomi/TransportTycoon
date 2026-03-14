using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private Point? DragStartPoint { get; set; } = null;
        private Point? DragStartOffset { get; set; } = null;
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Private event methods
        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Zoom with touchpad: Pinch-to-zoom
            // Zoom with mouse: Ctrl+Scroll wheel
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;

                if (this.DataContext is MainViewModel vm)
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

        private void MapScrollViewer_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragStartPoint = e.GetPosition(MapScrollViewer);
            DragStartOffset = new(MapScrollViewer.HorizontalOffset, MapScrollViewer.VerticalOffset);
            MapScrollViewer.CaptureMouse();
        }

        private void MapScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
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

        private void MapScrollViewer_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
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
