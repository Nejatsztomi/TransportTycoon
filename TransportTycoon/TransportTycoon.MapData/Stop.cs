using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    public struct Stop : IInfrastructure
    {
        #region Fields
        public List<IBuildingBlocks>? Goods { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly int Price => 200;
        public readonly FieldType FieldType => FieldType.Stop;
        #endregion

        #region Constructors
        public Stop(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }
        #endregion

        #region Public Methods
        public readonly List<Load> ShowLoads()
        {
            return [];
        }

        public readonly bool VehicleToBuilding()
        {
            return false;
        }

        public readonly bool BuildingToVehicle()
        {
            return false;
        }
        #endregion

        #region Private Methods
        private void SetBuildingBlocks() { }
        #endregion
    }
}
