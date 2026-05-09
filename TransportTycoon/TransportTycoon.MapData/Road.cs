using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData
{
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

    public struct Road : IInfrastructure
    {
        #region Static Fields
        public static int Price { get; } = 10;
        #endregion

        #region Fields
        public RoadType RoadType { get; private set; }
        public CityEntity? Pointer { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }

        #endregion

        #region Constructors
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
        /// <returns></returns>
        public readonly bool InCity()
        {
            return Pointer is not null;
        }
        #endregion
    }
}
