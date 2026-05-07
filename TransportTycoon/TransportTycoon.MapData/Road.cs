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
        #region Public properties
        public RoadType RoadType { get; private set; }
        public BuildingEntity? Pointer { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly int Price => 100;
        #endregion

        #region Constructors
        public Road(int x, int y, RoadType type, int height, BuildingEntity? pointer = null)
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
