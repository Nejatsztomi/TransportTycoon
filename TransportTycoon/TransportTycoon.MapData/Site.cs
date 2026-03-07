using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public abstract class Site : BuildingBlocks
    {
        protected Site(int x, int y) : base(x, y) 
        {
            Scaler = 1;
        }
        
    }

    public class LumberCamp : Site
    {
        #region Constructor
        public LumberCamp(int x, int y) : base(x, y)
        {
            Offset = 20;
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
