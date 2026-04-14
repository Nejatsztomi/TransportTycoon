using System.ComponentModel;
using System.Windows;
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
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                if (!mainViewModel.CanClose())
                {
                    e.Cancel = true;
                    return;
                }
                Application.Current.Shutdown();
            }
        }
        #endregion
    }
}
