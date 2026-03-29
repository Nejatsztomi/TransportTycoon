using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class GameViewModel : ViewModelBase, IDisposable, IViewConstraints
    {
        #region Properties
        #region IViewConstraints
        public double MinimumWidth => 800;
        public double MinimumHeight => 450;
        #endregion

        #region Relay commands
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

        #region Constructors
        public GameViewModel(GameModel model)
        {
            Model = model;

            model.NewGameCreated += Model_NewGameCreated;
            model.GameTicked += Model_GameTicked;
            model.GameAdvanced += Model_GameAdvanced;

            NormalSpeedCommand = new(OnNormalSpeed);
            FastSpeedCommand = new(OnFastSpeed);
            SuperFastSpeedCommand = new(OnSuperFastSpeed);

            PauseGameCommand = new(OnPauseGame);
            ResumeGameCommand = new(OnResumeGame);
            EditorModeCommand = new(OnEditorMode);

            TileClickCommand = new(OnTileClick);

            Tiles = [];
            RefreshTable();
        }

        private void Model_GameAdvanced(object? sender, List<Tuple<int, int>> grownTrees)
        {
            // O(n * m + m)
            Tiles.Where(tile => grownTrees.Any(tuple => tuple.Item1 == tile.X && tuple.Item2 == tile.Y))
                .ToList()
                .ForEach(tile => tile.RefreshTreeCount());
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

        private void OnNormalSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.Normal);
            OnResumeGame();
        }

        private void OnFastSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.Fast);
            OnResumeGame();
        }

        private void OnSuperFastSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.SuperFast);
            OnResumeGame();
        }

        private void OnPauseGame()
        {
            Model.SetMode(GameMode.Paused);
        }

        private void OnResumeGame()
        {
            Model.SetMode(GameMode.Run);
        }

        private void OnEditorMode()
        {
            Model.SetMode(GameMode.Editor);
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

        #region Dispose
        // Unsubscribe from events to prevent memory leaks
        public void Dispose()
        {
            Model.NewGameCreated -= Model_NewGameCreated;
            Model.GameTicked -= Model_GameTicked;
            Model.GameAdvanced -= Model_GameAdvanced;
        }
        #endregion
    }
}
