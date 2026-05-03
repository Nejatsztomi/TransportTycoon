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
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
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
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.BigBus;

            //modifiable fields
            CurrentSpeed = TopSpeed;
        }
        #endregion
    }
}
