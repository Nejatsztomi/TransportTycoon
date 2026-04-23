using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Persistence
{
    public class GameSaveData
    {
        public MapGenerationContextData MapContextData { get; set; }
        public ulong GameTime { get; set; }
        public int PlayerBalance { get; set; }

        public List<TileSaveData> ModifiedTiles { get; set; } = [];
        public List<TreeSaveData> ModifiedTrees { get; set; } = [];

        public List<VehicleSaveData> Vehicles { get; set; } = [];
        public List<BuildingEntitySaveData> BuildingEntities { get; set; } = [];
    }

    public readonly record struct TileSaveData(
        int X,
        int Y,
        Persistence.SaveFieldType Type
        );

    public readonly record struct TreeSaveData(
        int X,
        int Y,
        int Amount
        );

    public readonly record struct VehicleSaveData(
        Persistence.VehicleType Type,
        int CurrentX,
        int CurrentY,
        Persistence.LoadType CurrentLoad,
        int CurrentCapacity
        );

    public readonly record struct BuildingEntitySaveData(
        int TopLeftX,
        int TopLeftY,
        int CurrentCapacity,
        int Productivity
        );

    public enum VehicleType : byte
    {
        Van = 0,
        Pickup = 1,
        Truck = 2,
        LiquidTruck = 3,
        SmallBus = 4,
        BigBus = 5,
    }

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
}
