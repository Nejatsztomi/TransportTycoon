using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelViewConstraintBase
    {
        #region Private fields
        [ObservableProperty]
        private ViewModelBase _currentView;
        private GameModel? _model;

        private readonly StartMenuViewModel _startMenuViewModel;

        #endregion
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #region Properties

        #endregion

        #region Constructors
        public MainViewModel()
        {
            _startMenuViewModel = new();

            _startMenuViewModel.StartingNewGame += StartMenuViewModel_StartingNewGame;
            _startMenuViewModel.ShowGameCreationView += StartMenuViewModel_ShowGameCreationView;
            _startMenuViewModel.LoadingGame += StartMenuViewModel_LoadingGame;
            _startMenuViewModel.ExitingGame += StartMenuViewModel_ExitingGame;

            CurrentView = _startMenuViewModel;
        }
        #endregion

        #region Private Methods
        private void StartMenuViewModel_StartingNewGame(object? _1, EventArgs _2)
        {
            _model = new(new(new()), new WpfDispatcherTimer());
            _model.GameOver += Model_GameOver;
            _model.NewGame();

            GameViewModel gameViewModel = new(_model);

            CurrentView = gameViewModel;
        }

        private void StartMenuViewModel_ShowGameCreationView(object? _1, EventArgs _2)
        {
            CreateGameViewModel createGameViewModel = new();

            createGameViewModel.BackToMainMenu += CreateGameViewModel_BackToMainMenu;
            createGameViewModel.CreateGame += CreateGameViewModel_CreateGame;

            CurrentView = createGameViewModel;
        }

        private void CreateGameViewModel_CreateGame(object? _, MapGenerationContext context)
        {
            _model = new(new(context), new WpfDispatcherTimer());
            _model.GameOver += Model_GameOver;
            _model.NewGame();

            GameViewModel gameViewModel = new(_model);

            CurrentView = gameViewModel;
        }

        private void StartMenuViewModel_LoadingGame(object? _1, string _2)
        {
            throw new NotImplementedException("Load game functionality is not implemented yet!");
        }

        private void StartMenuViewModel_ExitingGame(object? _1, EventArgs _2)
        {
            // Calls the MainWindows close method, which is basically the same as pressing the 
            Application.Current.MainWindow?.Close();
        }

        private void CreateGameViewModel_BackToMainMenu(object? _1, EventArgs _2)
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
