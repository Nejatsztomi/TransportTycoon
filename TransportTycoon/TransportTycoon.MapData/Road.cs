namespace TransportTycoon.MapData
{
    public enum RoadType
    {
        Horizontal = 0, Vertical = 1, RightTurn = 2, LeftTurn = 3, UpperRightturn = 4, UpperLeftTurn = 5, UpperTRoad = 6, DownTRoad = 7, RightTRoad = 8, LeftTRoad = 9, XRoad = 10
    }
    public class Road : Infrastructure
    {
        #region Fields
        public RoadType Type { get; private set; }
        public (int, int)? Pointer { get; private set; }
        #endregion

        #region Constructors
        public Road() { }
        #endregion

        #region Public methods
        public void ChangeType(RoadType type)
        {
            Type = type;
        }

        public bool InCity()
        {
            return Pointer is null;
        }
        #endregion
    }
}
