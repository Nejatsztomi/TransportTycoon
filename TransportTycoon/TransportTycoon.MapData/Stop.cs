using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public class Stop : Infrastructure
    {
        #region Fields
        public List<BuildingBlocks>? Goods { get; private set; }
        #endregion
        #region Public Methods
        public Stop()
        {
            //...
        }
        public List<Load> ShowLoads()
        {
            return null;//...
        }
        public bool VehicleToBuilding()
        {
            return false;//...
        }
        public bool BuildingToVehicle()
        {
            return false;//...
        }
        #endregion
        #region Private Methods
        private void SetBuildingBlocks()
        {
            //...
        }
        #endregion
    }
}
