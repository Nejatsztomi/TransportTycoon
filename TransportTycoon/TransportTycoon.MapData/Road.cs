using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
    /// <summary>
    /// Specifies the type of road segment, indicating its orientation or intersection pattern within a road network.
    /// </summary>
    /// <remarks>Use this enumeration to represent different road shapes, such as straight segments, turns,
    /// T-junctions, and crossroads, when modeling or rendering road layouts. The values correspond to common road
    /// configurations encountered in grid-based or map-based systems.</remarks>
    public enum RoadType : byte
    {
        Horizontal = 0,
        Vertical = 1,
        RightTurn = 2,
        LeftTurn = 3,
        UpperRightTurn = 4,
        UpperLeftTurn = 5,
        UpperTRoad = 6,
        DownTRoad = 7,
        RightTRoad = 8,
        LeftTRoad = 9,
        XRoad = 10,
    }

    /// <summary>
    /// Represents a road infrastructure element within a city, including its type, position, and optional connection to
    /// a city entity.
    /// </summary>
    /// <remarks>A Road can be associated with a specific type, coordinates, height, and may optionally be
    /// linked to a city entity via the Pointer property. The static Price property indicates the cost associated with
    /// constructing a road. This class is typically used to model roads, bridges, and their connections within a city
    /// simulation or infrastructure management context.</remarks>
    public class Road : Infrastructure
    {
        #region Static Fields
        public static int Price { get; } = 10;
        #endregion

        #region Fields
        /// <summary>
        /// Gets the type of road associated with this instance.
        /// </summary>
        public RoadType RoadType { get; private set; }

        /// <summary>
        /// Gets the referenced city entity, if available.
        /// </summary>
        public CityEntity? Pointer { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Road class with the specified coordinates, road type, height, and optional
        /// city entity pointer.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the road.</param>
        /// <param name="y">The vertical coordinate of the road.</param>
        /// <param name="type">The type of the road to create.</param>
        /// <param name="height">The height value associated with the road.</param>
        /// <param name="pointer">An optional reference to a related city entity. May be null if no association is required.</param>
        public Road(int x, int y, RoadType type, int height, CityEntity? pointer = null)
        {
            X = x;
            Y = y;
            RoadType = type;
            Height = height;
            Pointer = pointer;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Changes the roadtype of the road to the specified type. This method can be used to change the type of the road when building bridges or when connecting roads to cities. For example, when building a horizontal bridge, the roadtype of the road will be changed to Horizontal, and when connecting a road to a city, the roadtype will be changed to RightTurn or LeftTurn depending on the direction of the connection.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeType(RoadType type)
        {
            RoadType = type;
        }

        /// <summary>
        /// Decides if the road is in a city by checking if the pointer to the city is not null. If the pointer is not null, it means that the road is connected to a city and therefore is considered to be in a city.
        /// </summary>
        /// <returns>True if the road is connected to a city; otherwise, false.</returns>
        public bool InCity() => Pointer is not null;
        #endregion
    }
}
