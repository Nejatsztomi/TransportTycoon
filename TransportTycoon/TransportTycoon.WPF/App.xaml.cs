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
        private GameModel model = null!;
        private MainViewModel mainViewModel = null!;
        private MainWindow view = null!;

        private StartViewModel startViewModel = null!;
        private StartWindow startView = null!;
        #endregion
        #region Properties
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
                StartGame(model);
                startView.Close();
            };

            startViewModel.LoadGame += (sender, e) =>
            {
                //TODO::
            };

            startViewModel.ExitGame += new EventHandler(StartView_Close);

            startView = new StartWindow
            {
                DataContext = startViewModel,
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
            mainViewModel.Exit += new EventHandler(GameView_Close);

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
            model = new GameModel(2000,new WpfDispatcherTimer());
            model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            model.NewGame();

            //ViewModel
            mainViewModel = new MainViewModel(model);
            mainViewModel.Exit += new EventHandler(GameView_Close);

            //View
            view = new MainWindow
            {
                DataContext = mainViewModel,
            };
            view.Closing += new System.ComponentModel.CancelEventHandler(View_Close);
            view.Show();
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
            bool isGameOver = model.IsGameOver;
            model.SetMode(GameMode.Paused);

            if (MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; 

                if (!isGameOver)
                    model.SetMode(GameMode.Run);
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
                view.Close();
                startView.Show();
            }
            else 
            {

            }


        }

        private void StartView_Close(object? sender, EventArgs e)
        {
            startView.Close();
        }
        private void GameView_Close(object? sender, EventArgs e)
        {
            view.Close();
        }
        #endregion

    }

}
