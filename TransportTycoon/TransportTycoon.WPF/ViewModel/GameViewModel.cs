using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;
using TransportTycoon.Model;
using TransportTycoon.WPF.Utils;

namespace TransportTycoon.WPF.ViewModel
{
    public sealed partial class GameViewModel : ViewModelViewConstraintBase, IDisposable
    {
        #region Properties
        #region IViewConstraints
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #endregion

        public GameModel Model { get; init; }

        public ObservableCollection<VehicleViewModel> Vehicles { get; private set; }

        public int Balance => Model.Balance;
        public ulong GameTime => Model.GameTime;
        public bool IsPaused => Model.Mode == GameMode.Paused;
        public bool IsEditorMode => Model.Mode == GameMode.Editor;

        #region Map
        public GameTable Map => Model.Map;
        public int Width => Model.Map.Width;
        public int Height => Model.Map.Height;
        public IField[,] Tiles { get; }
        public WriteableBitmap MinimapImage { get; set; }
        #endregion

        [ObservableProperty]
        private int _selectedTabIndex = 0;
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

            model.GameTicked += Model_GameTicked;
            model.GameAdvanced += Model_GameAdvanced;
            model.InfrastructureBuilt += Model_InfrastructureBuilt;
            model.FieldChanged += Model_FieldChanged;
            model.BalanceChanged += Model_BalanceChanged;
            model.SelectedFieldChanged += Model_SelectedFieldChanged;
            model.VehicleChanged += Model_VehicleChanged;
            model.SelectedStopFieldsChanged += Model_SelectedStopFieldsChanged;

            Vehicles = [];
            Tiles = model.Map.Table;
            MinimapImage = new(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            GenerateMinimap();
        }
        #endregion


        #region Public methods
        public void PauseGame()
        {
            Model.Mode = GameMode.Paused;
            OnPropertyChanged(nameof(IsPaused));
        }

        public void ResumeGame()
        {
            Model.Mode = GameMode.Run;
            if (Model.SelectedField is not null) Model.SetSelectedField(-1, -1);
            Model.DeleteRoute(-1, -1);
            OnPropertyChanged(nameof(IsPaused));
        }

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
                    IField tile = Map[x, y];

                    int index = (y * Width) + x;

                    pixels[index] = ConvertTileToColor(tile);
                }
            }
            MinimapImage.WritePixels(new Int32Rect(0, 0, Width, Height), pixels, Width * 4, 0);
        }

        /// <summary>
        /// Updates the tile (pixel) on the minimap based the given color.
        /// </summary>
        /// <param name="x">The tile's (pixel) X coordinate.</param>
        /// <param name="y">The tile's (pixel) Y coordinate.</param>
        /// <param name="newColor">The color in ARGB format.</param>
        public void UpdateMinimapTile(int x, int y, uint newColor)
        {
            uint[] colorData = [newColor];
            Int32Rect updateRect = new(x, y, 1, 1);
            MinimapImage.WritePixels(updateRect, colorData, 4, 0);
        }


        public void OnTileLeftClick(int x, int y)
        {
            // Predicate
            if (!IsEditorMode) return;

            var tile = Tiles[x, y];

            switch (SelectedButton)
            {
                case 11:
                    Model.DecreaseHeight(tile.X, tile.Y);
                    break;
                case 12:
                    Model.IncreaseHeight(tile.X, tile.Y);
                    break;
                case 21:
                    Model.BuildRoad(tile.X, tile.Y);
                    break;
                case 22:
                    Model.BuildBridge(tile.X, tile.Y);
                    break;
                case 23:
                    Model.BuildStop(tile.X, tile.Y);
                    break;
                case 24:
                    Model.Destroy(tile.X, tile.Y);
                    break;
                case 31:
                    Vehicle? vehicle = Model.BuyVehicle(tile.X, tile.Y, VehicleType.SmallBus);
                    if (vehicle is not null)
                    {
                        Vehicles.Add(new VehicleViewModel(vehicle));
                    }
                    break;
                case 32:
                    Vehicle? vehicle2 = Model.BuyVehicle(tile.X, tile.Y, VehicleType.BigBus);
                    if (vehicle2 is not null)
                    {
                        Vehicles.Add(new VehicleViewModel(vehicle2));
                    }
                    break;
                case 41:
                    Model.DefineRoute(tile.X, tile.Y);
                    break;
                case 42:
                    Model.QueryRoute(tile.X, tile.Y);
                    break;
                case 43:
                    Model.AssignRoute(tile.X, tile.Y);
                    break;
                case 44:
                    Model.DeleteRoute(tile.X, tile.Y);
                    break;
                default:
                    break;
            }
            MapUpdated?.Invoke();
            UpdateMinimapTile(x, y, ConvertTileToColor(Map[x, y]));
        }
        #endregion

        #region Private methods
        //private void ShowRoute(HashSet<(int X, int Y)>? roads)
        //{
        //    if (roads is null) return;
        //    foreach (var (x, y) in roads)
        //    {
        //        var tile = Tiles[x, y];
        //        tile?.IsPath = true;
        //    }
        //}

        //private void RemoveRoutes()
        //{
        //    foreach (var item in Tiles)
        //    {
        //        item.IsPath = false;
        //    }
        //}

        /// <summary>
        /// Convert's a tile's <see cref="FieldType"/> to a color for the minimap.
        /// </summary>
        /// <param name="tile">The field.</param>
        /// <returns>The <see cref="uint"/> ARGB format.</returns>
        private uint ConvertTileToColor(IField tile)
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
            PauseGame();
        }

        [RelayCommand]
        private void OnResumeGame()
        {
            ResumeGame();
        }

        [RelayCommand]
        private void OnEditorMode()
        {
            Model.Mode = GameMode.Editor;
        }

        [RelayCommand]
        private void OnIncreaseHeight()
        {
            if (Model.SelectedField is not null)
            {
                Model.IncreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
            }
        }

        [RelayCommand]
        private void OnDecreaseHeight()
        {
            if (Model.SelectedField is not null)
            {
                Model.DecreaseHeight(Model.SelectedField.X, Model.SelectedField.Y);
            }
        }

        [RelayCommand]
        private void OnSetSelectedButton(object? x)
        {
            if (x is null) return;
            SelectedButton = Convert.ToInt32(x);
            if (SelectedButton < 10)
            {
                if (Model.SelectedField is not null) Model.SetSelectedField(-1, -1);
                Model.DeleteRoute(-1, -1);
            }
            else if (SelectedButton > 20 && SelectedButton < 30 && SelectedButton != 22 && Model.SelectedField is not null) Model.SetSelectedField(-1, -1);
            else if (SelectedButton > 40 && SelectedButton == 42) Model.DeleteRoute(-1, -1);
        }

        [RelayCommand]
        private void OnExitGame()
        {
            App.Current.Shutdown();
        }

        [RelayCommand]
        private async Task OnSaveGame()
        {
            var fileDiag = new OpenFileDialog
            {
                Title = "Choose a save location",
                Filter = "JSON files|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                RestoreDirectory = true,
            };

            bool? result = fileDiag.ShowDialog();
            if (result is null || result == false) return;

            var uri = fileDiag.FileName;
            await Model.SaveGame(uri);
        }

        #endregion

        #region Event methods
        private void Model_NewGameCreated(object? _1, EventArgs _2) { }
        // Ha a renderelés sokáig tartanak feliratkozhatunk erre,
        // de automatikus megjelenik a térkép, ha kész a render.
        private void Model_GameTicked(object? _1, EventArgs _2)
        {
            OnPropertyChanged(nameof(GameTime));
        }

        private void Model_SelectedStopFieldsChanged(object? _1, List<Stop> list)
        {
            //foreach (var tile in Tiles)
            //{
            //    tile.IsSelected = list.Any(t => t.X == tile.X && t.Y == tile.Y);
            //    tile.SelectedOrder = list.FindIndex(t => t.X == tile.X && t.Y == tile.Y) + 1;
            //}
        }

        private void Model_SelectedFieldChanged(object? _1, (int, int) e)
        {
            //if (Model.SelectedField is null)
            //{
            //    var tile = Tiles.FirstOrDefault(t => t.IsSelected);
            //    tile?.IsSelected = false;
            //}
            //else
            //{
            //    var tile = Tiles.FirstOrDefault(t => t.X == e.Item1 && t.Y == e.Item2);
            //    tile?.IsSelected = true;
            //}
        }

        private void Model_BalanceChanged(object? _1, EventArgs _2)
        {
            OnPropertyChanged(nameof(Balance));
        }

        private void Model_FieldChanged(object? _1, TransportTycoonFieldEventArgs e)
        {
            //var tile = Tiles.FirstOrDefault(t => t.X == e.X && t.Y == e.Y);

            //tile?.RefreshTerrain(Model.Map[e.X, e.Y]);
        }

        private void Model_VehicleChanged(object? _1, Vehicle e)
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.Vehicle == e);
            vehicle?.RefreshVehicle(e);
        }

        // TODO: hídak esetén minden Minimapbeli pontot frissíteni ez alapján
        // TODO2: esetleg egy olyan Minimap frissítő ami listával végzi el
        private void Model_InfrastructureBuilt(object? _1, List<(int x, int y)> _2)
        {
            //foreach (var (x, y) in changedFields)
            //{
            //    FieldViewModel? tile = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
            //    if (tile != null)
            //    {
            //        string oldPath = tile.ImagePath;
            //        int index = Tiles.IndexOf(tile);
            //        Tiles[index] = new(Model.Map[x, y], oldPath);
            //        tile.RefreshInfrastructure();
            //    }
            //}
        }

        // Ez is felesleges lesz, össze kell vonni majd az eventeket, hogy csak globális frissítő event legyen
        private void Model_GameAdvanced(object? _1, List<Tuple<int, int>> _2)
        {
            MapUpdated?.Invoke();
        }

        partial void OnSelectedTabIndexChanged(int value)
        {
            OnSetSelectedButton(value);
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
            Model.VehicleChanged -= Model_VehicleChanged;
        }
        #endregion
    }
}
