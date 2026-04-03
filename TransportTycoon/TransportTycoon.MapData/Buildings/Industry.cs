namespace TransportTycoon.MapData.Buildings
{
    public abstract class Industry : BuildingBlocks
    {
        #region Constructors
        protected Industry(int x, int y, IndustryEntity buildingEntity) : base(x, y, buildingEntity) { }
        #endregion
    }

    public class Mill : Industry
    {
        #region Constructors
        public Mill(int x, int y, MillEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.Mill;
        }
        #endregion
    }

    public class Plant : Industry
    {
        #region Constructors
        public Plant(int x, int y, PlantEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.Plant;
        }
        #endregion
    }

    public class Factory : Industry
    {
        #region Constructors
        public Factory(int x, int y, FactoryEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.Factory;
        }
        #endregion
    }
}
