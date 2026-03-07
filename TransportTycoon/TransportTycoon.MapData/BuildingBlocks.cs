using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

namespace TransportTycoon.MapData
{
    public abstract class BuildingBlocks
    {

        #region Fields
        #endregion

        #region Properties
        public int Capacity { protected set; get; } = 500;
        public int Occupancy { protected set; get; }
        public int Productivity { protected set; get; }
        public double Scaler { protected set; get; }
        public (int, int) Id { protected set; get; }
        public (int, int) Pointer { protected set; get; }
        #endregion

        #region Public Methods
        public int ChangeProduction() 
        {

        }

        public virtual int Production() 
        {

        }

        public bool IsMain() 
        {

        }

        public virtual Load GetLoad() 
        {

        }
        public void Unload(int q) 
        {

        }


        #endregion

        #region Private Methods
        #endregion



    }

    public class House() : BuildingBlocks 
    {
        public House() 
        {
            
        }
    }
}
