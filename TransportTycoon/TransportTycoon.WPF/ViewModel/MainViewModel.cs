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
        private GameModel? _model;
        private StartMenuViewModel _startMenuViewModel;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public MainViewModel()
        {
            _startMenuViewModel = new();

            _startMenuViewModel.StartingNewGame += new(StartGame);
            _startMenuViewModel.CreateNewGame += new(CreateNewGame);
            _startMenuViewModel.LoadingGame += new(LoadGame);
            _startMenuViewModel.ExitingGame += new(ExitGame);

            CurrentView = _startMenuViewModel;
        }
        #endregion

        #region Private Methods
        private void StartGame(object? _1, EventArgs _2)
        {
            _model = new(new GameTable(), new WpfDispatcherTimer());
            _model.GameOver += new(Model_GameOver);
            _model.NewGame();

            GameViewModel gameViewModel = new(_model);

            CurrentView = gameViewModel;
        }

        private void CreateNewGame(object? _1, EventArgs _2)
        {
            CreateGameViewModel createGameViewModel = new();

            createGameViewModel.BackToMainMenu += new(BackToMainMenu);

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

        private void BackToMainMenu(object? _1, EventArgs _2)
        {
            CurrentView = _startMenuViewModel;
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
                CurrentView = _startMenuViewModel;
            }
        }

        private bool WantsToExit() => MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        #endregion

        #region Public method
        public bool CanClose()
        {
            _model?.Mode = GameMode.Paused;

            if (WantsToExit())
            {
                return true;
            }

            if (_model is not null && !_model.IsGameOver)
            {
                _model.Mode = GameMode.Run;
            }
            return false;
        }
        #endregion
    }
}
