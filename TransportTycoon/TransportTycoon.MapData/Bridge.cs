using System.Reflection;

namespace TransportTycoon.MapData
{
    public enum BridgeType
    {
        HorizontalBlueBridge = 0, VerticalBlueBridge = 1, HorizontalYellowBridge = 2, VerticalYellowBridge = 3, HorizontalRedBridge = 4, VerticalRedBridge = 5
    }

    public abstract class Bridge : Infrastructure
    {
        #region Fields
        public int SpeedLimit { get; protected set; }
        public int Range { get; protected set; }
        public BridgeType BridgeType { get; protected set; }
        #endregion
    }

    public class YellowBridge : Bridge
    {
        #region Constructor
        public YellowBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 100;
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

    public class BlueBridge : Bridge
    {
        #region Constructor
        public BlueBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 100;
            Range = 15;
            X = x;
            Y = y;
            FieldType = FieldType.Bridge;
            BridgeType = type;
            Height = height;
            Price=100;
        }
        #endregion
    }

    public class RedBridge : Bridge
    {
        #region Constructor
        public RedBridge(int x, int y, BridgeType type, int height)
        {
            SpeedLimit = 100;
            Range = 17;
            X = x;
            Y = y;
            FieldType = FieldType.Bridge;
            BridgeType = type;
            Height = height;
            Price=150;
        }
        #endregion
    }

}
