using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public abstract class Field
    {

        #region Properties
        public int X { protected set; get; }
        public int Y { protected set; get; }
        public int Height { protected set; get; }
        public virtual bool Modifiable { protected set; get; }
        #endregion
        
        #region Public Methods
        #endregion
        #region Private Methods
        #endregion
        


       


    }
}
