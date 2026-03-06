using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Field
        public List<Goods> AcceptedGoods {  get; protected set; }
        #endregion      
    }

    public class Van : Transport
    {
        #region Constructor
        public Van(int speed, Prouth route, int x, int y, Direction direction, List<Goods> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.Van;
            AcceptedGoods = acceptedGoods;

            //modifiable fields
            CurrentSpeed = speed; // or is it max in the beginning?
            CurrentLoad = null;
            CurrentCapacity = 0;
            Route=route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?      
        }
        #endregion
    }

    public class Pickup : Transport
    {
        #region Constructor
        public Pickup(int speed, Prouth route, int x, int y, Direction direction, List<Goods> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.Pickup;
            AcceptedGoods = acceptedGoods;

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

    public class Truck : Transport
    {
        #region Constructor
        public Truck(int speed, Prouth route, int x, int y, Direction direction, List<Goods> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.Truck;
            AcceptedGoods = acceptedGoods;

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

    public class LiquidTruck : Transport
    {
        #region Constructor
        public LiquidTruck(int speed, Prouth route, int x, int y, Direction direction, List<Goods> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            this.Type = VehicleType.LiquidTruck;
            AcceptedGoods = acceptedGoods;

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
