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
        #region Constructor
        public SmallBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 1;
            MaxCapacity = 10;
            Price = 500;
            Maintenance = 2;
            Type = VehicleType.SmallBus;

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }

    public sealed class BigBus : Bus
    {
        #region Constructor
        public BigBus(int x, int y, double angle, Prouth? route) : base(x, y, angle, route)
        {
            //fixed fields
            TopSpeed = 1;
            MaxCapacity = 25;
            Price = 1200;
            Maintenance = 5;
            Type = VehicleType.BigBus;

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
