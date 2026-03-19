using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Field
        public List<LoadType>? AcceptedGoods { get; protected set; }
        #endregion      
    }

    public class Van : Transport
    {
        #region Constructor
        public Van(Prouth route, int x, int y, Direction direction, List<LoadType> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Van;
            AcceptedGoods = acceptedGoods;

            //modifiable fields
            CurrentSpeed = TopSpeed;
            CurrentLoad = null;
            CurrentCapacity = 0;
            Route = route;
            X = x;
            Y = y;
            Direction = direction; //get info from route?      
        }
        #endregion
    }

    public class Pickup : Transport
    {
        #region Constructor
        public Pickup(Prouth route, int x, int y, Direction direction, List<LoadType> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Pickup;
            AcceptedGoods = acceptedGoods;

            //modifiable fields
            CurrentSpeed = TopSpeed;
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
        public Truck(Prouth route, int x, int y, Direction direction, List<LoadType> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Truck;
            AcceptedGoods = acceptedGoods;

            //modifiable fields
            CurrentSpeed = TopSpeed;
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
        public LiquidTruck(Prouth route, int x, int y, Direction direction, List<LoadType> acceptedGoods)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = acceptedGoods;

            //modifiable fields
            CurrentSpeed = TopSpeed;
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
