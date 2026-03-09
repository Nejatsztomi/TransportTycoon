using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public enum BridgeType
    {
        HorizontalBlueBridge = 0, VerticalBlueBridge = 1, HorizontalYellowBridge = 2, VerticalYellowBridge = 3, HorizontalRedBridge = 4, VerticalRedBridge = 5
    }
    public abstract class Bridge : Infrastructure
    {
        #region Fields
        public int SpeedLimit { get; private set; }
        public int Range { get; private set; }
        public BridgeType Type { get; private set; }
        #endregion
    }
    public class YellowBridge : Bridge
    {
        #region Constructor
        public YellowBridge()
        {
            //...
        }
        #endregion
    }
    public class BlueBridge : Bridge
    {
        #region Constructor
        public BlueBridge()
        {
            //...
        }
        #endregion
    }
    public class RedBridge : Bridge
    {
        #region Constructor
        public RedBridge()
        {
            //...
        }
        #endregion
    }
}
