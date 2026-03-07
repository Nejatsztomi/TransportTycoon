using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Bus : Vehicle
    {
        #region Field
        public static LoadType acceptedLoad { get; } = LoadType.People;
        #endregion 
    }

    public class SmallBus : Vehicle
    {
        #region Constructor
        public SmallBus(int speed, Prouth route, int x, int y, Direction direction)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.SmallBus;

            //modifiable fields
            CurrentSpeed = speed; // or is it max in the beginning?
            CurrentLoad = null;
            CurrentCapacity = 0;
            Route = route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?      
        }
        #endregion
    }

    public class BigBus : Vehicle
    {
        #region Constructor
        public BigBus(int speed, Prouth route, int x, int y, Direction direction)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.BigBus;

            //modifiable fields
            CurrentSpeed = speed; // or is it max in the beginning?
            CurrentLoad = null;
            CurrentCapacity = 0;
            Route = route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?      
        }
        #endregion
    }
}
