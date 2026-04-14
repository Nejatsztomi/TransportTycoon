using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportTycoon.MapData;
using TransportTycoon.Model;

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

        public ObservableCollection<FieldViewModel> Tiles { get; private set; }
        public ObservableCollection<VehicleViewModel> Vehicles { get; private set; }

        public int Balance => Model.Balance;
        public int GameTime => Model.GameTime;
        public bool IsPaused => Model.Mode == GameMode.Paused;
        public bool IsEditorMode => Model.Mode == GameMode.Editor;

        #region Map
        public int MapColumns => Model.Map.Width;
        public int MapRows => Model.Map.Height;
        [ObservableProperty]
        private double _zoomLevel = 1.0;
        #endregion
        [ObservableProperty]
        private int _selectedTabIndex = 0;
        [ObservableProperty]
        private int _selectedButton = 0;
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
            model.SelectedStopFieldsChanged += Model_SelectedStopFieldsChanged;

            Tiles = [];
            Vehicles = [];
            RefreshTable();
        }
        #endregion

        #region Private methods
        private void Model_SelectedStopFieldsChanged(object? sender, List<Stop> list)//
        {
            if (list == null) return;
            foreach (var tile in Tiles)
            {
                tile.IsSelected = list.Any(t => t.X == tile.X && t.Y == tile.Y);
                tile.SelectedOrder = list.FindIndex(t => t.X == tile.X && t.Y == tile.Y) + 1;
            }
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
                    Tiles[index] = new(Model.Map[x, y], oldPath);
                    tile.RefreshInfrastructure();
                }
            }
        }

        private void Model_GameAdvanced(object? sender, List<Tuple<int, int>> grownTrees)
        {
            // O(n * m + m)
            Tiles.Where(tile => grownTrees.Any(tuple => tuple.Item1 == tile.X && tuple.Item2 == tile.Y))
                .ToList()
                .ForEach(tile => tile.RefreshTreeCount());
        }

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

        partial void OnSelectedTabIndexChanged(int value)//
        {
            OnSetSelectedButton(value);
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
            if (Model.SelectedField != null) Model.SetSelectedField(-1, -1);
            Model.DeleteRoute(-1, -1);
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
            if (SelectedButton < 10)
            {
                if (Model.SelectedField != null) Model.SetSelectedField(-1, -1);
                Model.DeleteRoute(-1, -1);
            }
            else if (SelectedButton > 20 && SelectedButton < 30 && SelectedButton != 22 && Model.SelectedField != null) Model.SetSelectedField(-1, -1);
            else if (SelectedButton > 40 && SelectedButton == 42) Model.DeleteRoute(-1, -1);
        }

        [RelayCommand(CanExecute = nameof(IsEditorMode))]
        private void OnTileLeftClick(FieldViewModel tile)
        {
            if (tile == null) return;
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
                    Vehicle vehicle = Model.BuyVehicle(tile.X, tile.Y, VehicleType.SmallBus)!;
                    if (vehicle != null)
                    {
                        Vehicles.Add(new VehicleViewModel(vehicle));
                    }
                    break;
                case 32:
                    Vehicle vehicle2 = Model.BuyVehicle(tile.X, tile.Y, VehicleType.BigBus)!;
                    if (vehicle2 != null)
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
        public void Dispose()
        {
            Model.NewGameCreated -= Model_NewGameCreated;
            Model.GameTicked -= Model_GameTicked;
            Model.GameAdvanced -= Model_GameAdvanced;
            Model.InfrastructureBuilt -= Model_InfrastructureBuilt;
            Model.FieldChanged -= Model_FieldChanged;
            Model.BalanceChanged -= Model_BalanceChanged;
        }
        #endregion
    }
}
