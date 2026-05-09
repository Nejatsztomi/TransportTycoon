using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Persistence
{
    /// <summary>
    /// Represents the data structure used for saving the state of a game in Transport Tycoon.
    /// This class encapsulates all the necessary information to restore a game session, including the map context, game time, player balance, difficulty level, modified tiles and trees, as well as the current state of vehicles and building entities. It is designed to be serialized and deserialized when saving and loading game data, ensuring that players can continue their progress seamlessly.
    /// </summary>
    public class GameSaveData
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the context data used for map generation.
        /// </summary>
        public MapGenerationContextData MapContextData { get; set; }

        /// <summary>
        /// Gets or sets the current game time in ticks.
        /// </summary>
        public ulong GameTime { get; set; }

        /// <summary>
        /// Gets or sets the current balance of the player.
        /// </summary>
        public int PlayerBalance { get; set; }

        /// <summary>
        /// Gets or sets the difficulty level for the operation.
        /// </summary>
        public Persistence.Difficulty Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the collection of tiles that have been modified.
        /// </summary>
        public List<TileSaveData> ModifiedTiles { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of trees that have been modified.
        /// </summary>
        public List<TreeSaveData> ModifiedTrees { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of vehicles associated with this save data.
        /// </summary>
        public List<VehicleSaveData> Vehicles { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of building entities to be saved or loaded.
        /// </summary>
        public List<BuildingEntitySaveData> BuildingEntities { get; set; } = [];
        #endregion
    }

    #region DTOs
    /// <summary>
    /// Represents the data required to save a single tile, including its position, type, and height.
    /// </summary>
    /// <param name="X">The horizontal coordinate of the tile.</param>
    /// <param name="Y">The vertical coordinate of the tile.</param>
    /// <param name="Type">The type of the tile as defined by the save field type.</param>
    /// <param name="Height">The height value associated with the tile.</param>
    public readonly record struct TileSaveData(
        int X,
        int Y,
        Persistence.SaveFieldType Type,
        int Height
        );

    /// <summary>
    /// Represents the data required to save the state of a tree, including its position and amount.
    /// </summary>
    /// <param name="X">The X-coordinate of the tree's position.</param>
    /// <param name="Y">The Y-coordinate of the tree's position.</param>
    /// <param name="Amount">The amount associated with the tree. The meaning of this value depends on the application's context.</param>
    public readonly record struct TreeSaveData(
        int X,
        int Y,
        int Amount
        );

    /// <summary>
    /// Represents the data required to persist the state of a vehicle, including its type, position, load, capacity,
    /// orientation, and associated prouth information.
    /// </summary>
    /// <param name="Type">The type of the vehicle to be saved.</param>
    /// <param name="CurrentX">The current X-coordinate of the vehicle's position.</param>
    /// <param name="CurrentY">The current Y-coordinate of the vehicle's position.</param>
    /// <param name="CurrentLoad">The current load carried by the vehicle.</param>
    /// <param name="CurrentCapacity">The maximum capacity of the vehicle.</param>
    /// <param name="Angle">The orientation angle of the vehicle, in degrees.</param>
    /// <param name="Prouth">The prouth data associated with the vehicle.</param>
    public readonly record struct VehicleSaveData(
        Persistence.VehicleType Type,
        int CurrentX,
        int CurrentY,
        Persistence.LoadType CurrentLoad,
        int CurrentCapacity,
        double Angle,
        ProuthData Prouth
        );

    /// <summary>
    /// Represents the data required to save the state of a building entity, including its position and current
    /// capacity.
    /// </summary>
    /// <param name="TopLeftX">The X-coordinate of the building's top-left corner in the grid.</param>
    /// <param name="TopLeftY">The Y-coordinate of the building's top-left corner in the grid.</param>
    /// <param name="CurrentCapacity">The current capacity value associated with the building entity.</param>
    public readonly record struct BuildingEntitySaveData(
        int TopLeftX,
        int TopLeftY,
        int CurrentCapacity
        );

    /// <summary>
    /// Represents a collection of stops as a value type for efficient transport or processing.
    /// </summary>
    /// <param name="Stops">The list of stops represented as Coordinate objects. Cannot be null.</param>
    public readonly record struct ProuthData(
        List<Coordinate> Stops
        );

    /// <summary>
    /// Represents a two-dimensional point with integer X and Y coordinates.
    /// </summary>
    /// <param name="X">The horizontal position of the coordinate.</param>
    /// <param name="Y">The vertical position of the coordinate.</param>
    public readonly record struct Coordinate(
        int X,
        int Y
        );
    #endregion

    #region Enums
    /// <summary>
    /// Specifies the types of vehicles available for selection.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the category of a vehicle, such as van, pickup, truck, or
    /// bus. The values can be used for filtering, categorization, or logic that depends on vehicle type.</remarks>
    public enum VehicleType : byte
    {
        Van = 0,
        Pickup = 1,
        Truck = 2,
        LiquidTruck = 3,
        SmallBus = 4,
        BigBus = 5
    }

    /// <summary>
    /// Specifies the type of field that can be saved in the map data, such as terrain, road, stop, or various bridge
    /// types.
    /// </summary>
    /// <remarks>Use this enumeration to identify and differentiate between different field types when
    /// processing or serializing map data. Each value represents a distinct field type that may have unique behavior or
    /// rendering in the application.</remarks>
    public enum SaveFieldType : byte
    {
        Terrain,
        Road,
        Stop,
        HorizontalGreenBridge,
        VerticalGreenBridge,
        HorizontalYellowBridge,
        VerticalYellowBridge,
        HorizontalRedBridge,
        VerticalRedBridge,
    }

    /// <summary>
    /// Specifies the types of building entities available in the system.
    /// </summary>
    /// <remarks>Use this enumeration to identify or categorize buildings such as houses, farms, mines, and
    /// various production facilities. The values are stable and suitable for serialization or storage.</remarks>
    public enum BuildingEntityType : byte
    {
        House = 0,
        Farm = 1,
        Mine = 2,
        LumberCamp = 3,
        Mill = 4,
        Factory = 5,
        Plant = 6,
    }

    /// <summary>
    /// Specifies the types of loads that can be transported or processed within the system.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the kind of cargo or material involved in operations such as
    /// shipping, storage, or processing. The values represent distinct categories, including agricultural products, raw
    /// materials, manufactured goods, and people. Additional values may be added in future versions to support new load
    /// types.</remarks>
    public enum LoadType : byte
    {
        None,
        Wheat,
        Oil,
        Wood,
        Flour,
        Rubber,
        Paper,
        People,
    }

    /// <summary>
    /// Specifies the available difficulty levels.
    /// </summary>
    /// <remarks>Use this enumeration to indicate or select the difficulty setting for an operation, game, or
    /// feature. The values represent increasing levels of challenge.</remarks>
    public enum Difficulty : byte
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
    }
    #endregion
}
