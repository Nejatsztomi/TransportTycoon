using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

namespace TransportTycoon.MapData
{
    public abstract class Site : BuildingBlocks
    {
        protected Site(int x, int y) : base(x, y) { }
        
    }

    public class LumberCamp : Site
    {
        #region Constructor
        public LumberCamp(int x, int y) : base(x, y)
        {
            Offset = 20;
            Scaler = 1;
        }
        #endregion

        #region Methods
        public override LoadType GetLoad()
        {
            return LoadType.Wood;
        }
        #endregion
    }

    public class Mine : Site
    {
        #region Constructor
        public Mine(int x, int y) : base(x, y)
        {
            Offset = 30;
            Scaler = 1;
        }
        #endregion

        #region Methods
        public override LoadType GetLoad()
        {
            return LoadType.Oil;
        }
        #endregion
    }

    public class Farm : Site
    {
        #region Constructor
        public Farm(int x, int y) : base(x, y)
        {
            Offset = 40;
            Scaler = 1;
        }
        #endregion

        #region Methods
        public override LoadType GetLoad()
        {
            return LoadType.Wheat;
        }
        #endregion
    }
}
