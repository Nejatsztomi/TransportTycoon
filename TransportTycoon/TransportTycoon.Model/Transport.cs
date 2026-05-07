using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Protected constructors
        protected Transport(int x, int y, double angle, Prouth? route)
        {
            X = x;
            Y = y;
            Angle = angle;
            Prouth = route;
        }
        #endregion
    }

    public sealed class Van : Transport
    {
        #region Constructor 
        public Van(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.Van;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class Pickup : Transport
    {
        #region Constructor
        public Pickup(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.Pickup;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class Truck : Transport
    {
        #region Constructor
        public Truck(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 1.5;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.Truck;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class LiquidTruck : Transport
    {
        #region Constructor
        public LiquidTruck(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = [LoadType.Oil];

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
