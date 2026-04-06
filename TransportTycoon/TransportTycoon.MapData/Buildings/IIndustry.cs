namespace TransportTycoon.MapData.Buildings
{
    public interface IIndustry : IBuildingBlocks { }

    public struct Mill : IIndustry
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.Mill;
        #endregion

        #region Constructors
        public Mill(int x, int y, MillEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    public struct Plant : IIndustry
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.Plant;
        #endregion

        #region Constructors
        public Plant(int x, int y, PlantEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    public struct Factory : IIndustry
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.Factory;
        #endregion

        #region Constructors
        public Factory(int x, int y, FactoryEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
