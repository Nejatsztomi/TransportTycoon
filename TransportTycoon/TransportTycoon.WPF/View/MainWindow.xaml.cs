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
        #endregion
    }
}
