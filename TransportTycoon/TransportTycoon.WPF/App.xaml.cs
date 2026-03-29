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
        private GameViewModel? _mainViewModel;
        private MainWindow? _mainView;
        private StartWindow? _startView;
        private StartMenuViewModel? _startViewModel;
        #endregion

        #region Properties
        private GameModel Model
        {
            get => _model ?? throw new InvalidOperationException("Model is not initialized.");
            set => _model = value;
        }
        private GameViewModel MainViewModel
        {
            get => _mainViewModel ?? throw new InvalidOperationException("MainViewModel is not initialized.");
            set => _mainViewModel = value;
        }
        private MainWindow MainView
        {
            get => _mainView ?? throw new InvalidOperationException("MainView is not initialized.");
            set => _mainView = value;
        }
        private StartWindow StartView
        {
            get => _startView ?? throw new InvalidOperationException("StartView is not initialized.");
            set => _startView = value;
        }
        private StartMenuViewModel StartViewModel
        {
            get => _startViewModel ?? throw new InvalidOperationException("StartViewModel is not initialized.");
            set => _startViewModel = value;
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

        #region Public Methods
        #endregion

        #region Private Methods
        private void StartGame(Difficulty difficulty)
        {
            //model
            Model = new(difficulty, new WpfDispatcherTimer());
            Model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            Model.NewGame();

            //ViewModel
            MainViewModel = new(Model);
            MainViewModel.Exit += new EventHandler(GameView_Close);

            //View
            MainView = new()
            {
                DataContext = MainViewModel,
            };
            MainView.Closing += new CancelEventHandler(View_Close);
            MainView.Show();

            // Close the start view
            // Must be called after .Show(), otherwise the app exists, because ShutdownMode = OnLastWindowClose by default
            StartView.Hide();
        }
        #endregion

        #region Private event Methods
        private void ShowStartMenu(object? sender, StartupEventArgs e)
        {
            StartViewModel = new();

            StartViewModel.StartNewGame += (sender, selectedDifficulty) =>
            {
                StartGame(selectedDifficulty);
            };

            StartViewModel.LoadGame += (sender, e) =>
            {
                throw new NotImplementedException("Load game functionality is not implemented yet!");
            };

            StartViewModel.ExitGame += new EventHandler(StartView_Close);

            StartView = new StartWindow
            {
                DataContext = StartViewModel,
            };
            StartView.Closing += new CancelEventHandler(StartView_Close);
            CurrentView = StartView;
            StartView.Show();
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
                StartView.Closing -= StartView_Close;
                Application.Current.Shutdown();
            }
        }

        private void Model_GameOver(object? sender, TransportTycoonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Unfortunately, you lost!" + Environment.NewLine +
                                                        "Fate has a cruel sense of humor." + Environment.NewLine +
                                                        "Survived Time: " + e.GameTime + Environment.NewLine +
                                                        "Owned Vehicles: " + e.NumberOfVehicles + Environment.NewLine +
                                                        "Would you like to return to the Main menu?",
                                                        "TransportTycoon",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                //TODO:We need a method that will open the main menu
                MainView.Closing -= View_Close;

                MainView.Close();

                StartView.Show();
            }
        }

        private void StartView_Close(object? sender, EventArgs e)
        {
            StartView.Close();
        }

        private void GameView_Close(object? sender, EventArgs e)
        {
            CurrentView?.Close();
            CurrentView = null;
        }
        #endregion

        #region Game event methods
        #endregion
    }
}
