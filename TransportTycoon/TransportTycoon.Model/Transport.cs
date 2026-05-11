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
        #region Static Fields
        public static int Price { get; } = 1200;
        #endregion
        #region Constructor 
        public Van(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            TopSpeed = 0.9;
            MaxCapacity = 30;
            Maintenance = 10;
            Type = VehicleType.Van;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class Pickup : Transport
    {
        #region Static Fields
        public static int Price { get; } = 600;
        #endregion

        #region Constructor
        public Pickup(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 0.9;
            MaxCapacity = 10;
            Maintenance = 3;
            Type = VehicleType.Pickup;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class Truck : Transport
    {
        #region Static Fields
        public static int Price { get; } = 1400;
        #endregion

        #region Constructor
        public Truck(int x, int y, double angle, Prouth? route = null) : base(x, y, angle, route)
        {
            TopSpeed = 0.9;
            MaxCapacity = 20;
            Maintenance = 6;
            Type = VehicleType.Truck;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class LiquidTruck : Transport
    {
        #region Static Fields
        public static int Price { get; } = 1800;
        #endregion
        #region Constructor
        public LiquidTruck(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 0.9;
            MaxCapacity = 20;
            Maintenance = 8;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = [LoadType.Oil];
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
