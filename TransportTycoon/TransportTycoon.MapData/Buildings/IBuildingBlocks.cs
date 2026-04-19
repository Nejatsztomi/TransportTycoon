namespace TransportTycoon.MapData.Buildings
{
    public interface IBuildingBlocks : IField
    {
        #region Properties
        public BuildingEntity BuildingEntity { get; protected set; }
        #endregion
    }

    public struct House : IBuildingBlocks
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.House;
        #endregion

        #region Constructors
        public House(int x, int y, CityEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
