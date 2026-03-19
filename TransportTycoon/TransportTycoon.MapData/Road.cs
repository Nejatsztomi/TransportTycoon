using System;
using System.Collections.Generic;
using System.Text;

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
        public (int,int)? Pointer { get; private set; }
        #endregion

        #region Puplic methods
        public Road()
        {
            //...
        }

        public void ChangeType(RoadType type)
        {
            Type = type; 
        }
        public bool InCity()
        {
            if (Pointer == null) return false;
            else return true;
        }
        #endregion
    }
}
