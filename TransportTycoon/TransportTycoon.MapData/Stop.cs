namespace TransportTycoon.MapData
{
    public class Stop : Infrastructure
    {
        #region Fields
        public List<BuildingBlocks>? Goods { get; private set; }
        #endregion

        #region Constructors
        public Stop() { }
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
