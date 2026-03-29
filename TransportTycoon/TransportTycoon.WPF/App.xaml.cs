using System.ComponentModel;
using System.Windows;
using TransportTycoon.Model;
using TransportTycoon.WPF.View;
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
        private GameViewModel? _gameViewModel;
        private GameView? _gameView;
        private StartMenuView? _startMenuView;
        private StartMenuViewModel? _startMenuViewModel;

        private MainViewModel? _mainViewModel;
        private MainWindow? _mainView;
        #endregion

        #region Properties
        private GameModel Model
        {
            get => _model ?? throw new InvalidOperationException("Model is not initialized.");
            set => _model = value;
        }
        private GameViewModel GameViewModel
        {
            get => _gameViewModel ?? throw new InvalidOperationException("GameViewModel is not initialized.");
            set => _gameViewModel = value;
        }
        private GameView GameView
        {
            get => _gameView ?? throw new InvalidOperationException("GameView is not initialized.");
            set => _gameView = value;
        }
        private StartMenuView StartMenuView
        {
            get => _startMenuView ?? throw new InvalidOperationException("StartMenuView is not initialized.");
            set => _startMenuView = value;
        }
        private StartMenuViewModel StartMenuViewModel
        {
            get => _startMenuViewModel ?? throw new InvalidOperationException("StartMenuViewModel is not initialized.");
            set => _startMenuViewModel = value;
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

        private Window? CurrentView { get; set; } // Vagy event argumentként átadni a view-t a ViewModel-nek
        #endregion

        #region Constructor
        public App()
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Startup += new StartupEventHandler(ShowStartMenu);
        }
        #endregion

        #region Private event methods
        private void ShowStartMenu(object? sender, StartupEventArgs e)
        {
            MainViewModel = new();

            MainView = new()
            {
                DataContext = MainViewModel,
            };

            //MainView.Closing += new CancelEventHandler(StartView_Close);
            MainView.Show();
        }

        private void StartView_Close(object? sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void View_Close(object? sender, CancelEventArgs e)
        {
            bool isGameOver = Model.IsGameOver;
            Model.SetMode(GameMode.Paused);

            if (MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

                if (!isGameOver)
                    Model.SetMode(GameMode.Run);
            }
            else
            {
                //StartMenuView.Closing -= StartView_Close;
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region Game event methods
        #endregion
    }
}
