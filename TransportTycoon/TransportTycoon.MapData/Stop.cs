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
        /// <summary>
        /// Returns the loads that the connected buildings can provide, so the vehicle can check if it can pick up something from this stop
        /// </summary>
        /// <returns></returns>
        public List<LoadType> ShowLoads()
        {
            if (Connenctions is null) return new List<LoadType>();
            List<LoadType> possibleLoads = new List<LoadType>();
            foreach (var building in Connenctions)
            {
                possibleLoads.Add(building.BuildingEntity.GetProvideLoad());
            }
            return possibleLoads;
        }
        /// <summary>
        /// Retrieves a list of load types that represent the consumption needs of all connected buildings.
        /// </summary>
        public List<LoadType> ShowNeeds()
        {
            if (Connenctions is null) return new List<LoadType>();
            List<LoadType> possibleNeeds = new List<LoadType>();
            foreach (var building in Connenctions)
            {
                possibleNeeds.Add(building.BuildingEntity.GetConsumeLoad());
            }
            return possibleNeeds;
        }
        //returns the leftover capacity of the vehicle after it tries to deliver the load to the connected buildings,
        //if it can deliver at least some of it, otherwise returns the original capacity
        public int VehicleToBuilding(LoadType? load, int vehicleCanGive)
        {
            //a jarmu megerkezik, leadja a termeket ha tudja,
            //akkor tudja leadni ha a jarmu olyan termeket szallit, mint amit a gyár/város befogad
            if (load is null) return 0; // if the vehicle is empty, it can deliver nothing, so the leftover capacity is 0
            //if the stop has no connections, it has no needs, so the vehicle can deliver nothing, so the leftover capacity is 0
            if (ShowNeeds().Count == 0) return vehicleCanGive;
            if (Connenctions is null) return vehicleCanGive;

            List<BuildingBlocks> buildings = new List<BuildingBlocks>();
            foreach (var building in Connenctions)
            {
                if (building.BuildingEntity.GetConsumeLoad() == load)
                {
                    buildings.Add(building);
                }
            }

            foreach(var building in buildings)
            {
                int buildingCanTake = building.BuildingEntity.Capacity;
                if (buildingCanTake >= vehicleCanGive)
                {
                    int buildingLeftOver = buildingCanTake - vehicleCanGive;
                    building.BuildingEntity.SetCapacity(buildingLeftOver);
                    return 0;
                }
                else
                {
                    vehicleCanGive = vehicleCanGive - buildingCanTake;
                    building.BuildingEntity.SetCapacity(0);
                }
            }
            return vehicleCanGive;
        }

        public bool BuildingToVehicle()
        {
            return false;
            //a jarmu megerkezik, felveszi a termeket ha tudja,
            //akkor tudja felvenni, ha a jarmu olyan termeket fogad, mint amit gyár/város ad, vagy nem szállít semmit se
            //akkor nem, ha a jármű tele van
            //tele lehet?
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
