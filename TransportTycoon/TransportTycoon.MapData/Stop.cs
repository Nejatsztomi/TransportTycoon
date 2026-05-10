using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    /// <summary>
    /// Represents a stop infrastructure element that maintains connections to building blocks and provides methods to
    /// query which buildings can accept or provide goods based on vehicle capabilities.
    /// </summary>
    /// <remarks>A Stop maintains a collection of connected building blocks and exposes methods to determine
    /// which of these can interact with vehicles based on the types of goods they accept or provide. The static Price
    /// property indicates the cost associated with creating a stop. Connections may be null if not
    /// initialized.</remarks>
    public class Stop : Infrastructure
    {
        #region Static fields
        public static int Price { get; } = 300;
        #endregion

        #region Fields
        public List<BuildingBlocks>? Connections { get; private set; } = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Stop class with the specified coordinates and height.
        /// </summary>
        /// <param name="x">The X-coordinate of the stop location.</param>
        /// <param name="y">The Y-coordinate of the stop location.</param>
        /// <param name="height">The height value associated with the stop.</param>
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
        public List<BuildingBlocks> ShowWhatTheBuildingsCanGet(List<LoadType> vehicleAcceptedGoods)
        {
            List<BuildingBlocks> buildings = [];
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
        public List<BuildingBlocks> ShowWhatTheBuildingsCanGive(List<LoadType> vehicleAcceptedGoods)
        {
            List<BuildingBlocks> buildings = [];
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
        /// <summary>
        /// Adds the specified building block to the collection of connections, if the collection is not null.
        /// </summary>
        /// <remarks>If the Connections collection is null, the building block will not be
        /// added.</remarks>
        /// <param name="buildingBlock">The building block to add to the connections. This parameter must not be null.</param>
        public void SetBuildingBlocks(BuildingBlocks buildingBlock)
        {
            Connections?.Add(buildingBlock);
        }
        #endregion
    }
}
