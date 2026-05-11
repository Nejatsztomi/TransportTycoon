namespace TransportTycoon.MapData
{
    /// <summary>
    /// Specifies the available types of bridges, categorized by orientation and color.
    /// </summary>
    /// <remarks>Use this enumeration to indicate the specific bridge type in scenarios where orientation
    /// (horizontal or vertical) and color (green, yellow, or red) are relevant. The Null value represents the absence
    /// of a bridge.</remarks>
    public enum BridgeType
    {
        HorizontalGreenBridge = 0,
        VerticalGreenBridge = 1,
        HorizontalYellowBridge = 2,
        VerticalYellowBridge = 3,
        HorizontalRedBridge = 4,
        VerticalRedBridge = 5,
        Null = 6,
    }

    /// <summary>
    /// Represents an abstract base class for a bridge, providing common properties such as speed limit, range, and
    /// bridge type.
    /// </summary>
    /// <remarks>Inherit from this class to implement specific types of bridges with custom behavior. The
    /// Bridge class exposes key characteristics that are shared across different bridge implementations.</remarks>
    public abstract class Bridge : Infrastructure
    {
        #region Fields
        /// <summary>
        /// Gets the maximum allowed speed for the associated entity.
        /// </summary>
        public abstract double SpeedLimit { get; }

        /// <summary>
        /// Gets the range value represented by this property.
        /// </summary>
        public abstract int Range { get; }

        /// <summary>
        /// Gets the type of bridge represented by this instance.
        /// </summary>
        public BridgeType BridgeType { get; protected set; }
        #endregion
    }

    /// <summary>
    /// Represents a bridge of the Yellow type with predefined characteristics such as price, speed limit, and range.
    /// </summary>
    /// <remarks>YellowBridge is a specific implementation of the Bridge class, providing fixed values for
    /// price, speed limit, and range. Use this class when a bridge with these characteristics is required.</remarks>
    public class YellowBridge : Bridge
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 60;
        #endregion

        #region Public properties
        public override double SpeedLimit => 0.5;
        public override int Range => 13;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the YellowBridge class with the specified coordinates, bridge type, and
        /// height.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the bridge location.</param>
        /// <param name="y">The vertical coordinate of the bridge location.</param>
        /// <param name="type">The type of the bridge to create.</param>
        /// <param name="height">The height of the bridge. Must be a non-negative integer.</param>
        public YellowBridge(int x, int y, BridgeType type, int height)
        {
            X = x;
            Y = y;
            BridgeType = type;
            Height = height;
        }
        #endregion
    }

    /// <summary>
    /// Represents a bridge with predefined characteristics, including price, speed limit, and range.
    /// </summary>
    /// <remarks>The GreenBridge class provides a specific implementation of a bridge with fixed values for
    /// price, speed limit, and range. It is typically used in scenarios where a standardized bridge configuration is
    /// required.</remarks>
    public class GreenBridge : Bridge
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 80;
        #endregion

        #region Public properties
        public override double SpeedLimit => 0.5;
        public override int Range => 15;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the GreenBridge class with the specified coordinates, bridge type, and height.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the bridge location.</param>
        /// <param name="y">The vertical coordinate of the bridge location.</param>
        /// <param name="type">The type of the bridge to create.</param>
        /// <param name="height">The height of the bridge.</param>
        public GreenBridge(int x, int y, BridgeType type, int height)
        {
            X = x;
            Y = y;
            BridgeType = type;
            Height = height;
        }
        #endregion
    }

    /// <summary>
    /// Represents a bridge of type RedBridge with predefined speed limit, range, and price characteristics.
    /// </summary>
    /// <remarks>RedBridge is a specialized implementation of the Bridge class, providing fixed values for
    /// speed limit, range, and price. Use this class when a bridge with these specific characteristics is
    /// required.</remarks>
    public class RedBridge : Bridge
    {
        #region Static fields
        /// <summary>
        /// Gets the default price value.
        /// </summary>
        public static int Price { get; } = 100;
        #endregion

        #region Public properties
        public override double SpeedLimit => 0.5;
        public override int Range => 17;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the RedBridge class with the specified coordinates, bridge type, and height.
        /// </summary>
        /// <param name="x">The X-coordinate of the bridge's location.</param>
        /// <param name="y">The Y-coordinate of the bridge's location.</param>
        /// <param name="type">The type of the bridge to create.</param>
        /// <param name="height">The height of the bridge.</param>
        public RedBridge(int x, int y, BridgeType type, int height)
        {
            X = x;
            Y = y;
            BridgeType = type;
            Height = height;
        }
        #endregion
    }

}
