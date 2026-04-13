using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Field
        
        #endregion      
    }

    public class Van : Transport
    {
        #region Constructor
        public Van(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Van;
            //TODO: kitölteni az acceptedGoods listát
            AcceptedGoods = new List<LoadType> { LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat };

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
        public Pickup(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Pickup;
            //TODO: kitölteni az acceptedGoods listát
            AcceptedGoods = new List<LoadType> { LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat };

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
        public Truck(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Truck;
            //TODO: kitölteni az acceptedGoods listát
            AcceptedGoods = new List<LoadType> { LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat };

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
        public LiquidTruck(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.LiquidTruck;
            //TODO: kitölteni az acceptedGoods listát
            AcceptedGoods = [LoadType.Oil];

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
