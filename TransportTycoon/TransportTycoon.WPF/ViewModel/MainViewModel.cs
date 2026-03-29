using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;
using TransportTycoon.Model;
using TransportTycoon.WPF.View;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Private fields
        [ObservableProperty]
        private object _currentView;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            StartMenuViewModel startMenuViewModel = new();

            startMenuViewModel.StartNewGame += (sender, selectedDifficulty) =>
            {
                StartGame(selectedDifficulty);
            };

            startMenuViewModel.LoadGame += (sender, e) =>
            {
                throw new NotImplementedException("Load game functionality is not implemented yet!");
            };

            // Itt a kezdeti nézetet adjuk meg
            CurrentView = startMenuViewModel;
        }
        #endregion

        #region Private Methods
        private void StartGame(Difficulty difficulty)
        {
            // model
            GameModel model = new(difficulty, new WpfDispatcherTimer());
            model.GameOver += new EventHandler<TransportTycoonEventArgs>(Model_GameOver);
            model.NewGame();

            //ViewModel
            GameViewModel gameViewModel = new(model);
            gameViewModel.Exit += new EventHandler(GameView_Close);

            CurrentView = gameViewModel;
        }
        #endregion

        #region Private event methods
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
                //GameView.Closing -= View_Close;

                //GameView.Close();

                //StartMenuView.Show();
            }
        }

        private void GameView_Close(object? sender, EventArgs e)
        {

        }
        #endregion
    }
}
