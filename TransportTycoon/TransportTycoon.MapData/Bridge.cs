namespace TransportTycoon.MapData
{
    public enum BridgeType
    {
        HorizontalGreenBridge = 0, VerticalGreenBridge = 1, HorizontalYellowBridge = 2, VerticalYellowBridge = 3, HorizontalRedBridge = 4, VerticalRedBridge = 5, Null = 6
    }

    public abstract class Bridge : Infrastructure
    {
        #region Fields
        public double SpeedLimit { get; protected set; }
        public int Range { get; protected set; }
        public BridgeType BridgeType { get; protected set; }
        #endregion
    }

    public sealed class YellowBridge : Bridge
    {
        #region Constructor
        public YellowBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 0.8;
            Range = 13;
            X = x;
            Y = y;
            FieldType = FieldType.Bridge;
            BridgeType = type;
            Height = height;
            Price = 50;
        }
        #endregion
    }

    public sealed class GreenBridge : Bridge
    {
        #region Constructor
        public GreenBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 0.8;
            Range = 15;
            X = x;
            Y = y;
            FieldType = FieldType.Bridge;
            BridgeType = type;
            Height = height;
            Price = 100;
        }
        #endregion
    }

    public sealed class RedBridge : Bridge
    {
        #region Constructor
        public RedBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 0.8;
            Range = 17;
            X = x;
            Y = y;
            FieldType = FieldType.Bridge;
            BridgeType = type;
            Height = height;
            Price = 150;
        }
        #endregion
    }

}
