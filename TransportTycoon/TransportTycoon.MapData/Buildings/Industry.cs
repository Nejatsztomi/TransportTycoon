namespace TransportTycoon.MapData.Buildings
{
    /// <summary>
    /// Represents an abstract base class for industry-related domain entities.
    /// </summary>
    /// <remarks>Inherit from this class to define specific types of industries within the domain model. This
    /// class provides a common foundation for all industry implementations.</remarks>
    public abstract class Industry : BuildingBlocks { }

    /// <summary>
    /// Represents a mill industry building within the simulation, providing access to its associated building entity
    /// and location.
    /// </summary>
    /// <remarks>The Mill class extends the Industry base class and is typically used to model mills as part
    /// of a larger industrial or simulation system. The associated building entity provides additional details and
    /// functionality specific to the mill.</remarks>
    public class Mill : Industry
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Mill class with the specified coordinates and building entity.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the mill location.</param>
        /// <param name="y">The vertical coordinate of the mill location.</param>
        /// <param name="buildingEntity">The MillEntity instance that represents the building associated with this mill. Cannot be null.</param>
        public Mill(int x, int y, MillEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    /// <summary>
    /// Represents an industrial plant that is associated with a specific building entity.
    /// </summary>
    /// <remarks>A Plant is a specialized type of Industry that encapsulates a building entity of type
    /// PlantEntity. Use this class to model plants within the industrial domain, associating them with their spatial
    /// coordinates and building data.</remarks>
    public class Plant : Industry
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Plant class with the specified coordinates and associated building entity.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the plant location.</param>
        /// <param name="y">The vertical coordinate of the plant location.</param>
        /// <param name="buildingEntity">The PlantEntity instance associated with this plant. Cannot be null.</param>
        public Plant(int x, int y, PlantEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }

    /// <summary>
    /// Represents a factory building within an industry, providing access to its associated building entity and
    /// location.
    /// </summary>
    /// <remarks>Use this class to model a factory as part of an industrial system. The factory is initialized
    /// with its coordinates and a specific factory entity, and exposes its building entity for further operations.
    /// Inherits from the Industry base class.</remarks>
    public class Factory : Industry
    {
        #region Public properties
        public override BuildingEntity BuildingEntity { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Factory class with the specified coordinates and building entity.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the factory location.</param>
        /// <param name="y">The vertical coordinate of the factory location.</param>
        /// <param name="buildingEntity">The building entity associated with the factory. Cannot be null.</param>
        public Factory(int x, int y, FactoryEntity buildingEntity)
        {
            X = x;
            Y = y;
            Height = -1;
            BuildingEntity = buildingEntity;
        }
        #endregion
    }
}
