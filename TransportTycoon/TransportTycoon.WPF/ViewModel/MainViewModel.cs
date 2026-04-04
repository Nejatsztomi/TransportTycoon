using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Private fields
        [ObservableProperty]
        private object _currentView;
        #endregion

        #region Properties
        private GameModel? Model { get; set; }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            CurrentView = GetNewStartMenu();
        }
        #endregion

        #region Private Methods
        private StartMenuViewModel GetNewStartMenu()
        {
            StartMenuViewModel startMenuViewModel = new();

            startMenuViewModel.StartingNewGame += new(StartGame);
            startMenuViewModel.CreateNewGame += new(CreateNewGame);
            startMenuViewModel.LoadingGame += new(LoadGame);
            startMenuViewModel.ExitingGame += new(ExitGame);

            return startMenuViewModel;
        }

        private void StartGame(object? _1, EventArgs _2)
        {
            Model = new(new GameTable(), new WpfDispatcherTimer());
            Model.GameOver += new(Model_GameOver);
            Model.NewGame();

            GameViewModel gameViewModel = new(Model);

            CurrentView = gameViewModel;
        }

        private void CreateNewGame(object? _1, EventArgs _2)
        {
            CreateGameViewModel createGameViewModel = new();
            CurrentView = createGameViewModel;
        }

        private void LoadGame(object? _1, string _2)
        {
            throw new NotImplementedException("Load game functionality is not implemented yet!");
        }

        private void ExitGame(object? _1, EventArgs _2)
        {
            // Calls the MainWindows close method, which is basically the same as pressing the 
            Application.Current.MainWindow?.Close();
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
                //Model = null;
                CurrentView = GetNewStartMenu();
            }
        }

        private bool WantsToExit() => MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        #endregion

        #region Public method
        public bool CanClose()
        {
            Model?.SetMode(GameMode.Paused);

            if (WantsToExit())
            {
                return true;
            }

            if (Model is not null && !Model.IsGameOver)
            {
                Model.SetMode(GameMode.Run);
            }
            return false;
        }
        #endregion
    }
}
