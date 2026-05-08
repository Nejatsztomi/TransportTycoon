using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    public struct Stop : IInfrastructure
    {
        #region Static Fields
        public static int Price { get; } = 300;
        #endregion

        #region Fields
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public List<IBuildingBlocks>? Connections { get; private set; } = [];
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
        /// <summary>
        /// the method that retrieves a list of building blocks that can accept goods from the specified vehicles.
        /// </summary>
        /// <param name="vehicleAcceptedGoods"></param>
        /// <returns></returns>
        public readonly List<IBuildingBlocks> ShowWhatTheBuildingsCanGet(List<LoadType> vehicleAcceptedGoods)
        {
            List<IBuildingBlocks> buildings = [];
            if (Connections is null) return buildings;
            foreach (var building in Connections)
            {
                if (building.BuildingEntity is CityEntity)
                {
                    buildings.Add(building);
                    continue;
                }
                LoadType type = LoadType.None;
                Load? load = building.BuildingEntity.GetConsumeLoad();
                if (load is not null)
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
        public readonly List<IBuildingBlocks> ShowWhatTheBuildingsCanGive(List<LoadType> vehicleAcceptedGoods)
        {
            List<IBuildingBlocks> buildings = [];
            if (Connections is null) return buildings;
            foreach (var building in Connections)
            {
                LoadType type = building.BuildingEntity.GetProvideLoad().LoadType;
                if (vehicleAcceptedGoods.Contains(type))
                {
                    buildings.Add(building);
                }
            }
            return buildings;
        }

        public readonly void SetBuildingBlocks(IBuildingBlocks buildingBlock)
        {
            Connections?.Add(buildingBlock);
        }
        #endregion
    }
}
