namespace TransportTycoon.MapData
{
    public enum BridgeType
    {
        HorizontalGreenBridge = 0, VerticalGreenBridge = 1, HorizontalYellowBridge = 2, VerticalYellowBridge = 3, HorizontalRedBridge = 4, VerticalRedBridge = 5, Null = 6
    }

    public interface IBridge : IInfrastructure
    {

        #region Fields
        public int SpeedLimit { get; }
        public int Range { get; }
        public BridgeType BridgeType { get; protected set; }
        #endregion
    }

    public struct YellowBridge : IBridge
    {
        #region static Fields
        public static int Price { get; } = 60;
        #endregion
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BridgeType BridgeType { get; set; }
        public readonly int SpeedLimit => 50;
        public readonly int Range => 13;
        #endregion

        #region Constructor
        public YellowBridge(int x, int y, BridgeType type, int height)
        {
            X = x;
            Y = y;
            BridgeType = type;
            Height = height;
        }
        #endregion
    }

    public struct GreenBridge : IBridge
    {
        #region static Fields
        public static int Price { get; } = 80;
        #endregion

        #region Properties
        public BridgeType BridgeType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly int SpeedLimit => 50;
        public readonly int Range => 15;
        #endregion

        #region Constructor
        public GreenBridge(int x, int y, BridgeType type, int height)
        {
            X = x;
            Y = y;
            BridgeType = type;
            Height = height;
        }
        #endregion
    }

    public struct RedBridge : IBridge
    {
        #region Static Fields
        public static int Price { get; } = 100;
        #endregion

        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BridgeType BridgeType { get; set; }

        public readonly int SpeedLimit => 50;
        public readonly int Range => 17;
        #endregion

        #region Constructor
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
