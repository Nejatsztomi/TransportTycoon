namespace TransportTycoon.MapData.Buildings
{
    /// <summary>
    /// Represents an abstract base class for fields that are associated with a building entity.
    /// </summary>
    /// <remarks>This class provides a foundation for specialized field types that relate to building
    /// entities. It exposes the associated building entity through the BuildingEntity property. Inherit from this class
    /// to implement custom building-related fields.</remarks>
    public abstract class BuildingBlocks : Field
    {
        #region Properties
        /// <summary>
        /// Gets or sets the building entity associated with this instance.
        /// </summary>
        public abstract BuildingEntity BuildingEntity { get; protected set; }
        #endregion
    }

    /// <summary>
    /// Represents a residential building within the city simulation, providing functionality specific to houses.
    /// </summary>
    /// <remarks>Inherits from BuildingBlocks and encapsulates data and behavior related to house entities.
    /// Use this class to model and manage individual houses as part of the broader city infrastructure.</remarks>
    public class House : BuildingBlocks
    {
        #region Properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the House class at the specified coordinates and associates it with the given
        /// building entity.
        /// </summary>
        /// <param name="x">The X-coordinate of the house location.</param>
        /// <param name="y">The Y-coordinate of the house location.</param>
        /// <param name="buildingEntity">The CityEntity that represents the building associated with this house. Cannot be null.</param>
        public House(int x, int y, CityEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
