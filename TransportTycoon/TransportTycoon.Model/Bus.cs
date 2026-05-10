using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Bus : Vehicle
    {
        #region Protected constructors
        protected Bus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            AcceptedGoods = [LoadType.People];
        }
        #endregion 
    }

    public sealed class SmallBus : Bus
    {
        #region Static Fields
        public static int Price { get; } = 500;
        #endregion

        #region Constructor
        public SmallBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 1;
            MaxCapacity = 10;
            Maintenance = 2;
            Type = VehicleType.SmallBus;
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class BigBus : Bus
    {
        #region Static Fields
        public static int Price { get; } = 1200;
        #endregion

        #region Constructor
        public BigBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            TopSpeed = 1;
            MaxCapacity = 25;
            Maintenance = 5;
            Type = VehicleType.BigBus;
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
