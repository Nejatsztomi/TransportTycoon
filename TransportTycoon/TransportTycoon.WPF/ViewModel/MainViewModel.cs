using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Properties
        #region Relay commands
        public RelayCommand NormalSpeedCommand { get; init; }
        public RelayCommand FastSpeedCommand { get; init; }
        public RelayCommand SuperFastSpeedCommand { get; init; }

        public RelayCommand PauseGameCommand { get; init; }
        public RelayCommand ResumeGameCommand { get; init; }
        public RelayCommand EditorModeCommand { get; init; }
        public RelayCommand<object> SetSelectedButtonCommand { get; init; }

        public RelayCommand IncreaseHeightCommand { get; init; }
        public RelayCommand DecreaseHeightCommand { get; init; }

        public RelayCommand<FieldViewModel> TileClickCommand { get; init; }
        public RelayCommand<FieldViewModel> BuildInfrastructureCommand { get; init; }
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
        [ObservableProperty]
        private int _selectedButton = 0;
        #endregion
        #endregion

        #region Events
        public event EventHandler? NewGame;
        public event EventHandler? Exit;
        #endregion

        #region Constructors
        public MainViewModel(GameModel model)
        {
            Model = model;

            model.NewGameCreated += Model_NewGameCreated;
            model.GameTicked += Model_GameTicked;
            model.GameAdvanced += Model_GameAdvanced;
            model.InfrastructureBuilt += Model_InfrastructureBuilt;
            model.FieldChanged += Model_FieldChanged;
            model.BalanceChanged += Model_BalanceChanged;

            NormalSpeedCommand = new(OnNormalSpeed);
            FastSpeedCommand = new(OnFastSpeed);
            SuperFastSpeedCommand = new(OnSuperFastSpeed);

            PauseGameCommand = new(OnPauseGame);
            ResumeGameCommand = new(OnResumeGame);
            EditorModeCommand = new(OnEditorMode);

            IncreaseHeightCommand = new(OnIncreaseHeight);
            DecreaseHeightCommand = new(OnDecreaseHeight);

            TileClickCommand = new(OnTileClick);
            SetSelectedButtonCommand = new RelayCommand<object>(x =>
            {
                if (x == null) return;
                _selectedButton = Convert.ToInt32(x);
            });
            BuildInfrastructureCommand = new RelayCommand<FieldViewModel>(tile =>
            {
                switch (_selectedButton)
                {
                    case 1:
                        Model.BuildRoad(tile.X, tile.Y);
                        break;
                    case 2:
                        Model.BuildBridge(tile.X, tile.Y);
                        break;
                    default:
                        break;
                }
            }, (_) => IsEditorMode);

            Tiles = [];
            RefreshTable();
        }

        private void Model_BalanceChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Balance));
        }

        private void Model_FieldChanged(object? sender, TransportTycoonFieldEventArgs e)
        {
            var tile = Tiles.FirstOrDefault(t => t.X == e.X && t.Y == e.Y);

            if (tile != null)
            {
                tile.RefreshTerrain(Model.Map[e.X, e.Y]);
            }
        }

        private void Model_InfrastructureBuilt(object? sender, List<(int, int)> changedFields)
        {
            foreach (var (x, y) in changedFields)
            {
                FieldViewModel? tile = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
                if (tile != null)
                {
                    string oldPath = tile.ImagePath;
                    int index = Tiles.IndexOf(tile);
                    Tiles[index] = new(Model.Map[x, y]);
                    tile.RefreshInfrastructure();
                }
            }
            //RefreshTable();
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
                    tempList.Add(new(Model.Map[x, y]));
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
                Model.SetSelectedField(tile.X, tile.Y);
            }
        }

        private void OnIncreaseHeight()
        {
            if (Model.SelectedField != null)
            {
                Model.IncreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
            }
        }

        private void OnDecreaseHeight()
        {
            if (Model.SelectedField != null)
            {
                Model.DecreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
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
