using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public abstract class Infrastructure : Field
    {
        #region Field
        public int Price { get; protected set; }
        #endregion

        #region Public methods
        public void Place()
        {

        }
        public void Remove() // makes a terrain with a height based on infrastructure's height, and replace 
        {

        }
        #endregion
    }
}
