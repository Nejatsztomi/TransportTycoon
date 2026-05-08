using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Protected constructors
        protected Transport(int x, int y, double angle, Prouth? route = null)
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
        public Van(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 30;
            Price = 2200;
            Maintenance = 10;
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
            MaxCapacity = 10;
            Price = 600;
            Maintenance = 3;
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
        public Truck(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 20;
            Price = 1400;
            Maintenance = 6;
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
            MaxCapacity = 20;
            Price = 1800;
            Maintenance = 8;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = [LoadType.Oil];

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
