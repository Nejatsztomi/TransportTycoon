using System.Diagnostics;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.Model.Graph;
using TransportTycoon.Persistence;
using LoadType = TransportTycoon.MapData.LoadType;
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

    /// <summary>
    /// The <see cref="GameModel"/> class serves as the central component of the game's architecture, encapsulating the core game logic, state management, and interactions between various game entities.
    /// It maintains the current state of the game world, including the map, vehicles, player balance, and game time, while also providing methods for modifying the game state in response to player actions and game events.
    /// The class is designed to be flexible and extensible, allowing for easy integration of new features and mechanics as the game evolves.
    /// </summary>
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
        public const int DefaultBalance = 10_000;

        /// <summary>
        /// Default starting difficulty for new game.
        /// </summary>
        public const Difficulty DefaultDifficulty = Difficulty.Medium;
        #endregion

        #region Private fields
        private readonly ITimer _timer;
        private readonly Dictionary<(int X, int Y), Field> _modifiedFields = [];
        private double _timeAccumulator = 0.0;
        private IPathFinder _pathFinder;
        private readonly Vehicle?[,,] _tileOccupancy;
        #endregion

        #region Public properties
        #region Game state related data
        /// <summary>
        /// Gets the current game table representing the map layout and state.
        /// </summary>
        public GameTable Map { get; private set; }

        /// <summary>
        /// Gets the list representing the current game's vehicles.
        /// </summary>
        public List<Vehicle> Vehicles { get; private set; } = [];

        /// <summary>
        /// Gets the player's current balance.
        /// </summary>
        public int Balance { get; private set; }

        /// <summary>
        /// Gets the name associated with the saved data.
        /// </summary>
        public string SaveName { get; }

        /// <summary>
        /// Gets the game current time (ticks passed after first creating a new game).
        /// </summary>
        public ulong GameTime { get; private set; }

        /// <summary>
        /// Gets the game current difficulty level.
        /// </summary>
        public Difficulty Difficulty { get; private set; }
        #endregion

        #region Game logic
        /// <summary>
        /// Gets or sets the mode of the game.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the time speed of the game.
        /// </summary>
        public TimeSpeed TimeSpeed
        {
            get;
            set
            {
                TimeSpeedChanged?.Invoke(this, value);
                field = value;
            }
        }

        /// <summary>
        /// The game's graph representation of the map.
        /// </summary>
        public Graph.Graph GraphNetwork { get; private set; }
        public int Maintenance { get; private set; }
        #endregion

        public Field? SelectedField { get; private set; }
        public List<Stop> SelectedStopFields { get; private set; } = [];
        public bool IsGameOver => Balance <= 0;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a new game is created.
        /// </summary>
        public event EventHandler? NewGameCreated;

        /// <summary>
        /// Occurs when the game mode is changed.
        /// </summary>
        public event EventHandler<GameMode>? GameModeChanged;

        /// <summary>
        /// Occurs when the time speed value changes.
        /// </summary>
        public event EventHandler<TimeSpeed>? TimeSpeedChanged;
        public event EventHandler<TransportTycoonEventArgs>? GameOver;
        public event EventHandler<TransportTycoonFieldEventArgs>? FieldChanged;
        public event EventHandler? BalanceChanged;

        /// <summary>
        /// Occurs when a game tick happens.
        /// </summary>
        public event EventHandler? GameTicked;
        public event EventHandler<List<Tuple<int, int>>>? GameAdvanced;
        public event EventHandler<List<(int, int)>>? InfrastructureBuilt;
        public event EventHandler<(int, int)>? SelectedFieldChanged;
        public event EventHandler<Vehicle>? VehicleChanged;
        public event EventHandler<List<Stop>>? SelectedStopFieldsChanged;
        public event EventHandler<List<(int, int)>>? ProductionChanged;
        public event EventHandler<(int X, int Y, int Value)>? BalanceMessage;
        public event EventHandler? MaintenanceChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the GameModel class with the specified game map, timer, and creation data.
        /// </summary>
        /// <param name="map">The GameTable representing the map layout and state for the game.</param>
        /// <param name="timer">The timer used to control game time progression and periodic updates.</param>
        /// <param name="data">The GameCreationData containing initial configuration such as difficulty, balance, and save name.</param>
        public GameModel(GameTable map, ITimer timer, GameCreationData data)
        {
            Difficulty = data.Difficulty;
            Balance = data.Balance;
            SaveName = data.SaveName;

            Map = map;
            _timer = timer;
            _timer.Tick += Timer_Tick;

            SetTax();
            Mode = GameMode.Run;
            TimeSpeed = TimeSpeed.Normal;
            GameTime = 0;

            // We create an empty graph
            GraphNetwork = new([], []);
            _pathFinder = new AStarPathfinder(GraphNetwork);

            _tileOccupancy = new Vehicle?[Map.Width, Map.Height, 4];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModel"/> class using the specified map, timer, save data, and save name.
        /// Restores the game state from the provided save data, including map tiles, vehicles, and building entities.
        /// </summary>
        /// <remarks>
        /// This constructor fully restores the game state from the provided save data, including map modifications, vehicles, and building entities.
        /// It also sets up the timer and pathfinding network required for gameplay.
        /// </remarks>
        /// <param name="map">The <see cref="GameTable"/> representing the game map to be used for this game session.</param>
        /// <param name="timer">The timer instance used to manage game time progression and periodic updates.</param>
        /// <param name="data">The <see cref="GameSaveData"/> containing all persisted information required to restore the game state, including tiles, vehicles, and buildings.</param>
        /// <param name="saveName">The name of the save file or save slot associated with this game session. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the save data contains an invalid vehicle type or load type.</exception>
        /// <exception cref="Exception">Thrown if a building entity referenced in the save data cannot be found in the map.</exception>
        public GameModel(GameTable map, ITimer timer, GameSaveData data, string saveName)
        {
            Difficulty = (Difficulty)data.Difficulty;
            Balance = data.PlayerBalance;
            SaveName = saveName;
            GameTime = data.GameTime;

            Map = map;
            _timer = timer;
            _timer.Tick += Timer_Tick;

            SetTax();

            GraphNetwork = new([], []);
            _pathFinder = new AStarPathfinder(GraphNetwork);

            _tileOccupancy = new Vehicle?[Map.Width, Map.Height, 4];

            Map.Context = new(data.MapContextData);
            Map.GenerateMap();

            LoadGameFromData(data);
        }
        #endregion

        #region Public methods
        #region Persistence
        /// <summary>
        /// Creates a snapshot of the current game state for persistence or serialization.
        /// </summary>
        /// <remarks>Use this method to obtain a complete representation of the game's current state,
        /// suitable for saving or transferring between sessions. The returned data reflects all modifications made
        /// since the last load or save.</remarks>
        /// <returns>A GameSaveData object containing the current map context, game time, player balance, difficulty, and all
        /// modified tiles, trees, vehicles, and building entities.</returns>
        /// <exception cref="Exception">Thrown if an invalid field or vehicle load type is encountered during the save data generation.</exception>
        public GameSaveData GetGameSaveData()
        {
            List<TileSaveData> tileSaveDatas = [.. _modifiedFields.Select(kv => new TileSaveData()
            {
                X = kv.Key.X,
                Y = kv.Key.Y,
                Type = kv.Value switch
                {
                    Terrain => SaveFieldType.Terrain,
                    Road => SaveFieldType.Road,
                    Stop => SaveFieldType.Stop,
                    YellowBridge yellowBridge when yellowBridge.BridgeType == BridgeType.HorizontalYellowBridge => SaveFieldType.HorizontalYellowBridge,
                    YellowBridge yellowBridge when yellowBridge.BridgeType == BridgeType.VerticalYellowBridge => SaveFieldType.VerticalYellowBridge,
                    RedBridge redBridge when redBridge.BridgeType == BridgeType.HorizontalRedBridge => SaveFieldType.HorizontalRedBridge,
                    RedBridge redBridge when redBridge.BridgeType == BridgeType.VerticalRedBridge => SaveFieldType.VerticalRedBridge,
                    GreenBridge greenBridge when greenBridge.BridgeType == BridgeType.HorizontalGreenBridge => SaveFieldType.HorizontalGreenBridge,
                    GreenBridge greenBridge when greenBridge.BridgeType == BridgeType.VerticalGreenBridge => SaveFieldType.VerticalGreenBridge,
                    _ => throw new Exception($"Invalid field type at ({kv.Key.X}, {kv.Key.Y})")
                },
                Height = kv.Value.Height
            })];

            List<TreeSaveData> treesData = [.. Map.Table.Cast<Field>()
                .Where(field => field.GetTrees() > 0)
                .Select(field => new TreeSaveData()
                {
                    X = field.X,
                    Y = field.Y,
                    Amount = field.GetTrees()
                }
                )];

            List<VehicleSaveData> vehiclesData = [.. Vehicles.Select(v => new VehicleSaveData()
            {
                Type = (Persistence.VehicleType)v.Type,
                CurrentX = (int)v.X,
                CurrentY = (int)v.Y,
                CurrentLoad = v.CurrentLoad?.LoadType switch {
                    MapData.LoadType.Wheat => Persistence.LoadType.Wheat,
                    MapData.LoadType.Oil => Persistence.LoadType.Oil,
                    MapData.LoadType.Wood => Persistence.LoadType.Wood,
                    MapData.LoadType.Flour => Persistence.LoadType.Flour,
                    MapData.LoadType.Rubber => Persistence.LoadType.Rubber,
                    MapData.LoadType.Paper => Persistence.LoadType.Paper,
                    MapData.LoadType.People => Persistence.LoadType.People,
                    MapData.LoadType.None => Persistence.LoadType.None,

                    null => Persistence.LoadType.None,

                    _ => throw new Exception($"Invalid load type for vehicle at ({v.X}, {v.Y})")
                },
                CurrentCapacity = v.CurrentCapacity,
                Angle = v.Angle,
                Prouth = new(v.Prouth?.Stops.Select(stop => new Coordinate(stop.X, stop.Y)).ToList() ?? [])
            })];

            List<BuildingEntitySaveData> buildingsData = [.. Map.BuildingEntities
                .Select(entity => new BuildingEntitySaveData()
                {
                    TopLeftX = entity.TopLeftPoints.X,
                    TopLeftY = entity.TopLeftPoints.Y,
                    CurrentCapacity = (int)entity.CurrentCapacity,
                }
                )];

            return new()
            {
                MapContextData = new(Map.Context),
                GameTime = GameTime,
                PlayerBalance = Balance,
                Difficulty = (Persistence.Difficulty)Difficulty,

                ModifiedTiles = tileSaveDatas,
                ModifiedTrees = treesData,
                Vehicles = vehiclesData,
                BuildingEntities = buildingsData
            };
        }
        #endregion

        /// <summary>
        /// Retrieves the first vehicle located at the specified map coordinates.
        /// </summary>
        /// <remarks>If multiple vehicles occupy the same coordinates, only the first one found is
        /// returned. Coordinates are compared for exact equality.</remarks>
        /// <param name="x">The x-coordinate of the map position to search for a vehicle.</param>
        /// <param name="y">The y-coordinate of the map position to search for a vehicle.</param>
        /// <returns>A <see cref="Vehicle"/> object if a vehicle exists at the specified coordinates; otherwise, <see
        /// langword="null"/>.</returns>
        public Vehicle? GetVehicleAt(int x, int y)
        {
            return Vehicles.FirstOrDefault(v => v.MapX == x && v.MapY == y);
        }

        /// <summary>
        /// Starts a new game session: clears vehicles and building entities, generates a fresh map, starts the timer and raises the <see cref="NewGameCreated"/> event.
        /// </summary>
        public void NewGame()
        {
            Vehicles.Clear();
            Map.BuildingEntities.Clear();

            Map.GenerateMap();
            _timer.Start();
            NewGameCreated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the currently selected field on the map based on the specified coordinates.
        /// </summary>
        /// <remarks>This method raises the SelectedFieldChanged event after updating the selected field.
        /// Passing -1 for both x and y will deselect any currently selected field.</remarks>
        /// <param name="x">The zero-based x-coordinate of the field to select. If both x and y are -1, the selected field is cleared.</param>
        /// <param name="y">The zero-based y-coordinate of the field to select. If both x and y are -1, the selected field is cleared.</param>
        public void SetSelectedField(int x, int y)
        {
            if (x == -1 && y == -1) SelectedField = null;
            else SelectedField = Map[x, y];
            SelectedFieldChanged?.Invoke(this, (x, y));
        }

        /// <summary>
        /// Increases the height of the tile, if its Terrain
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void IncreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height + 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight))
                    {
                        if (field.Height == 4) return;
                        int cost;
                        if (terrain.Trees > 0)
                        {
                            cost = -50;
                            Balance += cost;
                        }
                        cost = -Terrain.Price;
                        Balance += cost;
                        terrain.IncreaseHeight();

                        Map.UpdateTable(x, y, terrain);

                        // Add the modified field to the dictionary
                        _modifiedFields[(x, y)] = terrain;

                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);
                        BalanceMessage?.Invoke(this, (x, y, cost));
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decreases the height of the tile, if its Terrain
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DecreaseHeight(int x, int y)
        {
            if (Mode == GameMode.Editor)
            {
                Field field = Map[x, y];

                if (field is Terrain terrain)
                {
                    int nextHeight = terrain.Height - 1;

                    if (Map.IsTileHeightPossible(x, y, nextHeight))
                    {
                        if (field.Height == 1) return;
                        int cost;
                        if (terrain.Trees > 0)
                        {
                            cost = -50;
                            Balance += cost;
                        }
                        cost = -Terrain.Price;
                        Balance += cost;
                        terrain.DecreaseHeight();

                        Map.UpdateTable(x, y, terrain);

                        // Add the modified field to the dictionary
                        _modifiedFields[(x, y)] = terrain;


                        FieldChanged?.Invoke(this, new TransportTycoonFieldEventArgs(x, y));
                        BalanceChanged?.Invoke(this, EventArgs.Empty);
                        BalanceMessage?.Invoke(this, (x, y, cost));
                        if (IsGameOver)
                        {
                            OnGameOver();
                            return;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Builds a road at the specified map coordinates if the game is in editor mode and the terrain at the location
        /// allows road construction.
        /// </summary>
        /// <remarks>This method only builds a road if the current game mode is set to editor and the
        /// specified location is a valid terrain with a height of 3 or less. The method updates the map, adjusts the
        /// balance based on the number of trees present, and triggers related events such as infrastructure updates and
        /// balance changes. If the game ends as a result of this action, the game over event is raised.</remarks>
        /// <param name="x">The x-coordinate of the map location where the road is to be built. Must be within the bounds of the map.</param>
        /// <param name="y">The y-coordinate of the map location where the road is to be built. Must be within the bounds of the map.</param>
        public void BuildRoad(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;
            List<(int X, int Y)> changedFields = [];

            int oldTrees = Map[x, y].GetTrees();

            Road newRoad = new(x, y, Map.CalculateRoadType(x, y), Map[x, y].Height);
            Map.UpdateTable(x, y, newRoad);
            changedFields.Add((x, y));

            // Add the modified field to the dictionary
            _modifiedFields[(x, y)] = newRoad;

            int cost;
            if (oldTrees == 0)
            {
                cost = -Road.Price;
                Balance += cost;
            }
            else
            {
                cost = -Road.Price * 2;
                Balance += cost;
            }

            foreach (Field? e in Map.NeighboursOfRoadsAndStops(x, y))
            {
                if (e is not null && e is Road road)
                {
                    road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                    Map.UpdateTable(e.X, e.Y, road);
                    changedFields.Add((e.X, e.Y));
                }
            }

            if (IsGameOver) OnGameOver();
            InfrastructureBuilt?.Invoke(this, changedFields);
            BalanceChanged?.Invoke(this, EventArgs.Empty);
            BalanceMessage?.Invoke(this, (x, y, cost));
            RebuildGraph();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void BuildBridge(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Water)
            {
                SetSelectedField(-1, -1);
                return;
            }

            if (SelectedField is null)
            {
                SetSelectedField(x, y);
                return;
            }

            if (SelectedField.X != x && SelectedField.Y != y)
            {
                SetSelectedField(-1, -1);
                return;
            }

            List<(int X, int Y)> changedFields = [];
            int cost = 0;
            if (SelectedField.X == x && SelectedField.Y == y)
            {
                Balance -= Map.CreateShortBridge(x, y, ref changedFields);
            }
            else if (SelectedField.X == x)
            {
                if (Math.Min(SelectedField.Y, y) - 1 < 0 || Map[x, Math.Min(SelectedField.Y, y) - 1].Height != 1 ||
                    Math.Max(SelectedField.Y, y) + 1 >= Map.Height || Map[x, Math.Max(SelectedField.Y, y) + 1].Height != 1)
                {
                    SetSelectedField(-1, -1);
                    return;
                }
                int dif = Math.Abs(SelectedField.Y - y);
                BridgeType b_type = Map.CalculateBridgeType(dif, "vertical");
                if (b_type == BridgeType.Null)
                {
                    SetSelectedField(-1, -1);
                    return;
                }

                for (int i = Math.Min(SelectedField.Y, y) + 1; i < Math.Max(SelectedField.Y, y); i++)
                {
                    if (Map[x, i] is not Water)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                }
                cost = -Map.CreateVerticalBridge(x, Math.Min(SelectedField.Y, y), Math.Max(SelectedField.Y, y), b_type, ref changedFields);
                Balance += cost;
            }
            else if (SelectedField.Y == y)
            {
                if (Math.Min(SelectedField.X, x) - 1 < 0 || Map[Math.Min(SelectedField.X, x) - 1, y].Height != 1 ||
                    Math.Max(SelectedField.X, x) + 1 >= Map.Width || Map[Math.Max(SelectedField.X, x) + 1, y].Height != 1)
                {
                    SetSelectedField(-1, -1);
                    return;
                }

                int dif = Math.Abs(SelectedField.X - x);
                BridgeType b_type = Map.CalculateBridgeType(dif, "horizontal");
                if (b_type == BridgeType.Null)
                {
                    SetSelectedField(-1, -1);
                    return;
                }

                for (int i = Math.Min(SelectedField.X, x); i <= Math.Max(SelectedField.X, x); i++)
                {
                    if (Map[i, y] is not Water)
                    {
                        SetSelectedField(-1, -1);
                        return;
                    }
                }
                cost = -Map.CreateHorizontalBridge(y, Math.Min(SelectedField.X, x), Math.Max(SelectedField.X, x), b_type, ref changedFields);
                Balance += cost;
            }

            SetSelectedField(-1, -1);
            if (IsGameOver) OnGameOver();

            // Modify the changed fields in the dictionary
            foreach (var change in changedFields)
            {
                _modifiedFields[change] = Map[change.X, change.Y];
            }

            InfrastructureBuilt?.Invoke(this, changedFields);
            BalanceChanged?.Invoke(this, EventArgs.Empty);
            if (cost != 0) BalanceMessage?.Invoke(this, (x, y, cost));
            RebuildGraph();
        }
        /// <summary>
        /// Attempts to build a stop at the specified map coordinates, updating the game state and player balance if
        /// successful.
        /// </summary>
        /// <remarks>This method only performs the operation if the game is not in editor mode and the
        /// specified location is valid for building a stop. The player's balance is adjusted based on the number of
        /// trees present at the location. The method also triggers events related to infrastructure changes and balance
        /// updates.</remarks>
        /// <param name="x">The x-coordinate on the map where the stop is to be constructed. Must be within the bounds of the map.</param>
        /// <param name="y">The y-coordinate on the map where the stop is to be constructed. Must be within the bounds of the map.</param>
        public void BuildStop(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Terrain || Map[x, y].Height > 3) return;

            if (!Map.StopEnvironment(x, y)) return;

            List<(int, int)> changedFields = [(x, y)];

            Stop stop = (Stop)Map[x, y];
            // Add the modified field to the dictionary
            _modifiedFields[(x, y)] = Map[x, y];

            int oldTrees = Map[x, y].GetTrees();
            int cost;
            if (oldTrees == 0)
            {
                cost = -Stop.Price;
                Balance += cost;
            }
            else
            {
                cost = -Stop.Price * 2;
                Balance += cost;
            }

            foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
            {
                if (e is not null && e is Road road)
                {
                    road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                    Map.UpdateTable(e.X, e.Y, road);
                    changedFields.Add((e.X, e.Y));
                }
            }

            if (IsGameOver) OnGameOver();

            RebuildGraph();
            InfrastructureBuilt?.Invoke(this, changedFields);
            BalanceChanged?.Invoke(this, EventArgs.Empty);
            BalanceMessage?.Invoke(this, (x, y, cost));
        }

        /// <summary>
        /// Destroys infrastructure at the specified coordinates when in Editor mode and allowed.
        /// Replaces roads/stops with terrain or destroys bridge chains; updates modified fields and rebuilds graph.
        /// </summary>
        /// <param name="x">Target tile X coordinate.</param>
        /// <param name="y">Target tile Y coordinate.</param>
        public void Destroy(int x, int y)
        {
            if (Mode != GameMode.Editor || Map[x, y] is not Infrastructure || (Map[x, y] is Road r && r.InCity())
                || Vehicles.Any(v => v.MapX == x && v.MapY == y)) return;
            List<(int X, int Y)> changedFields = [];

            if (Map[x, y] is Road || Map[x, y] is Stop)
            {
                Map[x, y] = new Terrain(x, y, Map[x, y].Height);
                changedFields.Add((x, y));

                foreach (var e in Map.NeighboursOfRoadsAndStops(x, y))
                {
                    if (e is not null && e is Road road)
                    {
                        road.ChangeType(Map.CalculateRoadType(e.X, e.Y));
                        Map.UpdateTable(e.X, e.Y, road);
                        changedFields.Add((e.X, e.Y));
                    }
                }
            }
            else
            {
                if (CheckDestroyBridge(x, y)) Map.DestroyBridge(x, y, ref changedFields);
            }
            // Modify the changed fields in the dictionary
            foreach (var change in changedFields)
            {
                _modifiedFields[change] = Map[change.X, change.Y];
            }
            InfrastructureBuilt?.Invoke(this, changedFields);
            RebuildGraph();
        }

        /// <summary>
        /// Create a new Vehicle based on the given type and coordinates, and add it to the player's collection if they have enough balance. Returns the created Vehicle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Vehicle? BuyVehicle(int x, int y, VehicleType type)
        {
            if (Map[x, y] is not Stop) return null;

            Vehicle vehicle = type switch
            {
                VehicleType.Van => new Van(x, y, 0.0, null),
                VehicleType.Pickup => new Pickup(x, y, 0.0, null),
                VehicleType.Truck => new Truck(x, y, 0.0, null),
                VehicleType.LiquidTruck => new LiquidTruck(x, y, 0.0, null),
                VehicleType.SmallBus => new SmallBus(x, y, 0.0, null),
                VehicleType.BigBus => new BigBus(x, y, 0.0, null),
                _ => throw new ArgumentException("Invalid vehicle type", nameof(type)),
            };

            if (Balance >= vehicle.Price)
            {
                Balance -= vehicle.Price;
                Vehicles.Add(vehicle);
                Maintenance += vehicle.Maintenance;
                BalanceChanged?.Invoke(this, EventArgs.Empty);
                BalanceMessage?.Invoke(this, (x, y, -vehicle.Price));
                MaintenanceChanged?.Invoke(this, EventArgs.Empty);
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
        /// <remarks>
        /// This method iterates through the collection of vehicles and updates each one invoking the step operation.
        /// No action is taken if the game mode is not set to run.</remarks>
        public void StepAllVehicles(double deltaTime)
        {
            if (Mode != GameMode.Run) return;

            foreach (var vehicle in Vehicles)
            {
                if (vehicle.IsLost) continue;

                int currentLaneIdx = vehicle.GetLaneIdx();

                if (vehicle.MapX != vehicle.LastMapX
                    || vehicle.MapY != vehicle.LastMapY
                    || currentLaneIdx != vehicle.LastLaneIdx)
                {
                    if (vehicle.LastMapX >= 0 && vehicle.LastLaneIdx >= 0)
                    {
                        _tileOccupancy[vehicle.LastMapX, vehicle.LastMapY, vehicle.LastLaneIdx] = null;
                    }

                    if (0 <= vehicle.MapX && vehicle.MapX < Map.Width
                        && 0 <= vehicle.MapY && vehicle.MapY < Map.Height)
                    {
                        _tileOccupancy[vehicle.MapX, vehicle.MapY, currentLaneIdx] = vehicle;
                    }

                    // Update the tracker
                    vehicle.LastMapX = vehicle.MapX;
                    vehicle.LastMapY = vehicle.MapY;
                    vehicle.LastLaneIdx = currentLaneIdx;
                }

                ApplyAntiCollision(vehicle);
                vehicle.Step(deltaTime);
            }
        }

        /// <summary>
        /// Adds a stop at the specified map coordinates to the current route if it is not already selected.
        /// </summary>
        /// <remarks>If the stop at the specified coordinates is not already part of the selected route,
        /// it is added and an event is raised to notify subscribers of the change.</remarks>
        /// <param name="x">The zero-based x-coordinate of the stop on the map. Must be within the bounds of the map array.</param>
        /// <param name="y">The zero-based y-coordinate of the stop on the map. Must be within the bounds of the map array.</param>
        public void DefineRoute(int x, int y)
        {
            if (Map[x, y] is not Stop stop) return;
            if (!SelectedStopFields.Contains(stop))
            {
                SelectedStopFields.Add(stop);
                SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
            }
        }

        /// <summary>
        /// Queries the route for a vehicle located at the specified coordinates and updates the selected stop fields
        /// accordingly.
        /// </summary>
        /// <remarks>If a vehicle is found at the given coordinates and it has an assigned route, the
        /// route is converted to stop tiles and the selected stop fields are updated. If no vehicle is found, the
        /// method exits without making changes. This method also raises the SelectedStopFieldsChanged event when the
        /// selected stop fields are updated.</remarks>
        /// <param name="x">The x-coordinate used to locate the vehicle on the map.</param>
        /// <param name="y">The y-coordinate used to locate the vehicle on the map.</param>
        public void QueryRoute(int x, int y)
        {
            Vehicle? selectedVehcile = Vehicles.Find(v => Math.Abs(v.X - x) < 0.0001 && Math.Abs(v.Y - y) < 0.0001);
            if (selectedVehcile is null) return;

            if (selectedVehcile.Prouth is not null)
            {
                SelectedStopFields = ProuthUtil.ConvertNodestoStopTiles(selectedVehcile.Prouth.Stops, Map);
            }

            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }

        /// <summary>
        /// Assigns a route to the vehicle located at the specified coordinates using the currently selected stops.
        /// </summary>
        /// <remarks>This method requires that at least one stop is selected before invocation. If no
        /// vehicle exists at the specified coordinates, the method performs no action. After assigning the route, the
        /// selected stops are cleared and an event is raised to notify subscribers of the change.</remarks>
        /// <param name="x">The X coordinate of the vehicle to which the route will be assigned.</param>
        /// <param name="y">The Y coordinate of the vehicle to which the route will be assigned.</param>
        public void AssignRoute(int x, int y)
        {
            if (SelectedStopFields.Count == 0) return;

            Vehicle? selectedVehicle = Vehicles.Find(v => Math.Abs(v.X - x) < 0.0001 && Math.Abs(v.Y - y) < 0.0001);
            if (selectedVehicle is null) return;
            Debug.WriteLine("Vehicle candiate found at X={0}, Y={1}", x, y);
            Debug.WriteLine("The select stop are located at:");
            foreach (var stop in SelectedStopFields)
            {
                Debug.WriteLine("Stop at X={0}, Y={1}", x, y);
            }

            Debug.WriteLine("Converting stop tiles to nodes then creating Prouth...");
            Prouth prouth = new(ProuthUtil.ConvertStopTilesToNodes(SelectedStopFields, GraphNetwork));
            Debug.WriteLine("Done!");

            Debug.WriteLine("Assigned prouth with {0} stops to the vehicle.", prouth.Stops.Count);

            var ghostNodeInjector = new GhostNodeInjector(GraphNetwork, new(Map));
            selectedVehicle.SetProuth(prouth, _pathFinder, ghostNodeInjector);

            Debug.WriteLine("Resetting stop list and inkoving event");
            SelectedStopFields = [];
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }

        /// <summary>
        /// delete the car's route
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DeleteRoute(int x, int y)
        {
            if (SelectedStopFields.Count == 0) return;

            if (x == -1 && y == -1) SelectedStopFields = [];
            else
            {
                Stop? removeItem = SelectedStopFields.Find(s => s.X == x && s.Y == y);
                if (removeItem is null) return;
                SelectedStopFields.Remove(removeItem);
            }
            SelectedStopFieldsChanged?.Invoke(this, SelectedStopFields);
        }
        #endregion

        #region Private Methods
        private void LoadGameFromData(GameSaveData data)
        {
            _modifiedFields.Clear();
            data.ModifiedTiles.ForEach(tile =>
            {
                int x = tile.X;
                int y = tile.Y;
                Map[x, y] = tile.Type switch
                {
                    SaveFieldType.Terrain => new Terrain(x, y, tile.Height),
                    SaveFieldType.Road => new Road(x, y, Map.CalculateRoadType(x, y), tile.Height),
                    SaveFieldType.Stop => new Stop(x, y, tile.Height),
                    SaveFieldType.HorizontalYellowBridge => new YellowBridge(x, y, BridgeType.HorizontalYellowBridge, 0),
                    SaveFieldType.VerticalYellowBridge => new YellowBridge(x, y, BridgeType.VerticalYellowBridge, 0),
                    SaveFieldType.HorizontalRedBridge => new RedBridge(x, y, BridgeType.HorizontalRedBridge, 0),
                    SaveFieldType.VerticalRedBridge => new RedBridge(x, y, BridgeType.VerticalRedBridge, 0),
                    SaveFieldType.HorizontalGreenBridge => new GreenBridge(x, y, BridgeType.HorizontalGreenBridge, 0),
                    SaveFieldType.VerticalGreenBridge => new GreenBridge(x, y, BridgeType.VerticalGreenBridge, 0),
                    _ => Map[x, y]
                };

                _modifiedFields.Add((x, y), Map[x, y]);
            });

            // Make sure roads have correct rotation
            data.ModifiedTiles
                .Where(tile => tile.Type == SaveFieldType.Road)
                .ToList()
                .ForEach(tile =>
                {
                    Map[tile.X, tile.Y] = new Road(tile.X, tile.Y, Map.CalculateRoadType(tile.X, tile.Y), Map[tile.X, tile.Y].Height);
                });

            data.ModifiedTiles
                .Where(tile => tile.Type == SaveFieldType.Stop)
                .ToList()
                .ForEach(stop =>
                {
                    Map.StopEnvironment(stop.X, stop.Y);
                });

            RebuildGraph();

            data.ModifiedTrees.ForEach(treeData =>
            {
                if (Map[treeData.X, treeData.Y] is Terrain terrain)
                {
                    terrain.Trees = treeData.Amount;
                }
            });

            data.Vehicles.ForEach(vehicleData =>
            {
                Vehicle vehicle = vehicleData.Type switch
                {
                    Persistence.VehicleType.Van => new Van(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    Persistence.VehicleType.Pickup => new Pickup(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    Persistence.VehicleType.Truck => new Truck(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    Persistence.VehicleType.LiquidTruck => new LiquidTruck(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    Persistence.VehicleType.SmallBus => new SmallBus(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    Persistence.VehicleType.BigBus => new BigBus(vehicleData.CurrentX, vehicleData.CurrentY, vehicleData.Angle, null),
                    _ => throw new ArgumentException("Invalid vehicle type in save data", nameof(vehicleData.Type)),
                };

                vehicle.SetCurrentCapacity(vehicleData.CurrentCapacity);

                Load? load = vehicleData.CurrentLoad switch
                {
                    Persistence.LoadType.Wheat => new Wheat(),
                    Persistence.LoadType.Oil => new Oil(),
                    Persistence.LoadType.Wood => new Wood(),
                    Persistence.LoadType.Flour => new Flour(),
                    Persistence.LoadType.Rubber => new Rubber(),
                    Persistence.LoadType.Paper => new Paper(),
                    Persistence.LoadType.People => new People(),
                    Persistence.LoadType.None => null,

                    _ => throw new ArgumentException("Invalid load type in save data", nameof(vehicleData.CurrentLoad)),
                };
                vehicle.SetCurrentLoad(load);

                var stops = vehicleData.Prouth.Stops
                .Select(stop => Map[stop.X, stop.Y])
                .Cast<Stop>()
                .ToList();

                if (stops.Any())
                {
                    var prouth = new Prouth(ProuthUtil.ConvertStopTilesToNodes(stops, GraphNetwork));
                    vehicle.SetProuth(prouth, _pathFinder, new(GraphNetwork, new(Map)));
                }

                Vehicles.Add(vehicle);
                _tileOccupancy[vehicle.MapX, vehicle.MapY, vehicle.GetLaneIdx()] = vehicle;
            });


            data.BuildingEntities.ForEach(buildingEntityData =>
            {
                BuildingEntity? buildingEntity = Map.BuildingEntities.
                FirstOrDefault(entity => entity.TopLeftPoints.X == buildingEntityData.TopLeftX && entity.TopLeftPoints.Y == buildingEntityData.TopLeftY)
                ?? throw new Exception($"Failed to find building entity at ({buildingEntityData.TopLeftX}, {buildingEntityData.TopLeftY}) in the map.");

                buildingEntity.CurrentCapacity = buildingEntityData.CurrentCapacity;
            });
        }

        private void ApplyAntiCollision(Vehicle vehicle)
        {
            double targetSpeed = vehicle.TopSpeed;

            Field currentField = Map[vehicle.MapX, vehicle.MapY];
            if (currentField is Bridge bridge)
            {
                targetSpeed = Math.Min(targetSpeed, bridge.SpeedLimit);
            }

            if (vehicle.GetNextTileCoordinates() is (int nextX, int nextY))
            {
                int nextLaneIdx = vehicle.GetLaneIdx();
                Vehicle? vehicleAhead = _tileOccupancy[nextX, nextY, nextLaneIdx];

                if (vehicleAhead is not null && vehicleAhead != vehicle)
                {
                    targetSpeed = Math.Min(targetSpeed, vehicleAhead.CurrentSpeed);
                }
            }
            vehicle.ChangeCurrentSpeed(targetSpeed);
        }

        /// <summary>
        /// A method that rebuilds the graph representation of the map.
        /// </summary>
        private void RebuildGraph()
        {
            Debug.WriteLine("Starting to rebuild the graph!");
            if (!Map.IsMapGenerated) return;
            GraphNetwork = GraphBuilder.BuildGraph(Map);
            _pathFinder = new AStarPathfinder(GraphNetwork);
            Debug.WriteLine("The graph has been successfully rebuilt!");

            ReasignVehiclesProuth();
        }

        private void ReasignVehiclesProuth()
        {
            Debug.WriteLine("Starting to reasign vehicle prouths!");
            var ghostNodeInjector = new GhostNodeInjector(GraphNetwork, new(Map));

            foreach (var vehicle in Vehicles)
            {
                if (vehicle.Prouth is not null && vehicle.Prouth.Stops.Count > 0)
                {
                    var stopFields = ProuthUtil.ConvertNodestoStopTiles(vehicle.Prouth.Stops, Map);
                    vehicle.Prouth = new(ProuthUtil.ConvertStopTilesToNodes(stopFields, GraphNetwork));

                    vehicle.PathFinder = _pathFinder;
                    vehicle.RecalculateRoute(ghostNodeInjector);
                }
            }
            Debug.WriteLine("Successfully reasigned vehicle prouths!");
        }

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

            Random rnd = new(Map.Context.Seed);
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
                                Map.UpdateTable(i, j, terrain);
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
                    Map.UpdateTable(terrain.X, terrain.Y, terrain);
                    grownTrees.Add(new(terrain.X, terrain.Y));
                }
            }

            return grownTrees;
        }

        private void AllProduction()
        {
            foreach (var item in Map.BuildingEntities)
            {
                item.Production();
            }
            ProductionChanged?.Invoke(this, [.. Map.BuildingEntities.Select(be => (be.TopLeftPoints))]);
        }
        #endregion

        #region Private event Methods
        private void OnGameOver()
        {
            _timer.Stop();
            Mode = GameMode.Paused;
            GameModeChanged?.Invoke(this, GameMode.Paused);
            GameOver?.Invoke(this, new TransportTycoonEventArgs(GameTime, Vehicles.Count, Maintenance));
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
                //  && vehicle.CurrentRoute == null && vehicle.Prouth != null
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
                        if (vehicle.CurrentCapacity > 0 && vehicle.CurrentLoad is not null)
                        {
                            int vehicleCanGive;
                            LoadType? vehicleLoad = vehicle.CurrentLoad?.LoadType;
                            foreach (var building in buildings_taker)
                            {
                                if (building.BuildingEntity is IndustryEntity industry && building.BuildingEntity is not CityEntity)
                                {
                                    if (vehicleLoad == building.BuildingEntity.GetConsumeLoad()?.LoadType)
                                    {
                                        vehicleCanGive = vehicle.CurrentCapacity;
                                        int buildingCanTake = (int)industry.MaxConsumeCapacity - (int)industry.ConsumeCapacity;
                                        if (buildingCanTake >= vehicleCanGive)
                                        {
                                            int buildingNewCapacity = (int)industry.ConsumeCapacity + vehicleCanGive;
                                            Balance += vehicleCanGive * vehicle.CurrentLoad!.Price;
                                            BalanceChanged?.Invoke(this, EventArgs.Empty);
                                            BalanceMessage?.Invoke(this, (vehicle.MapX, vehicle.MapY, vehicleCanGive * vehicle.CurrentLoad!.Price));
                                            industry.SetConsumeCapacity(buildingNewCapacity);
                                            vehicle.SetCurrentCapacity(0);
                                            vehicle.SetCurrentLoad(null);
                                            break;
                                        }
                                        else
                                        {
                                            vehicleCanGive -= buildingCanTake;
                                            Balance += buildingCanTake * vehicle.CurrentLoad!.Price;
                                            BalanceChanged?.Invoke(this, EventArgs.Empty);
                                            BalanceMessage?.Invoke(this, (vehicle.MapX, vehicle.MapY, buildingCanTake * vehicle.CurrentLoad!.Price));
                                            industry.SetConsumeCapacity(industry.MaxConsumeCapacity);
                                            vehicle.SetCurrentCapacity(vehicleCanGive);
                                        }
                                    }
                                }
                                else if (building.BuildingEntity is CityEntity city)
                                {
                                    vehicleCanGive = vehicle.CurrentCapacity;
                                    Balance += vehicleCanGive * vehicle.CurrentLoad!.Price;
                                    BalanceChanged?.Invoke(this, EventArgs.Empty);
                                    BalanceMessage?.Invoke(this, (vehicle.MapX, vehicle.MapY, vehicleCanGive * vehicle.CurrentLoad!.Price));
                                    vehicle.SetCurrentCapacity(0);
                                    vehicle.SetCurrentLoad(null);
                                    break;
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
                                    int buildingCanGive = (int)building.BuildingEntity.CurrentCapacity;
                                    if (buildingCanGive == 0) break;
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

            if (0 > x || x >= Map.Height || 0 > y || y >= Map.Width) return false;
            Field currentField = Map[x, y];

            if (currentField is Stop)
            {
                return true;
            }
            return false;
        }

        private bool CheckDestroyBridge(int x, int y)
        {
            int up = y - 1;
            while (Map[x, up] is Bridge)
            {
                if (Vehicles.Any(v => v.MapX == x && v.MapY == up)) return false;
                up--;
            }
            int down = y + 1;
            while (Map[x, down] is Bridge)
            {
                if (Vehicles.Any(v => v.MapX == x && v.MapY == down)) return false;
                down++;
            }
            int left = x - 1;
            while (Map[left, y] is Bridge)
            {
                if (Vehicles.Any(v => v.MapX == left && v.MapY == y)) return false;
                left--;
            }
            int right = x + 1;
            while (Map[right, y] is Bridge)
            {
                if (Vehicles.Any(v => v.MapX == right && v.MapY == y)) return false;
                right++;
            }
            return true;
        }
        #endregion

        #region Timer event handlers
        private void Timer_Tick(double deltaTime)
        {
            if (IsGameOver)
            {
                OnGameOver();
                return;
            }

            double scaledDeltaTime = deltaTime * (double)TimeSpeed;
            StepAllVehicles(scaledDeltaTime);
            _timeAccumulator += scaledDeltaTime;

            while (_timeAccumulator >= 1)
            {
                GameTime++;
                AllVehiclesDoTheTransport();
                AllProduction();
                if (GameTime > 0 && GameTime % 10 == 0)
                {
                    var grownTrees = ForestGrowing();
                    GameAdvanced?.Invoke(this, grownTrees);
                }

                foreach (var vehicle in Vehicles)
                {
                    Balance -= vehicle.Maintenance;
                    BalanceChanged?.Invoke(this, EventArgs.Empty);
                }

                GameTicked?.Invoke(this, EventArgs.Empty);
                _timeAccumulator -= 1;
            }
        }
        #endregion
    }
}
