using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Properties
        #region Relay commands
        public RelayCommand NewGameCommand { get; init; }
        public RelayCommand ExitCommand { get; init; }

        public RelayCommand NormalSpeedCommand { get; init; }
        public RelayCommand FastSpeedCommand { get; init; }
        public RelayCommand SuperFastSpeedCommand { get; init; }

        public RelayCommand PauseGameCommand { get; init; }
        public RelayCommand ResumeGameCommand { get; init; }
        public RelayCommand EditorModeCommand { get; init; }

        public RelayCommand<FieldViewModel> TileClickCommand { get; init; }
        #endregion

        public GameModel Model { get; init; }

        public ObservableCollection<FieldViewModel> Tiles { get; private set; }

        public int Balance => Model.Balance;
        public int GameTime => Model.GameTime;
        public bool IsPaused => Model.Mode == GameMode.Paused;
        public bool IsEditorMode => Model.Mode == GameMode.Editor;

        #region Map
        public int MapColumns => Model.Map.Width;
        public int MapRows => Model.Map.Height;
        [ObservableProperty]
        private double _zoomLevel = 1.0;
        [ObservableProperty]
        private string _selectedTile = "Click a tile!";
        #endregion
        #endregion

        #region Events
        public event EventHandler? NewGame;
        public event EventHandler? Exit;

        public event EventHandler<GameMode>? GameModeChanged;
        public event EventHandler<TimeSpeed>? TimeSpeedChanged;
        #endregion

        #region Constructors
        public MainViewModel(GameModel model)
        {
            Model = model;

            model.NewGameCreated += Model_NewGameCreated;
            model.GameTicked += Model_GameTicked;
            model.GameAdvanced += Model_GameAdvanced;

            NewGameCommand = new(OnNewGame);
            ExitCommand = new(OnExit);
            NormalSpeedCommand = new(OnNormalSpeed);
            FastSpeedCommand = new(OnFastSpeed);
            SuperFastSpeedCommand = new(OnSuperFastSpeed);

            PauseGameCommand = new(OnPauseGame);
            ResumeGameCommand = new(OnResumeGame);
            EditorModeCommand = new(OnEditorMode);

            TileClickCommand = new(OnTileClick!);

            Tiles = [];
            RefreshTable();
        }

        private void Model_GameAdvanced(object? sender, EventArgs e)
        {
            RefreshTable();
        }
        #endregion

        #region Private methods
        private void RefreshTable()
        {
            //Tiles.Clear();
            List<FieldViewModel> tempList = new(Model.Map.Width * Model.Map.Height + 1);
            for (int x = 0; x < Model.Map.Width; x++)
            {
                for (int y = 0; y < Model.Map.Height; y++)
                {
                    string path = Model.Map[x, y] switch
                    {
                        Plain _ => "Assets/Images/Terrain/field.png",
                        Hill _ => "Assets/Images/Terrain/hill.png",
                        Water _ => "Assets/Images/Terrain/water2.png",
                        _ => "Assets/Images/Terrain/field.png"
                    };
                    tempList.Add(new(Model.Map[x, y], path));
                }
            }
            Tiles = new(tempList);
        }
        #endregion

        #region Relay command methods
        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExit()
        {
            Exit?.Invoke(this, EventArgs.Empty);
        }

        private void OnNormalSpeed()
        {
            TimeSpeedChanged?.Invoke(this, TimeSpeed.Normal);
        }

        private void OnFastSpeed()
        {
            TimeSpeedChanged?.Invoke(this, TimeSpeed.Fast);
        }

        private void OnSuperFastSpeed()
        {
            TimeSpeedChanged?.Invoke(this, TimeSpeed.SuperFast);
        }

        private void OnPauseGame()
        {
            GameModeChanged?.Invoke(this, GameMode.Paused);
        }

        private void OnResumeGame()
        {
            GameModeChanged?.Invoke(this, GameMode.Run);
        }

        private void OnEditorMode()
        {
            GameModeChanged?.Invoke(this, GameMode.Editor);
        }
        private void OnTileClick(object? param)
        {
            if (param is FieldViewModel tile)
            {
                SelectedTile = $"Clicked tile at ({tile.X}, {tile.Y})";
            }
        }
        #endregion

        #region Event methods
        private void Model_NewGameCreated(object? sender, EventArgs e)
        {
            RefreshTable();
        }

        private void Model_GameTicked(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GameTime));
        }
        #endregion
    }
}
