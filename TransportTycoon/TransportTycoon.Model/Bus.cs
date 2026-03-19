using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public abstract class Bus : Vehicle
    {
        #region Field
        public static LoadType AcceptedLoad { get; } = LoadType.People;
        #endregion 
    }

    public class SmallBus : Bus
    {
        #region Constructor
        public SmallBus(Prouth route, int x, int y, Direction direction)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.SmallBus;

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

    public class BigBus : Bus
    {
        #region Constructor
        public BigBus(Prouth route, int x, int y, Direction direction)
        {
            //fixed fields
            TopSpeed = 100;
            MaxCapacity = 100;
            Price = 100;
            Maintance = 100;
            Type = VehicleType.BigBus;

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
