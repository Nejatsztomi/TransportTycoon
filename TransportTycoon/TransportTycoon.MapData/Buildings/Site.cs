namespace TransportTycoon.MapData.Buildings
{
    /// <summary>
    /// Represents an abstract base class for a site within the application domain.
    /// </summary>
    /// <remarks>Inherit from this class to define specific types of sites with additional properties or
    /// behaviors. This class provides a common foundation for all site-related entities.</remarks>
    public abstract class Site : BuildingBlocks { }

    /// <summary>
    /// Represents a lumber camp site where wood resources are gathered and processed.
    /// </summary>
    /// <remarks>A LumberCamp is a specialized type of site associated with wood collection activities. It is
    /// initialized with a specific location and a corresponding building entity. This class is typically used in
    /// resource management or simulation scenarios where tracking and managing lumber operations is required.</remarks>
    public class LumberCamp : Site
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the LumberCamp class at the specified coordinates with the given building
        /// entity.
        /// </summary>
        /// <param name="x">The X-coordinate of the lumber camp's location.</param>
        /// <param name="y">The Y-coordinate of the lumber camp's location.</param>
        /// <param name="buildingEntity">The LumberCampEntity that represents the building associated with this lumber camp. Cannot be null.</param>
        public LumberCamp(int x, int y, LumberCampEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    /// <summary>
    /// Represents a mine site with specific coordinates, height, and associated building entity.
    /// </summary>
    /// <remarks>The Mine class extends the Site type to provide additional properties relevant to a mine,
    /// such as its position and the building entity it contains. Use this class to model mine locations and their
    /// associated data within the application.</remarks>
    public class Mine : Site
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Mine class at the specified coordinates with the given building entity.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the mine location.</param>
        /// <param name="y">The vertical coordinate of the mine location.</param>
        /// <param name="buildingEntity">The building entity associated with the mine. Cannot be null.</param>
        public Mine(int x, int y, MineEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    /// <summary>
    /// Represents a farm site with specific coordinates and an associated building entity.
    /// </summary>
    /// <remarks>The Farm class extends the Site type to provide additional properties for location and
    /// building association. It is typically used to model a farm's position and structure within a larger site
    /// management system.</remarks>
    public class Farm : Site
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Farm class at the specified coordinates with the given building entity.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the farm location.</param>
        /// <param name="y">The vertical coordinate of the farm location.</param>
        /// <param name="buildingEntity">The building entity associated with the farm. Cannot be null.</param>
        public Farm(int x, int y, FarmEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
