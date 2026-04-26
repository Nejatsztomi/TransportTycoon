using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using TransportTycoon.Persistence;

namespace TransportTycoon.WPF.ViewModel
{
    public sealed partial class MainViewModel : ViewModelViewConstraintBase
    {
        #region Private fields
        [ObservableProperty]
        private ViewModelBase _currentView;
        private GameModel? _model;

        private readonly StartMenuViewModel _startMenuViewModel;

        #endregion

        #region Properties
        #region IViewConstraints
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #endregion
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

        #region Private event methods
        #region Start menu
        private void StartMenuViewModel_StartingNewGame(object? _1, EventArgs _2)
        {
            _model = CreateNewGameModel(new());

            CurrentView = CreateNewGameViewModel(_model);
        }

        private void StartMenuViewModel_ShowGameCreationView(object? _1, EventArgs _2)
        {
            CreateGameViewModel createGameViewModel = new();

            createGameViewModel.BackToMainMenu += BackToMainMenu;
            createGameViewModel.CreateGame += CreateGameViewModel_CreateGame;

            CurrentView = createGameViewModel;
        }

        private async void StartMenuViewModel_LoadingGame(object? _1, string uri)
        {
            try
            {
                _model = CreateNewGameModel(new());

                await _model.LoadGame(uri);

                CurrentView = CreateNewGameViewModel(_model);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load the game. The file might be corrupted or incompatible." + Environment.NewLine +
                                "Error details: " + ex.Message, "TransportTycoon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartMenuViewModel_ExitingGame(object? _1, EventArgs _2)
        {
            Application.Current.MainWindow?.Close();
        }
        #endregion

        #region Create game
        private void CreateGameViewModel_CreateGame(object? _1, MapGenerationContext context)
        {
            _model = CreateNewGameModel(context);
            CurrentView = CreateNewGameViewModel(_model);
        }

        private void BackToMainMenu()
        {
            CurrentView = _startMenuViewModel;
        }
        #endregion

        #region Game
        private void Model_GameOver(object? _1, TransportTycoonEventArgs e)
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
        #endregion
        #endregion

        #region Private methods
        private bool WantsToExit() => MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        /// <summary>
        /// Creates a new game model based on the provided map generation context.
        /// This method initializes the game model, sets up event handlers, and starts a new game.
        /// </summary>
        /// <param name="context">The context for map generation.</param>
        /// <returns>A new instance of the GameModel class.</returns>
        private GameModel CreateNewGameModel(MapGenerationContext context)
        {
            var model = new GameModel(new(MapGeneratorFactory.CreateMapGenerator(context), context), new WpfDispatcherTimer(), JsonSaveManagerFactory.Get());
            model.GameOver += Model_GameOver;
            model.NewGame();
            return model;
        }

        /// <summary>
        /// Creates a new instance of the GameViewModel class using the specified game model.
        /// </summary>
        /// <remarks>
        /// The BackToMainMenu event of the created GameViewModel is subscribed to the current
        /// handler to enable navigation back to the main menu.
        /// </remarks>
        /// <param name="model">The GameModel instance that provides the data for the new GameViewModel.</param>
        /// <returns>A new GameViewModel initialized with the specified game model.</returns>
        private GameViewModel CreateNewGameViewModel(GameModel model)
        {
            var gameViewModel = new GameViewModel(model);
            gameViewModel.BackToMainMenu += BackToMainMenu;
            return gameViewModel;
        }
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
