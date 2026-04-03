namespace TransportTycoon.MapData.Buildings
{
    public abstract class BuildingBlocks : Field
    {
        #region Properties
        public BuildingEntity BuildingEntity { get; }
        #endregion

        #region Constructors
        protected BuildingBlocks(int x, int y, BuildingEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            Modifiable = false;

            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    public class House : BuildingBlocks
    {
        #region Constructors
        public House(int x, int y, CityEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.House;
        }
        #endregion
    }
}
