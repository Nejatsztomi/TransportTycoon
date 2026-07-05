namespace TransportTycoon.MapData
{
    /// <summary>
    /// Specifies the type of load or cargo that can be transported or processed.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the kind of material or entity being handled, such as wheat,
    /// oil, wood, or people. The values correspond to distinct categories relevant to logistics, transportation, or
    /// inventory management scenarios.</remarks>
    public enum LoadType
    {
        None,
        Wheat,
        Oil,
        Wood,
        Flour,
        Rubber,
        Paper,
        People
    }

    /// <summary>
    /// Represents an abstract base class for different types of loads, providing common properties such as price and
    /// load type.
    /// </summary>
    /// <remarks>This class is intended to be inherited by specific load implementations. It encapsulates
    /// shared characteristics and behaviors for various load types within the application.</remarks>
    public abstract class Load
    {
        #region Properties
        /// <summary>
        /// Gets or sets the price value associated with this instance.
        /// </summary>
        public int Price { get; protected set; }

        /// <summary>
        /// Gets or sets the type of load operation to perform.
        /// </summary>
        public LoadType LoadType { get; set; }
        #endregion
    }

    /// <summary>
    /// Represents a load type for transporting people within the system.
    /// </summary>
    /// <remarks>This class is a specialized form of the Load type, specifically configured for handling
    /// people as cargo. It sets default values appropriate for people transport scenarios.</remarks>
    public sealed class People : Load
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the People class with default values.
        /// </summary>
        public People()
        {
            Price = 3;
            LoadType = LoadType.People;
        }
        #endregion
    }
}
