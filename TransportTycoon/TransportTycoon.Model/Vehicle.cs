using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public enum Direction
    {
        Up = 0, Down = 1, Left = 2, Right = 3
    }

    public enum VehicleType
    {
        Van = 0, Pickup = 1, Truck = 2, LiquidTruck = 3, SmallBus = 4, BigBus = 5
    }

    public abstract class Vehicle
    {
        #region Fields
        public double TopSpeed { get; protected set; }
        public double CurrentSpeed { get; protected set; }
        public Load? CurrentLoad { get; protected set; }
        public int MaxCapacity { get; protected set; }
        public int CurrentCapacity { get; protected set; }
        public Prouth? Route { get; protected set; }
        public VehicleType Type { get; protected set; }
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public Direction Direction { get; protected set; }
        public int Price { get; protected set; }
        public int Maintance { get; protected set; }

        public int MapX => (int)Math.Round(X);
        public int MapY => (int)Math.Round(Y);
        public List<LoadType>? AcceptedGoods { get; protected set; } = new List<LoadType>();
        #endregion

        #region Public methods
        public void Step(Direction dir = Direction.Up)
        {
            switch (dir)
            {
                case Direction.Up:
                    X -= CurrentSpeed;
                    Direction = Direction.Up;
                    break;
                case Direction.Down:
                    X += CurrentSpeed;
                    Direction = Direction.Down;
                    break;
                case Direction.Left:
                    Y -= CurrentSpeed;
                    Direction = Direction.Left;
                    break;
                case Direction.Right:
                    Y += CurrentSpeed;
                    Direction = Direction.Right;
                    break;
                default:
                    break;
            }
        }

        public void SetCurrentCapacity(int quantity)
        {
            if (quantity >= 0 && quantity <= MaxCapacity) CurrentCapacity = quantity;

            if(CurrentCapacity==0) CurrentLoad = null;
        }

        /// <summary>
        /// Changes the current speed of the vehicle, if the given speed is between 0 and the top speed of the vehicle
        /// </summary>
        /// <param name="speed"></param>
        public void ChangeCurrentSpeed(double speed)
        {
            if (speed >= 0 && speed <= TopSpeed) CurrentSpeed = speed;
        }

        
        public int Load(int quantity, Load load) //returns leftover
        {
            if (CurrentLoad != load) return quantity;
            else if (CurrentLoad == null)
            {
                CurrentLoad = load;
                if (quantity <= MaxCapacity)
                {
                    CurrentCapacity = quantity;
                    return 0;
                }
                else
                {
                    CurrentCapacity = MaxCapacity;
                    return quantity - MaxCapacity;
                }
            }
            else
            {
                if (CurrentCapacity + quantity <= MaxCapacity)
                {
                    CurrentCapacity = quantity;
                    return 0;
                }
                else
                {
                    CurrentCapacity = MaxCapacity;
                    return quantity - (MaxCapacity - CurrentCapacity);
                }
            }
        }

        public int UnLoad(int quantity, Load load) //returns unloaded quantity
        {
            if (CurrentLoad == null || CurrentLoad != load) return 0;
            else if (quantity < CurrentCapacity)
            {
                CurrentCapacity -= quantity;
                return quantity;
            }
            else if (quantity == CurrentCapacity)
            {
                CurrentCapacity -= quantity;
                CurrentLoad = null;
                return quantity;
            }
            else if (quantity > CurrentCapacity)
            {
                int tmp = CurrentCapacity;
                CurrentCapacity = 0;
                CurrentLoad = null;
                return tmp;
            }
            return 0;
        }
        #endregion
    }
}
