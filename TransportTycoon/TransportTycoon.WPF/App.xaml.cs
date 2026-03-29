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
            GameViewModel = new(Model);
            GameViewModel.Exit += new EventHandler(GameView_Close);

            //View
            GameView = new()
            {
                DataContext = GameViewModel,
            };
            GameView.Closing += new CancelEventHandler(View_Close);
            GameView.Show();

            // Close the start view
            // Must be called after .Show(), otherwise the app exists, because ShutdownMode = OnLastWindowClose by default
            StartMenuView.Hide();
        }
        #endregion

        #region Private event Methods
        private void ShowStartMenu(object? sender, StartupEventArgs e)
        {
            StartMenuViewModel = new();

            StartMenuViewModel.StartNewGame += (sender, selectedDifficulty) =>
            {
                StartGame(selectedDifficulty);
            };

            StartMenuViewModel.LoadGame += (sender, e) =>
            {
                throw new NotImplementedException("Load game functionality is not implemented yet!");
            };

            StartMenuViewModel.ExitGame += new EventHandler(StartView_Close);

            StartMenuView = new StartMenuView
            {
                DataContext = StartMenuViewModel,
            };
            StartMenuView.Closing += new CancelEventHandler(StartView_Close);
            CurrentView = StartMenuView;
            StartMenuView.Show();
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
                StartMenuView.Closing -= StartView_Close;
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
                GameView.Closing -= View_Close;

                GameView.Close();

                StartMenuView.Show();
            }
        }

        private void StartView_Close(object? sender, EventArgs e)
        {
            StartMenuView.Close();
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
