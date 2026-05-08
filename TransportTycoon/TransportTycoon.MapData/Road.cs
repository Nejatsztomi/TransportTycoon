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
        public void ChangeType(RoadType type)
        {
            RoadType = type;
        }

        public readonly bool InCity()
        {
            return Pointer is not null;
        }
        #endregion
    }
}
