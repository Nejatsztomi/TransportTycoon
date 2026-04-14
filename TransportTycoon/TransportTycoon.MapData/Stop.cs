using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    public sealed class Stop : Infrastructure
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
        /// Retrieves a list of buildings that can accept any of the specified load types.
        /// </summary>
        /// <remarks>If the connections collection is null, the method returns an empty list. This method
        /// filters buildings based on the load types they can consume, as determined by each building's
        /// entity.</remarks>
        /// <param name="vehicleAcceptedGoods">A list of load types representing the goods that vehicles can deliver. Only buildings that accept at least
        /// one of these load types are included in the result.</param>
        /// <returns>A list of BuildingBlocks objects representing buildings that can accept at least one of the specified load
        /// types. The list is empty if no such buildings are found or if there are no connections.</returns>
        public List<BuildingBlocks> ShowWhatTheBuildingsCanGet(List<LoadType> vehicleAcceptedGoods)
        {
            List<BuildingBlocks> buildings = new List<BuildingBlocks>();
            if (Connenctions is null) return buildings;
            foreach (var building in Connenctions)
            {
                LoadType type = LoadType.None;
                Load? load = building.BuildingEntity.GetConsumeLoad();
                if (load != null)
                {
                    type = load.LoadType;
                }
                if (vehicleAcceptedGoods.Contains(type))
                {
                    buildings.Add(building);
                }

            }
            return buildings;
        }
        /// <summary>
        /// Retrieves a list of building blocks that can provide goods accepted by the specified vehicles.
        /// </summary>
        /// <remarks>If the connections are null, an empty list is returned. This method filters buildings
        /// based on the load type they provide and the types of goods that the vehicles can accept.</remarks>
        /// <param name="vehicleAcceptedGoods">A list of load types representing the goods that the vehicles can accept. Only buildings that provide one of
        /// these load types are included in the result.</param>
        /// <returns>A list of BuildingBlocks that can provide goods matching the specified vehicle accepted goods. The list is
        /// empty if no buildings match the criteria or if there are no connections.</returns>
        public List<BuildingBlocks> ShowWhatTheBuildingsCanGive(List<LoadType> vehicleAcceptedGoods)
        {
            List<BuildingBlocks> buildings = new List<BuildingBlocks>();
            if (Connenctions is null) return buildings;
            foreach (var building in Connenctions)
            {
                LoadType type = building.BuildingEntity.GetProvideLoad().LoadType;
                if (vehicleAcceptedGoods.Contains(type))
                {
                    buildings.Add(building);
                }

            }
            return buildings;
        }


        /// <summary>
        /// Attempts to deliver the specified load from the vehicle to all connected buildings and returns the remaining
        /// capacity of the vehicle after delivery.
        /// </summary>
        /// <remarks>If the vehicle is empty or there are no connected buildings that can accept the
        /// specified load, the method returns the original capacity. The method distributes the load to all eligible
        /// buildings based on their available capacity.</remarks>
        /// <returns>The remaining capacity of the vehicle after attempting to deliver the load. Returns 0 if at least one
        /// connected building accepts the load; otherwise, returns the original capacity.</returns>
        //public int VehicleToBuilding(LoadType? load, int vehicleCanGive)
        //{
        //    //a jarmu megerkezik, leadja a termeket ha tudja,
        //    //akkor tudja leadni ha a jarmu olyan termeket szallit, mint amit a gyár/város befogad
        //    if (load is null) return 0; // if the vehicle is empty, it can deliver nothing, so the leftover capacity is 0
        //    //if the stop has no connections, it has no needs, so the vehicle can deliver nothing, so the leftover capacity is 0
        //    if (ShowNeeds().Count == 0) return vehicleCanGive;
        //    if (Connenctions is null) return vehicleCanGive;

        //    List<BuildingBlocks> buildings = new List<BuildingBlocks>();
        //    foreach (var building in Connenctions)
        //    {
        //        if (building.BuildingEntity.GetConsumeLoad() == load)
        //        {
        //            buildings.Add(building);
        //        }
        //    }

        //    foreach(var building in buildings)
        //    {

        //        int buildingCanTake = building.BuildingEntity.Capacity - building.BuildingEntity.Occupancy;
        //        if (buildingCanTake >= vehicleCanGive)
        //        {
        //            int buildingNewOccupancy = building.BuildingEntity.Occupancy + vehicleCanGive;
        //            building.BuildingEntity.SetOccupancy(buildingNewOccupancy);
        //            return 0;
        //        }
        //        else
        //        {
        //            vehicleCanGive = vehicleCanGive - buildingCanTake;
        //            building.BuildingEntity.SetOccupancy(building.BuildingEntity.Capacity);
        //        }
        //    }
        //    //returns the leftover what the buildings couldnt take
        //    return vehicleCanGive;
        //}
        ////return what the building can give to the vehicle , and how much the vehicle can still take after taking the load from the building
        //public (LoadType?, int) BuildingToVehicle(int maxVehicleCanTake, LoadType? currentLoad)
        //{
        //    // if the stop has no connections, it has no loads, so the vehicle can take nothing, so the leftover capacity is 0
        //    if (ShowLoads().Count == 0) return (currentLoad, 0);
        //    if (Connenctions is null) return (currentLoad, 0);
        //    List<BuildingBlocks> buildings = new List<BuildingBlocks>();
        //    foreach (var building in Connenctions)
        //    {
        //        if (currentLoad != null)
        //        {
        //            if (building.BuildingEntity.GetProvideLoad() == currentLoad)
        //            {
        //                buildings.Add(building);
        //            }
        //        }
        //        else 
        //        {
        //            buildings.Add(building);
        //        }

        //    }


        //}
        public void SetBuildingBlocks(BuildingBlocks buildingBlock)
        {
            Connenctions?.Add(buildingBlock);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
