namespace TransportTycoon.MapData.Buildings
{
    public interface ISite : IBuildingBlocks { }

    public struct LumberCamp : ISite
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.LumberCamp;
        #endregion

        #region Constructor
        public LumberCamp(int x, int y, LumberCampEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    public struct Mine : ISite
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.Mine;
        #endregion

        #region Constructor
        public Mine(int x, int y, MineEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    public struct Farm : ISite
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BuildingEntity BuildingEntity { get; set; }
        public readonly FieldType FieldType => FieldType.Farm;
        #endregion

        #region Constructor
        public Farm(int x, int y, FarmEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
