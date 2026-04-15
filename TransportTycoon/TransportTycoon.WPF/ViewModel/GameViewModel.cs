using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportTycoon.MapData;
using TransportTycoon.Model;

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
            model.VehicleChanged += Model_VehicleChanged;
            model.SelectedStopFieldsChanged += Model_SelectedStopFieldsChanged;

            Tiles = [];
            Vehicles = [];
            RefreshTable();
        }
        #endregion

        #region Private methods
        private void Model_SelectedStopFieldsChanged(object? _1, List<Stop> list)
        {
            if (list is null) return;
            foreach (var tile in Tiles)
            {
                tile.IsSelected = list.Any(t => t.X == tile.X && t.Y == tile.Y);
                tile.SelectedOrder = list.FindIndex(t => t.X == tile.X && t.Y == tile.Y) + 1;
            }
        }

        private void Model_SelectedFieldChanged(object? _1, (int, int) e)
        {
            if (Model.SelectedField is null)
            {
                var tile = Tiles.FirstOrDefault(t => t.IsSelected);
                tile?.IsSelected = false;
            }
            else
            {
                var tile = Tiles.FirstOrDefault(t => t.X == e.Item1 && t.Y == e.Item2);
                tile?.IsSelected = true;
            }
        }

        private void Model_BalanceChanged(object? _1, EventArgs _2)
        {
            OnPropertyChanged(nameof(Balance));
        }

        private void Model_FieldChanged(object? _1, TransportTycoonFieldEventArgs e)
        {
            var tile = Tiles.FirstOrDefault(t => t.X == e.X && t.Y == e.Y);

            tile?.RefreshTerrain(Model.Map[e.X, e.Y]);
        }
        private void Model_VehicleChanged(object? _1, (int oldX, int oldY, int newX, int newY) e)
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.MapX == e.oldX && v.MapY == e.oldY);
            if (vehicle is not null)
            {
                var newVehicle = Model.GetVehicleAt(e.newX, e.newY);
                if (newVehicle is not null)
                {
                    vehicle.RefreshVehicle(newVehicle);
                }
            }
        }

        private void Model_InfrastructureBuilt(object? _1, List<(int, int)> changedFields)
        {
            foreach (var (x, y) in changedFields)
            {
                FieldViewModel? tile = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
                if (tile is not null)
                {
                    string oldPath = tile.ImagePath;
                    int index = Tiles.IndexOf(tile);
                    Tiles[index] = new(Model.Map[x, y], oldPath);
                    tile.RefreshInfrastructure();
                }
            }
        }

        private void Model_GameAdvanced(object? _1, List<Tuple<int, int>> grownTrees)
        {
            // O(n * m + m)
            Tiles.Where(tile => grownTrees.Any(tuple => tuple.Item1 == tile.X && tuple.Item2 == tile.Y))
                .ToList()
                .ForEach(tile => tile.RefreshTreeCount());
        }

        private void RefreshTable()
        {
#pragma warning disable IDE0028 // Simplify collection initialization, not yet supported in .NET 10
            List<FieldViewModel> tempList = new(Model.Map.Width * Model.Map.Height + 1);
#pragma warning restore IDE0028 // Simplify collection initialization, not yet supported in .NET 10
            for (int x = 0; x < Model.Map.Width; x++)
            {
                for (int y = 0; y < Model.Map.Height; y++)
                {
                    tempList.Add(new(Model.Map[x, y]));
                }
            }

#pragma warning disable IDE0028 // Simplify collection initialization, not yet supported in .NET 10
            Tiles = new(tempList);
#pragma warning restore IDE0028 // Simplify collection initialization, not yet supported in .NET 10
        }

        partial void OnSelectedTabIndexChanged(int value)
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
            if (Model.SelectedField is not null) Model.SetSelectedField(-1, -1);
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
        private void OnVehicleStepUp()
        {
            Model.StepAllVehicles(Direction.Up);
        }
        [RelayCommand]
        private void OnVehicleStepDown()
        {
            Model.StepAllVehicles(Direction.Down);
        }
        [RelayCommand]
        private void OnVehicleStepLeft()
        {
            Model.StepAllVehicles(Direction.Left);
        }
        [RelayCommand]
        private void OnVehicleStepRight()
        {
            Model.StepAllVehicles(Direction.Right);
        }


        [RelayCommand(CanExecute = nameof(IsEditorMode))]
        private void OnTileLeftClick(FieldViewModel? tile)
        {
            if (tile is null) return;
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
                    Vehicle? vehicle = Model.BuyVehicle(tile.X, tile.Y, VehicleType.SmallBus)!;
                    if (vehicle is not null)
                    {
                        Vehicles.Add(new VehicleViewModel(vehicle));
                    }
                    break;
                case 32:
                    Vehicle? vehicle2 = Model.BuyVehicle(tile.X, tile.Y, VehicleType.BigBus)!;
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
        }
        #endregion

        #region Event methods
        private void Model_NewGameCreated(object? _1, EventArgs _2)
        {
            RefreshTable();
        }

        private void Model_GameTicked(object? _1, EventArgs _2)
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
