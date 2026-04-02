namespace TransportTycoon.MapData.Buildings
{
    public abstract class Site : BuildingBlocks
    {
        protected Site(int x, int y, SiteEntity buildingEntity) : base(x, y, buildingEntity) { }
    }

    public class LumberCamp : Site
    {
        #region Constructor
        public LumberCamp(int x, int y, LumberCampEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.LumberCamp;
        }
        #endregion
    }

    public class Mine : Site
    {
        #region Constructor
        public Mine(int x, int y, MineEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.Mine;
        }
        #endregion
    }

    public class Farm : Site
    {
        #region Constructor
        public Farm(int x, int y, FarmEntity buildingEntity) : base(x, y, buildingEntity)
        {
            FieldType = FieldType.Farm;
        }
        #endregion
    }
}
