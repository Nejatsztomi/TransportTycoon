using System.ComponentModel;
using System.Configuration;
using System.Data;
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
        private MainViewModel? _mainViewModel;
        private MainWindow? _mainView;
        private StartWindow? _startView;
        private StartViewModel? _startViewModel;
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
            get => _mainView ?? throw new InvalidOperationException("MainView is not initialized.");
            set => _mainView = value;
        }
        private StartWindow StartView
        {
            get => _startView ?? throw new InvalidOperationException("StartView is not initialized.");
            set => _startView = value;
        }
        private StartViewModel StartViewModel
        {
            get => _startViewModel ?? throw new InvalidOperationException("StartViewModel is not initialized.");
            set => _startViewModel = value;
        }
        #endregion

        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(ShowStartMenu);
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        private void ShowStartMenu(object sender, StartupEventArgs e) 
        {
            startViewModel = new StartViewModel();

            startViewModel.StartNewGame += (sender, SelectedDifficulty) =>
            {
                GameModel model = new GameModel(SelectedDifficulty, new WpfDispatcherTimer());
            };

            startViewModel.LoadGame += (sender, e) =>
            {
                //TODO::
            };

            startViewModel.ExitGame += new EventHandler(ViewModel_Close);

            startView = new StartWindow
            {
                DataContext = startView
            };
            startView.Closing += new System.ComponentModel.CancelEventHandler(StartView_Close);
            startView.Show();
        }

        private void StartGame(GameModel model) 
        {
            //model
            this.model = model;
            model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            model.NewGame();

            //ViewModel
            mainViewModel = new MainViewModel(model);
            mainViewModel.Exit += new EventHandler(ViewModel_Close);

            //View
            view = new MainWindow
            {
                DataContext = mainViewModel,
            };
            view.Closing += new System.ComponentModel.CancelEventHandler(View_Close);
            view.Show();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //model
            Model = new GameModel(2000, new WpfDispatcherTimer());
            Model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            Model.NewGame();

            //ViewModel
            MainViewModel = new MainViewModel(Model);
            MainViewModel.Exit += ViewModel_Close;
            MainViewModel.GameModeChanged += MainViewModel_GameModeChanged;
            MainViewModel.TimeSpeedChanged += MainViewModel_TimeSpeedChanged;

            // StartView
            //StartView = new StartWindow
            //{
            //    DataContext = MainViewModel,
            //};
            //StartView.Closing += new CancelEventHandler(View_Close);
            //StartView.Show();

            //MainView
            MainView = new MainWindow
            {
                DataContext = MainViewModel,
            };
            MainView.Closing += new CancelEventHandler(View_Close);
            MainView.Show();
        }
        #endregion

        #region Private event Methods
        private void StartView_Close(object? sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

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
            }
        }

        private void ViewModel_Close(object? sender, EventArgs e)
        {
            //view.Close();
        }
        #endregion

        #region Game event methods
        private void MainViewModel_TimeSpeedChanged(object? sender, TimeSpeed e)
        {
            Model.SetTimeSpeed(e);
        }

        private void MainViewModel_GameModeChanged(object? sender, GameMode e)
        {
            Model.SetMode(e);
        }
        #endregion
    }
}
