using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Field

        #endregion
    }

    public sealed class Van : Transport
    {
        #region Constructor 
        public Van(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Van;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

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

    public sealed class Pickup : Transport
    {
        #region Constructor
        public Pickup(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Pickup;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

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

    public sealed class Truck : Transport
    {
        #region Constructor
        public Truck(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 1.5;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.Truck;
            AcceptedGoods = [LoadType.Flour, LoadType.Paper, LoadType.Wood, LoadType.Rubber, LoadType.Wheat];

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

    public sealed class LiquidTruck : Transport
    {
        #region Constructor
        public LiquidTruck(int x, int y, Direction direction, Prouth route = null!)
        {
            //fixed fields
            TopSpeed = 0.9;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.LiquidTruck;
            AcceptedGoods = [LoadType.Oil];

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
