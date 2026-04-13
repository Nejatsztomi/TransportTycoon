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

        /// <summary>
        /// Attempts to deliver the specified load from the vehicle to all connected buildings and returns the remaining
        /// capacity of the vehicle after delivery.
        /// </summary>
        /// <remarks>If the vehicle is empty or there are no connected buildings that can accept the
        /// specified load, the method returns the original capacity. The method distributes the load to all eligible
        /// buildings based on their available capacity.</remarks>
        /// <param name="load">The type of load being delivered by the vehicle. Specify null if the vehicle is empty.</param>
        /// <param name="vehicleCanGive">The maximum amount of load, in units, that the vehicle can deliver during this operation. Must be zero or
        /// greater.</param>
        /// <returns>The remaining capacity of the vehicle after attempting to deliver the load. Returns 0 if at least one
        /// connected building accepts the load; otherwise, returns the original capacity.</returns>
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
                
                int buildingCanTake = building.BuildingEntity.Capacity - building.BuildingEntity.Occupancy;
                if (buildingCanTake >= vehicleCanGive)
                {
                    int buildingNewOccupancy = building.BuildingEntity.Occupancy + vehicleCanGive;
                    building.BuildingEntity.SetOccupancy(buildingNewOccupancy);
                    return 0;
                }
                else
                {
                    vehicleCanGive = vehicleCanGive - buildingCanTake;
                    building.BuildingEntity.SetOccupancy(building.BuildingEntity.Capacity);
                }
            }
            //returns the leftover what the buildings couldnt take
            return vehicleCanGive;
        }

        public (LoadType?, int) BuildingToVehicle(int maxVehicleCanTake, LoadType? currentLoad)
        {
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
