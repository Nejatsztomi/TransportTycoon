using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Bus : Vehicle
    {
        #region Field
        protected Bus()
        {
            AcceptedGoods = [LoadType.People];
        }
        #endregion 
    }

    public sealed class SmallBus : Bus
    {
        #region Constructor
        public SmallBus(int x, int y, Direction direction, Prouth route = null!) : base()
        {
            //fixed fields
            TopSpeed = 1;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.SmallBus;

            //modifiable fields
            CurrentSpeed = TopSpeed;
            CurrentLoad = null;
            CurrentCapacity = 0;
            Prouth = route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?
        }
        #endregion
    }

    public sealed class BigBus : Bus
    {
        #region Constructor
        public BigBus(int x, int y, Direction direction, Prouth route = null!) : base()
        {
            //fixed fields
            TopSpeed = 1;
            MaxCapacity = 100;
            Price = 100;
            Maintenance = 100;
            Type = VehicleType.BigBus;

            //modifiable fields
            CurrentSpeed = TopSpeed;
            CurrentLoad = null;
            CurrentCapacity = 0;
            Prouth = route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?      
        }
        #endregion
    }
}
