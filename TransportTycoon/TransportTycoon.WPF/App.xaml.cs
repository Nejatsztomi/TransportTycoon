using System.ComponentModel;
using System.Configuration;
using System.Data;
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
        private GameModel model = null!;
        private MainViewModel mainViewModel = null!;
        private MainWindow view = null!;
        #endregion
        #region Properties
        #endregion
        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        private void App_Startup(object sender, StartupEventArgs e)
        {
            //model
            model = new GameModel();
            model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            model.NewGame();

            //nézetmodel
            mainViewModel = new MainViewModel(model);
            //viewModel.GameOver += new EventHandler<BombazoEventArgs>(ViewModel_GameOver);
            mainViewModel.Exit += new EventHandler(ViewModel_Exit);

            //nézet
            view = new MainWindow
            {
                DataContext = mainViewModel,
            };
            view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            view.Show();
        }


        #endregion
        #region Private event Methods

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            bool isGameOver = model.IsGameOver;
            model.SetMode(GameMode.Paused);

            if (MessageBox.Show("Are you sure, that you want to exit?", "Bombazo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
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
            }
            else 
            {

            }


        }

        private void ViewModel_Exit(object? sender, EventArgs e)
        {
            view.Close();
        }
        #endregion

    }

}
