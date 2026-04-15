using System.Windows;
using TransportTycoon.Model;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private GameModel? _model;

        private MainViewModel? _mainViewModel;
        private MainWindow? _mainView;
        #endregion

        #region Properties
        private GameModel Model
        {
            get => _model ?? throw new InvalidOperationException("Model is not initialized.");
            set => _model = value;
        }

        private MainViewModel MainViewModel
        {
            get => _mainViewModel ?? throw new InvalidOperationException("MainViewModel is not initialized.");
            set => _mainViewModel = value;
        }

        private MainWindow MainView
        {
            get => _mainView ?? throw new InvalidOperationException("MainWindow is not initialized.");
            set => _mainView = value;
        }
        #endregion

        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(ShowStartMenu);
        }
        #endregion

        #region Private event methods
        private void ShowStartMenu(object? _1, StartupEventArgs _2)
        {
            MainViewModel = new();

            MainView = new()
            {
                DataContext = MainViewModel,
            };

            MainView.Show();
        }
        #endregion

        #region Game event methods
        #endregion
    }
}
