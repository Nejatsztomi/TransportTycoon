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
        public virtual int GetTrees() => 0;
        #endregion
        #region Private Methods
        #endregion
        


       


    }

    public class Water : Field
    {
        public Water(int x, int y)
        {
            X = x;
            Y = y;
            Height = 0;
        }
    }
}
