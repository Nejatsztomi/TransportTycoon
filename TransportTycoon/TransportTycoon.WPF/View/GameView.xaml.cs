using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        #region Constructors
        public GameView()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                Focus();
            };
        }
        #endregion

        #region Private event methods
        private void UserControl_PreviewKeyDown(object? _1, KeyEventArgs e)
        {
            Debug.WriteLine($"Key pressed: {e.Key}");
            if (e.Key != Key.Escape) return;

            if (DataContext is not GameViewModel viewModel) return;

            if (viewModel.IsPaused)
            {
                viewModel.ResumeGame();
            }
            else
            {
                viewModel.PauseGame();
            }

            Focus();
            e.Handled = true;
        }

        private void UserControl_MouseDown(object? _1, MouseButtonEventArgs _2)
        {
            Focus();
            Keyboard.Focus(this);
        }
        #endregion
    }
}
