using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

namespace TransportTycoon.MapData
{
    public abstract class BuildingBlocks : Field
    {

        #region Fields
        #endregion

        #region Properties
        //mennyit tud tárolni
        public int Capacity { protected set; get; } = 1000;
        //jeleneleg mennyit termelt
        public int Occupancy { protected set; get; }
        //milyen mennyiseggel termel
        public int Productivity { protected set; get; }
        //melyik telephely milyen szorzoval termel
        public double Scaler { protected set; get; }
        public double Offset { protected set; get; }
        public (int X, int Y) Id { protected set; get; }
        public (int X, int Y) Pointer { protected set; get; }
        #endregion

        #region Public Methods
        protected double GetMultiplier() 
        {
            double period = 300;
            double time = DateTime.Now.TimeOfDay.Seconds;

            //sin()->[-1,1]
            //0.5*sin() ->[-0.5, 0.5]
            //1.5 + 0.5*sin() ->[1.0, 2.0]

            double multiplier =1.5 + 0.5 *Math.Sin(( 2 * Math.PI * (time+Offset)) / period);

            return multiplier;
        }

        //the production itself
        public abstract void Production();
        

        public bool IsMain() 
        {
            return Id.Item1==Pointer.Item1 && Id.Item2==Pointer.Item2;
        }

        //Returns the facility's load  
        public abstract Load GetLoad(); 
        
        
        public int Unload(int q) //returns the maximum what the factory can give
        {
            if (q >= Occupancy)
            {
                Occupancy = 0;
                
            }
            else 
            {
                Occupancy -= q;
                
            }

            return Occupancy;
        }


        #endregion

        #region Private Methods
        #endregion



    }

    public class House() : BuildingBlocks 
    {
        public House(int x, int y ) 
        {
            X= x;
            Y = y;
            Id.Item1. = x;
            Id.Item2 = y;
            Scaler = 10;
        }
    }
}
