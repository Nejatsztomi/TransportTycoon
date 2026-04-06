namespace TransportTycoon.MapData
{
    public enum RoadType
    {
        Horizontal = 0, Vertical = 1, RightTurn = 2, LeftTurn = 3, UpperRightTurn = 4, UpperLeftTurn = 5, UpperTRoad = 6, DownTRoad = 7, RightTRoad = 8, LeftTRoad = 9, XRoad = 10
    }
    public struct Road : IInfrastructure
    {
        #region Fields
        public RoadType RoadType { get; private set; }
        public (int, int)? Pointer { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public FieldType FieldType { get; set; }
        public readonly int Price => 100;
        #endregion

        #region Constructors
        public Road(int x, int y, RoadType type, int height)
        {
            X = x;
            Y = y;
            FieldType = FieldType.Road;
            RoadType = type;
            Pointer = null;
            Height = height;
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
