using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;
using TransportTycoon.Model;
using TransportTycoon.WPF.Utils;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class GameViewModel : ViewModelViewConstraintBase, IDisposable
    {
        #region Properties
        #region IViewConstraints
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #endregion

        public GameModel Model { get; init; }

        [Obsolete]
        public ObservableCollection<FieldViewModel> Tiles { get; private set; }
        public Field[,] Tiles2 { get; }
        public WriteableBitmap MinimapImage { get; set; }

        public int Balance => Model.Balance;
        public int GameTime => Model.GameTime;
        public bool IsPaused => Model.Mode == GameMode.Paused;
        public bool IsEditorMode => Model.Mode == GameMode.Editor;

        #region Map
        public GameTable Map => Model.Map;
        public int Width => Model.Map.Width;
        public int Height => Model.Map.Height;
        [ObservableProperty]
        [Obsolete]
        private double _zoomLevel = 1.0;
        #endregion
        [ObservableProperty]
        private int _selectedButton = 0;
        #endregion

        #region Events
        /// <summary>
        /// A simple event to notify the view that the map has been updated and it should redraw itself.
        /// This is needed because some changes to the map (like tree growth) don't trigger any of the other events, but they still require a redraw.
        /// </summary>
        public event Action? MapUpdated;
        #endregion

        #region Constructors
        public GameViewModel(GameModel model)
        {
            Model = model;

            model.NewGameCreated += Model_NewGameCreated;
            model.GameTicked += Model_GameTicked;
            model.GameAdvanced += Model_GameAdvanced;
            model.InfrastructureBuilt += Model_InfrastructureBuilt;
            model.FieldChanged += Model_FieldChanged;
            model.BalanceChanged += Model_BalanceChanged;
            model.SelectedFieldChanged += Model_SelectedFieldChanged;

            Tiles = [];
            //RefreshTable();

            Tiles2 = model.Map.Table;
            MinimapImage = new(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            GenerateMinimap();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Generates a Minimap based on the current state of the map.
        /// Each tile is represented by a color (pixel) based on its FieldType.
        /// </summary>
        public void GenerateMinimap()
        {
            uint[] pixels = new uint[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Field tile = Map[x, y];

                    int index = (y * Width) + x;

                    pixels[index] = ConvertTileToColor(tile);
                }
            }
            MinimapImage.WritePixels(new Int32Rect(0, 0, Width, Height), pixels, Width * 4, 0);
        }

        public void UpdateMinimapTile(int x, int y, uint newColor)
        {
            uint[] colorData = [newColor];
            Int32Rect updateRect = new(x, y, 1, 1);
            MinimapImage.WritePixels(updateRect, colorData, 4, 0);
        }

        public void OnTileLeftClick(int x, int y)
        {
            if (!IsEditorMode) return;

            switch (SelectedButton)
            {
                case 1:
                    Model.DecreaseHeight(x, y);
                    break;
                case 2:
                    Model.IncreaseHeight(x, y);
                    break;
                case 11:
                    Model.BuildRoad(x, y);
                    break;
                case 12:
                    Model.BuildBridge(x, y);
                    break;
                case 13:
                    Model.BuildStop(x, y);
                    break;
                default:
                    break;
            }
            MapUpdated?.Invoke();
            UpdateMinimapTile(x, y, ConvertTileToColor(Map[x, y]));
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Convert's a tile's FieldType to a color for the minimap.
        /// </summary>
        /// <param name="tile">The field.</param>
        /// <returns>The <see cref="uint"/> ARGB format.</returns>
        private uint ConvertTileToColor(Field tile)
        {
            // Moved from FieldViewModel
            Color colorName = tile.FieldType switch
            {
                // Structures
                FieldType.House => Colors.Yellow,
                FieldType.Farm => Colors.LightGreen,
                FieldType.Mine => Colors.DarkOrange,
                FieldType.LumberCamp => Colors.SaddleBrown,
                FieldType.Mill => Colors.LightGray,
                FieldType.Factory => Colors.DimGray,

                // Infrastructure
                FieldType.Road => Colors.DarkGray,
                FieldType.Bridge => Colors.Red,
                FieldType.Stop => Colors.Red,

                // Terrain
                FieldType.Water => Colors.Blue,
                FieldType.Plain => Colors.Green,
                FieldType.Hill => Colors.DarkGreen,
                FieldType.Mountain => Colors.Gray,
                FieldType.HighMountain => Colors.DarkGreen,

                _ => Colors.Black,
            };

            return ColorConverterUtil.ColorToUInt32(colorName);
        }

        private void Model_SelectedFieldChanged(object? sender, (int, int) e)
        {
            if (Model.SelectedField == null)
            {
                var tile = Tiles.FirstOrDefault(t => t.IsSelected);
                if (tile != null) tile.IsSelected = false;
            }
            else
            {
                var tile = Tiles.FirstOrDefault(t => t.X == e.Item1 && t.Y == e.Item2);
                if (tile != null) tile.IsSelected = true;
            }
        }

        private void Model_BalanceChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Balance));
        }

        [Obsolete("We redraw the map each time, and get all the data directly from GameTable." +
            "If performance becomes and issues this can be considerd.")]
        private void Model_FieldChanged(object? sender, TransportTycoonFieldEventArgs e)
        {
            var tile = Tiles.FirstOrDefault(t => t.X == e.X && t.Y == e.Y);

            if (tile != null)
            {
                tile.RefreshTerrain(Model.Map[e.X, e.Y]);
            }
        }

        [Obsolete("We redraw the map each time, and get all the data directly from GameTable." +
            "If performance becomes and issues this can be considerd.")]
        private void Model_InfrastructureBuilt(object? sender, List<(int, int)> changedFields)
        {
            foreach (var (x, y) in changedFields)
            {
                FieldViewModel? tile = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
                if (tile != null)
                {
                    string oldPath = tile.ImagePath;
                    int index = Tiles.IndexOf(tile);
                    Tiles[index] = new(Model.Map[x, y], oldPath);
                    tile.RefreshInfrastructure();
                }
            }
        }

        private void Model_GameAdvanced(object? _1, List<Tuple<int, int>> _2)
        {
            // O(n * m + m)
            //Tiles.Where(tile => grownTrees.Any(tuple => tuple.Item1 == tile.X && tuple.Item2 == tile.Y))
            //    .ToList()
            //    .ForEach(tile => tile.RefreshTreeCount());
            MapUpdated?.Invoke();
        }

        [Obsolete]
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

        #region Relay commands
        [RelayCommand]
        private void OnNormalSpeed()
        {
            Model.TimeSpeed = TimeSpeed.Normal;
            OnResumeGame();
        }

        [RelayCommand]
        private void OnFastSpeed()
        {
            Model.TimeSpeed = TimeSpeed.Fast;
            OnResumeGame();
        }

        [RelayCommand]
        private void OnSuperFastSpeed()
        {
            Model.TimeSpeed = TimeSpeed.SuperFast;
            OnResumeGame();
        }

        [RelayCommand]
        private void OnPauseGame()
        {
            Model.Mode = GameMode.Paused;
        }

        [RelayCommand]
        private void OnResumeGame()
        {
            Model.Mode = GameMode.Run;
        }

        [RelayCommand]
        private void OnEditorMode()
        {
            Model.Mode = GameMode.Editor;
        }

        [RelayCommand]
        private void OnIncreaseHeight()
        {
            if (Model.SelectedField != null)
            {
                Model.IncreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
            }
        }

        [RelayCommand]
        private void OnDecreaseHeight()
        {
            if (Model.SelectedField != null)
            {
                Model.DecreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
            }
        }

        [RelayCommand]
        private void OnSetSelectedButton(object x)
        {
            if (x == null) return;
            SelectedButton = Convert.ToInt32(x);
            Model.SetSelectedField(-1, -1);
        }
        #endregion

        #region Event methods
        [Obsolete("If map generation takes to long, maybe we can subscribe to it." +
            "But our renderer takes the tiles directly from GameTable")]
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
        public void Dispose()
        {
            Model.NewGameCreated -= Model_NewGameCreated;
            Model.GameTicked -= Model_GameTicked;
            Model.GameAdvanced -= Model_GameAdvanced;
            Model.InfrastructureBuilt -= Model_InfrastructureBuilt;
            Model.FieldChanged -= Model_FieldChanged;
            Model.BalanceChanged -= Model_BalanceChanged;
            Model.SelectedFieldChanged -= Model_SelectedFieldChanged;
        }
        #endregion
    }
}
