using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
namespace TransportTycoon.Model
{
    public enum GameMode { Run, Paused, Editor }
    public enum TimeSpeed { Normal = 1, Fast = 2, SuperFast = 3 }
    public enum Difficulty { Easy = 0, Medium = 1, Hard = 2 }

    //Mintázat az összes osztályban
    #region Fields
    #endregion
    #region Properties
    #endregion
    #region Constructor
    #endregion
    #region Public Methods
    #endregion
    #region Private Methods
    #endregion
    #region Private event Methods
    #endregion

    public sealed class GameModel
    {
        #region Constants
        /// <summary>
        /// Represents the default interval value, in milliseconds, used for timing operations.
        /// </summary>
        public const int DefaultInterval = 1_000;

        /// <summary>
        /// Default starting balance for new game.
        /// </summary>
        public const int DefaultBalance = 1_000_000;
        /// <summary>
        /// Default starting difficulty for new game.
        /// </summary>
        public const Difficulty DefaultDifficulty = Difficulty.Medium;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        #endregion

        #region Properties
        public GameTable Map { get; private set; }
        public Field? SelectedField { get; private set; }
        public List<Stop> SelectedStopFields { get; private set; } = [];
        public int Balance { get; private set; }
        public int GameTime { get; private set; }
        public int Maintance { get; private set; }

        public GameMode Mode
        {
            get;
            set
            {
                if (value == GameMode.Paused || value == GameMode.Editor)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                }
                GameModeChanged?.Invoke(this, value);
                field = value;
            }
        }
        public TimeSpeed TimeSpeed
        {
            get;
            set
            {
                _timer.Interval = DefaultInterval / (double)(value);
                TimeSpeedChanged?.Invoke(this, value);
                field = value;
            }
        }
        public Difficulty Difficulty { get; }

        public bool IsGameOver => Balance <= 0;

        public List<Vehicle> Vehicles { get; private set; } = [];

        public int NumberOfVehicles => Vehicles.Count;
        public Vehicle? GetVehicleAt(int x, int y)
        {
            return Vehicles.FirstOrDefault(v => v.MapX == x && v.MapY == y);
        }
        #endregion

        #region Events
        public event EventHandler? NewGameCreated;
        public event EventHandler<GameMode>? GameModeChanged;
        public event EventHandler<TimeSpeed>? TimeSpeedChanged;
        public event EventHandler<TransportTycoonEventArgs>? GameOver;
        public event EventHandler<TransportTycoonFieldEventArgs>? FieldChanged;
        public event EventHandler? BalanceChanged;
        public event EventHandler? GameTicked;
        public event EventHandler<List<Tuple<int, int>>>? GameAdvanced;
        public event EventHandler<List<(int, int)>>? InfrastructureBuilt;
        public event EventHandler<(int, int)>? SelectedFieldChanged;
        public event EventHandler<(int oldX, int oldY, int newX, int newY)>? VehicleChanged;
        public event EventHandler<List<Stop>>? SelectedStopFieldsChanged;
        #endregion

        #region Constructor
        public GameModel(GameTable map, ITimer timer, Difficulty difficulty = DefaultDifficulty, int balance = DefaultBalance)
        {
            Difficulty = difficulty;
            Balance = balance;
            Map = map;
            _timer = timer;
            _timer.Elapsed += Timer_Tick;

            SetTax();
            Mode = GameMode.Run;
            TimeSpeed = TimeSpeed.Normal;
            GameTime = 0;
        }
        #endregion

        #region Public Methods
        public void NewGame()
        {
            Balance = DefaultBalance;

            Map.GenerateMap();
            _timer.Start();
            NewGameCreated?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedField(int x, int y)
        {
            if (x == -1 && y == -1) SelectedField = null;
            else SelectedField = Map[x, y];
            SelectedFieldChanged?.Invoke(this, (x, y));
        }

        public void IncreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height + 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.FieldType != FieldType.Road)
                    {
                        if (field.Height == 4) return;
                        if (terrain.Trees > 0)
                        {
                            Balance -= 50;
                        }
                        Balance -= 100;
                        terrain.IncreaseHeight();
                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }
                    }
                }
            }
        }

        public void DecreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height - 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight) && terrain.FieldType != FieldType.Road)
                    {
                        if (field.Height == 1) return;
                        if (terrain.Trees > 0)
                        {
                            Balance -= 50;
                        }
                        Balance -= 100;
                        terrain.DecreaseHeight();
                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }

                    }
                }
            }
        }
        public void BuildRoad(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;
            List<(int, int)> changedFields = [];

            int oldTrees = Map[x, y].GetTrees();
            Map[x, y] = new Road(x, y, Map.CalculateRoadType(x, y), Map[x, y].Height);
            changedFields.Add((x, y));

            if (oldTrees == 0) Balance -= ((Road)Map[x, y]).Price;
            else Balance -= ((Road)Map[x, y]).Price * 2;

            foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
            {
                if (e != null && e is Road road)
                {
                    road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                    changedFields.Add((e.X, e.Y));
                }
            }
            if (IsGameOver) OnGameOver();
            InfrastructureBuilt?.Invoke(this, changedFields);
            BalanceChanged?.Invoke(this, EventArgs.Empty);
        }
        public void BuildBridge(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Water) { SetSelectedField(-1, -1); return; }
            if (SelectedField == null) SetSelectedField(x, y);
            else
            {
                List<(int, int)> changedFields = [];
                if (SelectedField.X != x && SelectedField.Y != y) { SetSelectedField(-1, -1); return; }
                else if (SelectedField.X == x && SelectedField.Y == y)
                {
                    Balance -= Map.CreateShortBridge(x, y, ref changedFields);
                }
                else if (SelectedField.X == x)
                {
                    if (Math.Min(SelectedField.Y, y) - 1 < 0 || Map[x, Math.Min(SelectedField.Y, y) - 1].Height != 1 ||
                        Math.Max(SelectedField.Y, y) + 1 >= Map.Width || Map[x, Math.Max(SelectedField.Y, y) + 1].Height != 1)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                    int dif = Math.Abs(SelectedField.Y - y);
                    BridgeType b_type = Map.CalculateBridgeType(dif, "horizontal");
                    if (b_type == BridgeType.Null) { SetSelectedField(-1, -1); return; }

                    for (int i = Math.Min(SelectedField.Y, y) + 1; i < Math.Max(SelectedField.Y, y); i++)
                    {
                        if (Map[x, i] is not Water) { SetSelectedField(-1, -1); return; }
                    }
                    Balance -= Map.CreateHorizontalBridge(x, Math.Min(SelectedField.Y, y), Math.Max(SelectedField.Y, y), b_type, ref changedFields);
                }
                else if (SelectedField.Y == y)
                {
                    if (Math.Min(SelectedField.X, x) - 1 < 0 || Map[Math.Min(SelectedField.X, x) - 1, y].Height != 1 ||
                        Math.Max(SelectedField.X, x) + 1 >= Map.Height || Map[Math.Max(SelectedField.X, x) + 1, y].Height != 1)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }

                    int dif = Math.Abs(SelectedField.X - x);
                    BridgeType b_type = Map.CalculateBridgeType(dif, "vertical");
                    if (b_type == BridgeType.Null) { SetSelectedField(-1, -1); return; }

                    for (int i = Math.Min(SelectedField.X, x); i <= Math.Max(SelectedField.X, x); i++)
                    {
                        if (Map[i, y] is not Water) { SetSelectedField(-1, -1); return; }
                    }
                    Balance -= Map.CreateVerticalBridge(y, Math.Min(SelectedField.X, x), Math.Max(SelectedField.X, x), b_type, ref changedFields);
                }
                SetSelectedField(-1, -1);
                if (IsGameOver) OnGameOver();
                InfrastructureBuilt?.Invoke(this, changedFields);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void BuildStop(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;
            List<(int, int)> changedFields = [];

            int oldTrees = Map[x, y].GetTrees();
            if (Map.StopEnvironment(x, y))
            {
                changedFields.Add((x, y));

                if (oldTrees == 0) Balance -= ((Stop)Map[x, y]).Price;
                else Balance -= ((Stop)Map[x, y]).Price * 2;

                foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
                {
                    if (e != null && e is Road road)
                    {
                        road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                        changedFields.Add((e.X, e.Y));
                    }
                }
                if (IsGameOver) OnGameOver();
                InfrastructureBuilt?.Invoke(this, changedFields);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void Destroy(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Infrastructure || (Map[x, y] is Road r && r.InCity())
                || Vehicles.Any(v => v.MapX == x && v.MapY == y)) return;
            List<(int, int)> changedFields = [];

            if (Map[x, y] is Road || Map[x, y] is Stop)
            {
                Map[x, y] = new Terrain(x, y, Map[x, y].Height);
                changedFields.Add((x, y));

                foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
                {
                    if (e != null && e is Road road)
                    {
                        road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                        changedFields.Add((e.X, e.Y));
                    }
                }
            }
            else
            {
                Map.DestroyBridge(x, y, ref changedFields);
            }
            InfrastructureBuilt?.Invoke(this, changedFields);
        }

        //Create a new Vehicle based on the given type and coordinates, and add it to the player's collection if they have enough balance. Returns the created Vehicle.
        public Vehicle? BuyVehicle(int x, int y, VehicleType type)
        {
            if (Map[x, y] is not Stop) return null;

            Vehicle vehicle = type switch
            {
                VehicleType.Van => new Van(x, y, Direction.Up),
                VehicleType.Pickup => new Pickup(x, y, Direction.Up),
                VehicleType.Truck => new Truck(x, y, Direction.Up),
                VehicleType.LiquidTruck => new LiquidTruck(x, y, Direction.Up),
                VehicleType.SmallBus => new SmallBus(x, y, Direction.Up),
                VehicleType.BigBus => new BigBus(x, y, Direction.Up),
                _ => throw new ArgumentException("Invalid vehicle type", nameof(type)),
            };

            if (Balance >= vehicle.Price)
            {
                Balance -= vehicle.Price;
                Vehicles.Add(vehicle);
                BalanceChanged?.Invoke(this, EventArgs.Empty);
                if (IsGameOver)
                {
                    OnGameOver();
                }
            }
            return vehicle;
        }
        /// <summary>
        /// Advances the state of all vehicles in the game if the game is currently running.
        /// </summary>
        /// <remarks>This method iterates through the collection of vehicles and updates each one by
        /// invoking the step operation. No action is taken if the game mode is not set to run.</remarks>
        public void StepAllVehicles(Direction dir = Direction.Up)
        {
            if (Mode != GameMode.Run) return;
            foreach (Vehicle vehicle in Vehicles)
            {
                Step(vehicle, dir);
            }
        }
        public void DefineRoute(int x, int y)
        {
            if (Map[x, y] is not Stop) return;
            SelectedStopFields.Add((Stop)Map[x, y]);
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }
        public void QueryRoute(int x, int y)
        {
            Vehicle? selectedVehcile = Vehicles.Find(v => Math.Abs(v.X - x) < 0.0001 && Math.Abs(v.Y - y) < 0.0001);
            if (selectedVehcile != null) return;
            //SelectedStopFields = selectedVehcile.Prouth.Stops;
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }
        public void AssignRoute(int x, int y)
        {
            if (SelectedStopFields.Count == 0) return;

            Vehicle? selectedVehcile = Vehicles.Find(v => Math.Abs(v.X - x) < 0.0001 && Math.Abs(v.Y - y) < 0.0001);
            if (selectedVehcile == null) return;
            //selectedVehcile.SetProuth(SelectedStopFields)
            SelectedStopFields = [];
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }
        public void DeleteRoute(int x, int y)
        {
            if (SelectedStopFields.Count == 0) return;

            if (x == -1 && y == -1) SelectedStopFields = [];
            else
            {
                Stop? removeItem = SelectedStopFields.Find(s => s.X == x && s.Y == y);
                if (removeItem == null) return;
                SelectedStopFields.Remove(removeItem);
            }
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }
        #endregion

        #region Private Methods
        private void SetTax()
        {
            int tax = 30;
            switch (this.Difficulty)
            {
                case Difficulty.Easy:
                    tax = 10;
                    break;
                case Difficulty.Medium:
                    tax = 30;
                    break;
                case Difficulty.Hard:
                    tax = 50;
                    break;

            }
            Goods.SetGlobalTax(tax);
        }
        private List<Tuple<int, int>> ForestGrowing()
        {
            List<Tuple<int, int>> grownTrees = [];

            Random rnd = new();
            HashSet<Field> spreadedFields = [];
            for (int i = 0; i < Map.Height; i++)
            {
                for (int j = 0; j < Map.Width; j++)
                {
                    if (Map[i, j] is Terrain terrain && terrain.Trees > 0 && !terrain.IsFull)
                    {
                        if (rnd.Next(1, 101) <= 10)
                        {
                            if (terrain.Grow())
                            {
                                grownTrees.Add(new(i, j));
                            }

                            if (terrain.IsFull)
                            {
                                spreadedFields.UnionWith(Map.CheckNeighboringTrees(i, j));
                            }
                        }
                    }
                }
            }

            foreach (Field field in spreadedFields)
            {
                if (field is Terrain terrain && rnd.Next(1, 101) <= 100)
                {
                    terrain.SpreadForest();
                    grownTrees.Add(new(terrain.X, terrain.Y));
                }
            }

            return grownTrees;
        }
        /// <summary>
        /// Updates the position of the specified vehicle based on its current direction and speed, provided the game is
        /// in Run mode.
        /// </summary>
        /// <remarks>The method checks if the new coordinates are within the map boundaries and whether
        /// the vehicle can move to the new position, which must be an infrastructure. If the game is not in Run mode,
        /// the vehicle does not move.</remarks>
        /// <param name="vehicle">The vehicle to be moved, which influences its new position based on its direction and speed.</param>
        private void Step(Vehicle vehicle, Direction direction = Direction.Up)
        {
            //if the game is not in Run mode, the vehicles should not move
            if (Mode != GameMode.Run) return;

            vehicle.ChangeCurrentSpeed(vehicle.TopSpeed);

            //Calculates the new coordinates of the vehicle based on its current direction and speed.
            Direction dir = vehicle.Direction;
            double x = vehicle.X;
            double y = vehicle.Y;
            double speed = vehicle.CurrentSpeed;
            switch (direction)
            {
                case Direction.Up:
                    x -= speed;
                    break;
                case Direction.Down:
                    x += speed;
                    break;
                case Direction.Left:
                    y -= speed;
                    break;
                case Direction.Right:
                    y += speed;
                    break;
                default:
                    break;
            }
            int newX = (int)Math.Floor(x);
            int newY = (int)Math.Floor(y);

            if (0 > newX || newX >= Map.Width || 0 > newY || newY >= Map.Height) return;


            //Checks if the new coordinates are within the map boundaries and if the vehicle can move to the new position (i.e., it must be an infrastructure).
            if (0 > newX || newX >= Map.Width || 0 > newY || newY >= Map.Height)
            {
                vehicle.ChangeCurrentSpeed(0);
                return;
            }
            Field newField = Map[newX, newY];
            if (newField is not Infrastructure)
            {
                vehicle.ChangeCurrentSpeed(0);
                return;
            }

            Field currentField = Map[vehicle.MapX, vehicle.MapY];
            Vehicle? nextVehicle = Vehicles.FirstOrDefault(v => v != vehicle && v.MapX == newX && v.MapY == newY);

            SetVehicleSpeed(vehicle, nextVehicle, currentField, newField);
            if (vehicle.CurrentSpeed > 0)
            {
                vehicle.Step(direction);
                VehicleChanged?.Invoke(this, (currentField.X, currentField.Y, vehicle.MapX, vehicle.MapY));
            }

            //vehicle.UpdateDirection();
        }

        /// <summary>
        /// Sets the speed of the given vehicle based on the type of the new field it is moving to, and the presence of another vehicle on that field. 
        /// If the new field is a bridge, the vehicle's speed should be limited to the bridge's speed limit.
        /// If there is another vehicle on the new field, the current vehicle's speed should be limited to the speed of that vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="nextVehicle"></param>
        /// <param name="currentField"></param>
        /// <param name="newField"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetVehicleSpeed(Vehicle vehicle, Vehicle? nextVehicle, Field currentField, Field newField)
        {
            //if the vehicle will be on a bridge, it should slow down to its speedlimit
            if (newField is Bridge bridge)
            {
                vehicle.ChangeCurrentSpeed(Math.Min(vehicle.CurrentSpeed, bridge.SpeedLimit));
            }

            //if the newField is Incline, the vehicle should slow down to half of its current speed
            if (newField.Height > currentField.Height)
            {
                vehicle.ChangeCurrentSpeed(vehicle.CurrentSpeed / 2);
            }

            //if the next field has another vehicle on it, the current vehicle should slow down to the speed of that vehicle, or stop if the other vehicle is on a different field (to avoid collisions)
            if (nextVehicle != null)
            {
                bool isOppositeDirection = (vehicle.Direction == Direction.Up && nextVehicle.Direction == Direction.Down) ||
                    (vehicle.Direction == Direction.Down && nextVehicle.Direction == Direction.Up) ||
                    (vehicle.Direction == Direction.Left && nextVehicle.Direction == Direction.Right) ||
                    (vehicle.Direction == Direction.Right && nextVehicle.Direction == Direction.Left);

                if (!isOppositeDirection)
                {
                    //if the next vehicle is on a different field
                    if (currentField != newField)
                    {

                        vehicle.ChangeCurrentSpeed(0);
                    }
                    else //if they are on the same field
                    {
                        vehicle.ChangeCurrentSpeed(Math.Min(vehicle.CurrentSpeed, nextVehicle.CurrentSpeed));
                    }
                }

            }
        }
        #endregion

        #region Private event Methods
        private void OnGameOver()
        {
            _timer.Stop();
            GameModeChanged?.Invoke(this, GameMode.Paused);
            GameOver?.Invoke(this, new TransportTycoonEventArgs(GameTime, NumberOfVehicles, Maintance));
        }
        /// <summary>
        /// Processes the transfer of goods between all vehicles and buildings at their respective stop locations on the
        /// map.
        /// </summary>
        /// <remarks>This method iterates through all vehicles, determining if each is currently at a
        /// stop. For vehicles at a stop, it manages the loading and unloading of goods based on the vehicle's accepted
        /// goods, current load, and capacity, as well as the needs and supplies of the buildings present. The method
        /// updates the vehicle's state and the overall balance accordingly. This operation is typically called as part
        /// of the game's main update loop to simulate ongoing transport activity.</remarks>
        private void AllVehiclesDoTheTransport()
        {
            foreach (var vehicle in Vehicles)
            {
                if (IsCarOnStop(vehicle))
                {
                    Field currentField = Map[vehicle.MapX, vehicle.MapY];
                    if (currentField is Stop stop)
                    {
                        List<LoadType> vehicleAcceptedGoods = vehicle.AcceptedGoods!;

                        List<BuildingBlocks> buildings_giver = stop.ShowWhatTheBuildingsCanGive(vehicleAcceptedGoods);
                        List<BuildingBlocks> buildings_taker = stop.ShowWhatTheBuildingsCanGet(vehicleAcceptedGoods);

                        if (buildings_giver.Count == 0 && buildings_taker.Count == 0) continue;

                        //vehicle gives to the building
                        if (vehicle.CurrentCapacity > 0 && vehicle.CurrentLoad != null)
                        {
                            int vehicleCanGive;
                            LoadType? vehicleLoad = vehicle.CurrentLoad?.LoadType;
                            foreach (var building in buildings_taker)
                            {
                                if (building.BuildingEntity is IndustryEntity industry)
                                {
                                    if (vehicleLoad == building.BuildingEntity.GetConsumeLoad()?.LoadType)
                                    {
                                        vehicleCanGive = vehicle.CurrentCapacity;
                                        int buildingCanTake = industry.MaxConsumeCapacity - industry.ConsumeOccupancy;
                                        if (buildingCanTake >= vehicleCanGive)
                                        {
                                            int buildingNewCapacity = industry.ConsumeOccupancy + vehicleCanGive;
                                            Balance += vehicleCanGive * vehicle.CurrentLoad!.Price;
                                            BalanceChanged?.Invoke(this, EventArgs.Empty);
                                            industry.SetConsumeOccupancy(buildingNewCapacity);
                                            vehicle.SetCurrentCapacity(0);
                                            vehicle.SetCurrentLoad(null);

                                            break;
                                        }
                                        else
                                        {
                                            vehicleCanGive -= buildingCanTake;
                                            Balance += buildingCanTake * vehicle.CurrentLoad!.Price;
                                            BalanceChanged?.Invoke(this, EventArgs.Empty);
                                            industry.SetConsumeOccupancy(industry.MaxConsumeCapacity);
                                            vehicle.SetCurrentCapacity(vehicleCanGive);
                                        }
                                    }
                                }
                                else if (building.BuildingEntity is CityEntity city)
                                {
                                    vehicleCanGive = vehicle.CurrentCapacity;
                                    if (vehicleLoad == city.GetConsumeLoad()?.LoadType)
                                    {
                                        Balance += vehicleCanGive * vehicle.CurrentLoad!.Price;
                                        BalanceChanged?.Invoke(this, EventArgs.Empty);
                                        vehicle.SetCurrentCapacity(0);
                                        vehicle.SetCurrentLoad(null);
                                        break;
                                    }
                                }
                            }
                        }

                        //bulding gives to the vehicle
                        int vehicleCanTake = vehicle.MaxCapacity - vehicle.CurrentCapacity;
                        if (vehicleCanTake > 0)
                        {
                            foreach (var building in buildings_giver)
                            {
                                Load buildingLoad = building.BuildingEntity.GetProvideLoad();

                                bool acceptsLoad = vehicleAcceptedGoods.Contains(buildingLoad.LoadType);
                                bool isEmptyOrSameLoad = (vehicle.CurrentCapacity == 0) || (vehicle.CurrentLoad?.LoadType == buildingLoad.LoadType);

                                if (acceptsLoad && isEmptyOrSameLoad)
                                {
                                    int buildingCanGive = building.BuildingEntity.CurrentCapacity;
                                    if (buildingCanGive >= vehicleCanTake)
                                    {
                                        int buildingNewCapacity = buildingCanGive - vehicleCanTake;
                                        building.BuildingEntity.SetCurrentCapacity(buildingNewCapacity);
                                        vehicle.SetCurrentCapacity(vehicle.MaxCapacity);
                                        vehicle.SetCurrentLoad(buildingLoad);
                                        vehicleCanTake = 0;
                                        break;
                                    }
                                    else
                                    {
                                        vehicleCanTake -= buildingCanGive;
                                        building.BuildingEntity.SetCurrentCapacity(0);
                                        vehicle.SetCurrentCapacity(vehicle.CurrentCapacity + buildingCanGive);
                                        vehicle.SetCurrentLoad(buildingLoad);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determines whether the specified vehicle is currently located on a stop field within the map boundaries.
        /// </summary>
        /// <remarks>The method returns false if the vehicle's coordinates are outside the map
        /// boundaries.</remarks>
        /// <param name="v">The vehicle to check for presence on a stop field. Must have valid map coordinates.</param>
        /// <returns>true if the vehicle is positioned on a stop field within the map; otherwise, false.</returns>
        private bool IsCarOnStop(Vehicle v)
        {
            int x = v.MapX;
            int y = v.MapY;
            if (0 > x || x >= Map.Width || 0 > y || y >= Map.Height) return false;
            Field currentField = Map[x, y];
            if (currentField is Stop)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Timer event handlers
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (IsGameOver)
            {
                OnGameOver();
                return;
            }
            GameTime++;
            //TODO: AllVehiclesStep() comes here
            AllVehiclesDoTheTransport();
            if (GameTime > 0 && GameTime % 10 == 0)
            {
                var grownTrees = ForestGrowing();
                GameAdvanced?.Invoke(this, grownTrees);
            }
            GameTicked?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
