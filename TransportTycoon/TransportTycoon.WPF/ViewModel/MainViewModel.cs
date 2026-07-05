using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows;
using TransportTycoon.MapData;
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
        private readonly IPersistence _persistence;
        #endregion

        #region Properties
        #region IViewConstraints
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a save game operation is requested, providing the file path or identifier for the save data.
        /// </summary>
        public event Action<string>? SaveGame;

        /// <summary>
        /// Occurs when a request to load a game is made.
        /// </summary>
        public event Action? LoadGame;
        #endregion

        #region Constructors
        public MainViewModel(IPersistence persistence)
        {
            _persistence = persistence;
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
            var data = new GameCreationData(new(), DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss"));
            _model = CreateNewGameModel(data);

            CurrentView = CreateNewGameViewModel(_model);
        }

        private void StartMenuViewModel_ShowGameCreationView(object? _1, EventArgs _2)
        {
            CreateGameViewModel createGameViewModel = new();

            createGameViewModel.BackToMainMenu += BackToMainMenu;
            createGameViewModel.CreateGame += CreateGameViewModel_CreateGame;

            CurrentView = createGameViewModel;
        }

        private void StartMenuViewModel_LoadingGame()
        {
            LoadGame?.Invoke();
        }

        private void StartMenuViewModel_ExitingGame(object? _1, EventArgs _2)
        {
            Application.Current.MainWindow?.Close();
        }
        #endregion

        #region Create game
        private void CreateGameViewModel_CreateGame(GameCreationData data)
        {
            _model = CreateNewGameModel(data);
            CurrentView = CreateNewGameViewModel(_model);
        }

        private void BackToMainMenu()
        {
            CurrentView = _startMenuViewModel;
            _model = null;
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
                BackToMainMenu();
            }
        }

        private void GameViewModel_SaveGame()
        {
            if (_model is null) return;

            SaveGame?.Invoke(_model.SaveName);
        }
        #endregion
        #endregion

        #region Private methods
        private bool WantsToExit() => MessageBox.Show("Are you sure, that you want to exit?", "TransportTycoon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        /// <summary>
        /// Creates a new game model based on the provided map generation context.
        /// This method initializes the game model, sets up event handlers, and starts a new game.
        /// </summary>
        /// <param name="data">The data for game creation.</param>
        /// <returns>A new instance of the GameModel class.</returns>
        private GameModel CreateNewGameModel(GameCreationData data)
        {
            var gameTable = new GameTable(MapGeneratorFactory.CreateMapGenerator(data.MapGenerationContext), data.MapGenerationContext);
            var model = new GameModel(gameTable, new WpfRenderingTimer(), data);
            model.GameOver += Model_GameOver;
            model.NewGame();
            return model;
        }


        private GameModel CreateNewGameModelFromSave(GameSaveData data, string saveName)
        {
            var context = new MapGenerationContext(data.MapContextData);
            var gameTable = new GameTable(MapGeneratorFactory.CreateMapGenerator(context), context);
            var model = new GameModel(gameTable, new WpfRenderingTimer(), data, saveName);
            model.GameOver += Model_GameOver;
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
            gameViewModel.SaveGame += GameViewModel_SaveGame;
            return gameViewModel;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the game can be safely closed based on the current state and user intent.
        /// </summary>
        /// <remarks>This method temporarily pauses the game to check if the user intends to exit. If the
        /// game is not over and the user does not wish to exit, the game resumes running. Use this method to prompt for
        /// confirmation or handle cleanup before closing the game.</remarks>
        /// <returns><see langword="true"/> if the game can be closed; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Asynchronously saves the current game state to the specified location.
        /// </summary>
        /// <remarks>
        /// If the save operation fails, a message box is displayed to inform the user of the error.
        /// </remarks>
        /// <param name="uri">The destination URI where the game state will be saved. This should be a valid file path or resource
        /// identifier.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
        public async Task SaveGameAt(string uri)
        {
            try
            {
                var data = _model?.GetGameSaveData() ?? throw new NullReferenceException("The GameModel doesn't exists");
                await _persistence.SaveGame(uri, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save the game. Please try again." + Environment.NewLine +
                                "Error details: " + ex.Message, "TransportTycoon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Asynchronously loads a game state from the specified file URI and updates the current game view.
        /// </summary>
        /// <remarks>
        /// If the file is corrupted or incompatible, an error message is displayed and the
        /// current game state is not changed.
        /// </remarks>
        /// <param name="uri">The path or URI of the file containing the saved game data. Must refer to a valid and accessible file.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous load operation.</returns>
        public async Task LoadGameFrom(string uri)
        {
            try
            {
                var data = await _persistence.LoadGame(uri) ?? throw new NullReferenceException("Loaded data is null.");
                string saveName = Path.GetFileNameWithoutExtension(uri);

                _model = CreateNewGameModelFromSave(data, saveName);
                CurrentView = CreateNewGameViewModel(_model);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load the game. The file might be corrupted or incompatible." + Environment.NewLine +
                                "Error details: " + ex.Message, "TransportTycoon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
