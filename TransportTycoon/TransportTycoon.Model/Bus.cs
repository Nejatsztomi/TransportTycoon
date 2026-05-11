using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    /// <summary>
    /// Represents an abstract vehicle designed for transporting people along a specified route.
    /// </summary>
    /// <remarks>This class serves as a base for bus implementations and restricts accepted goods to
    /// passengers. Inherit from this class to define specific bus behaviors or features. The route and initial position
    /// are specified at construction.</remarks>
    public abstract class Bus : Vehicle
    {
        #region Protected constructors
        /// <summary>
        /// Initializes a new instance of the Bus class with the specified position, orientation, and route.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the bus's initial position.</param>
        /// <param name="y">The vertical coordinate of the bus's initial position.</param>
        /// <param name="angle">The initial orientation of the bus, in degrees.</param>
        /// <param name="route">The route assigned to the bus, or null if no route is assigned.</param>
        protected Bus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            AcceptedGoods = [LoadType.People];
        }
        #endregion 
    }

    /// <summary>
    /// Represents a bus with a small passenger capacity, suitable for routes requiring compact vehicles.
    /// </summary>
    /// <remarks>SmallBus is a specialized type of Bus designed for scenarios where maneuverability and lower
    /// capacity are preferred. It is initialized with predefined speed, capacity, and maintenance characteristics
    /// appropriate for small buses.</remarks>
    public sealed class SmallBus : Bus
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value used by the application.
        /// </summary>
        public static int Price { get; } = 500;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SmallBus class with the specified position, orientation, and route.
        /// </summary>
        /// <param name="x">The initial X-coordinate of the small bus.</param>
        /// <param name="y">The initial Y-coordinate of the small bus.</param>
        /// <param name="angle">The initial orientation angle of the small bus, in degrees.</param>
        /// <param name="route">The route assigned to the small bus. Can be null if no route is assigned.</param>
        public SmallBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 2.0;
            MaxCapacity = 10;
            Maintenance = 2;
            Type = VehicleType.SmallBus;
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }

    /// <summary>
    /// Represents a large-capacity bus with predefined characteristics, such as top speed and maximum passenger
    /// capacity.
    /// </summary>
    /// <remarks>BigBus is a specialized type of Bus designed to accommodate more passengers than standard
    /// buses. It is initialized with specific values for speed, capacity, and maintenance requirements. Use this class
    /// when modeling or simulating transportation scenarios that require large buses.</remarks>
    public sealed class BigBus : Bus
    {
        #region Static fields   
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 1200;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the BigBus class with the specified position, orientation, and route.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the initial position of the bus.</param>
        /// <param name="y">The vertical coordinate of the initial position of the bus.</param>
        /// <param name="angle">The initial orientation angle of the bus, in degrees.</param>
        /// <param name="route">The route assigned to the bus, or null if no route is assigned.</param>
        public BigBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 1.7;
            MaxCapacity = 25;
            Maintenance = 5;
            Type = VehicleType.BigBus;
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }
}
