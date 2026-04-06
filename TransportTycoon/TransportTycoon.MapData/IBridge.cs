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
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BridgeType BridgeType { get; set; }
        public readonly int Price => 50;
        public readonly int SpeedLimit => 100;
        public readonly int Range => 13;
        public readonly FieldType FieldType => FieldType.Bridge;
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
        #region Properties
        public BridgeType BridgeType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly int Price => 100;
        public readonly int SpeedLimit => 100;
        public readonly int Range => 15;
        public readonly FieldType FieldType => FieldType.Bridge;
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
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public BridgeType BridgeType { get; set; }
        public readonly int Price => 150;
        public readonly int SpeedLimit => 100;
        public readonly int Range => 17;
        public readonly FieldType FieldType => FieldType.Bridge;
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
