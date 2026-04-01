namespace TransportTycoon.MapData
{
    public class Stop : Infrastructure
    {
        #region Fields
        public List<BuildingBlocks>? Goods { get; private set; }
        #endregion

        #region Constructors
        public Stop(int x, int y, int height)
        {
            X = x; Y = y; Height = height;
            FieldType = FieldType.Stop;
            Price = 200;
        }
        #endregion

        #region Public Methods
        public List<Load> ShowLoads()
        {
            return [];
        }

        public bool VehicleToBuilding()
        {
            return false;
        }

        public bool BuildingToVehicle()
        {
            return false;
        }
        #endregion

        #region Private Methods
        private void SetBuildingBlocks() { }
        #endregion
    }
}
