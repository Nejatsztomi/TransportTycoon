using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    /// <summary>
    /// Represents an abstract base class for vehicles that support transport functionality.
    /// </summary>
    /// <remarks>Inherit from this class to implement specific types of transport vehicles. This class
    /// provides common initialization for position and orientation, and may be extended to add transport-specific
    /// behavior.</remarks>
    public abstract class Transport : Vehicle
    {
        #region Protected constructors
        /// <summary>
        /// Initializes a new instance of the Transport class with the specified position, orientation, and optional
        /// route.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the transport's initial position.</param>
        /// <param name="y">The vertical coordinate of the transport's initial position.</param>
        /// <param name="angle">The initial orientation angle of the transport, in degrees.</param>
        /// <param name="route">An optional route to assign to the transport. If null, the transport is not assigned a route.</param>
        protected Transport(int x, int y, double angle, Prouth? route = null)
        {
            X = x;
            Y = y;
            Angle = angle;
            Prouth = route;
        }
        #endregion
    }

    /// <summary>
    /// Represents a van transport vehicle with predefined characteristics and accepted goods types.
    /// </summary>
    /// <remarks>The Van class is a specialized type of Transport configured for carrying specific goods such
    /// as flour, paper, wood, rubber, and wheat. It provides default values for speed, capacity, and maintenance, and
    /// is intended for use in scenarios where these characteristics are required.</remarks>
    public sealed class Van : Transport
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 2200;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Van class with the specified position, orientation, and optional route.
        /// </summary>
        /// <param name="x">The initial X-coordinate of the van's position.</param>
        /// <param name="y">The initial Y-coordinate of the van's position.</param>
        /// <param name="angle">The initial orientation angle of the van, in degrees.</param>
        /// <param name="route">The route assigned to the van. If null, the van is initialized without a route.</param>
        public Van(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            TopSpeed = 1.5;
            MaxCapacity = 30;
            Maintenance = 10;
            Type = VehicleType.Van;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }

    /// <summary>
    /// Represents a pickup vehicle used for transporting specific types of goods with moderate capacity and speed.
    /// </summary>
    /// <remarks>The Pickup class is a specialized type of Transport configured to carry a limited set of load
    /// types, including flour, paper, wood, rubber, and wheat. It is designed for scenarios where moderate cargo
    /// capacity and speed are required. The static Price property indicates the cost associated with acquiring a pickup
    /// vehicle.</remarks>
    public sealed class Pickup : Transport
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 600;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Pickup class with the specified position, orientation, and route.
        /// </summary>
        /// <param name="x">The X-coordinate of the pickup's initial position.</param>
        /// <param name="y">The Y-coordinate of the pickup's initial position.</param>
        /// <param name="angle">The initial orientation angle of the pickup, in degrees.</param>
        /// <param name="route">The route assigned to the pickup. Can be null if no route is assigned.</param>
        public Pickup(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 1.2;
            MaxCapacity = 10;
            Maintenance = 3;
            Type = VehicleType.Pickup;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }

    /// <summary>
    /// Represents a truck vehicle used for transporting specific types of goods.
    /// </summary>
    /// <remarks>The Truck class is a specialized type of Transport designed to carry various load types such
    /// as flour, paper, wood, rubber, and wheat. It provides properties for speed, capacity, and maintenance relevant
    /// to truck operations. This class is sealed and cannot be inherited.</remarks>
    public sealed class Truck : Transport
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 1400;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Truck class at the specified position and orientation, with an optional
        /// route.
        /// </summary>
        /// <param name="x">The X-coordinate of the truck's initial position.</param>
        /// <param name="y">The Y-coordinate of the truck's initial position.</param>
        /// <param name="angle">The initial orientation angle of the truck, in degrees.</param>
        /// <param name="route">The route assigned to the truck. If null, the truck is initialized without a route.</param>
        public Truck(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            TopSpeed = 0.8;
            MaxCapacity = 20;
            Maintenance = 6;
            Type = VehicleType.Truck;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }

    /// <summary>
    /// Represents a specialized transport vehicle designed for carrying liquid goods, such as oil, along a specified
    /// route.
    /// </summary>
    /// <remarks>The LiquidTruck class is a sealed type that inherits from Transport and is configured to
    /// accept only liquid cargo types. It is initialized with predefined speed, capacity, and maintenance values
    /// suitable for liquid transport operations.</remarks>
    public sealed class LiquidTruck : Transport
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 1800;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the LiquidTruck class with the specified position, orientation, and route.
        /// </summary>
        /// <param name="x">The X-coordinate of the truck's initial position.</param>
        /// <param name="y">The Y-coordinate of the truck's initial position.</param>
        /// <param name="angle">The initial orientation angle of the truck, in degrees.</param>
        /// <param name="route">The route assigned to the truck. Can be null if no route is specified.</param>
        public LiquidTruck(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 0.6;
            MaxCapacity = 20;
            Maintenance = 8;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = [LoadType.Oil];
            CurrentSpeed = TopSpeed;
            TurnSpeed = 360.0 * TopSpeed;
        }
        #endregion
    }
}
