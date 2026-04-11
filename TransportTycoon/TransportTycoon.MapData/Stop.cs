using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    public class Stop : Infrastructure
    {
        #region Fields
        public List<BuildingBlocks>? Connenctions { get; private set; }
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
        public void SetBuildingBlocks(BuildingBlocks buildingBlock)
        {
            Connenctions?.Add(buildingBlock);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
