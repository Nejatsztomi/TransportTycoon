using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Persistence
{
    public class GameSaveData
    {
        public MapGenerationContext MapContext { get; set; }
        public ulong InGameTime { get; set; }
        public int PlayerBalance { get; set; }

        public List<TileSaveData> ModifiedTiles { get; set; } = [];
        public List<TreeSaveData> ModifiedTrees { get; set; } = [];

        public List<VehicleSaveData> Vehicles { get; set; } = [];
        public List<BuildingEntitySaveData> BuildingEntities { get; set; } = [];
    }

    public readonly record struct TileSaveData(
        int X,
        int Y,
        FieldType Type
        );

    public readonly record struct TreeSaveData(
        int X,
        int Y,
        int Amount
        );

    public readonly record struct VehicleSaveData(
        VehicleType Type,
        double CurrentX,
        double CurrentY,
        LoadType CurrentLoad,
        int CurrentCapacity
        );

    public readonly record struct BuildingEntitySaveData(
        int TopLeftX,
        int TopLeftY,
        int CurrentCapacity,
        int Productivity
        );

    // Enums to make JSON readable
    public enum VehicleType
    {
        Van = 0, Pickup = 1, Truck = 2, LiquidTruck = 3, SmallBus = 4, BigBus = 5
    }
}
