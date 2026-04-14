using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class VehicleViewModel : ViewModelBase
    {
        #region Properties
        private Vehicle Vehicle { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public double X => Vehicle.X;
        public double Y => Vehicle.Y;
        //public FieldType Type => Vehicle.Type;
        public Direction Direction => Vehicle.Direction;
        public VehicleType VehicleType => Vehicle.Type;

        public double PixelX { get; set; }
        public double PixelY { get; set; }


        public string? VehicleImagePath => VehicleType switch
        {
            VehicleType.BigBus => Direction switch
            {
                Direction.Up => "/Assets/Images/Vehicle/largeBusUp.png",
                Direction.Down => "/Assets/Images/Vehicle/largeBusDown.png",
                Direction.Left => "/Assets/Images/Vehicle/largeBusLeft.png",
                Direction.Right => "/Assets/Images/Vehicle/largeBusRight.png",
                _ => null
            },
            VehicleType.SmallBus => Direction switch
            {
                Direction.Up => "/Assets/Images/Vehicle/smallBusUp.png",
                Direction.Down => "/Assets/Images/Vehicle/smallBusDown.png",
                Direction.Left => "/Assets/Images/Vehicle/smallBusLeft.png",
                Direction.Right => "/Assets/Images/Vehicle/smallBusRight.png",
                _ => null
            },
            VehicleType.Van => "/Assets/Images/Vehicle/van.png",
            VehicleType.Pickup => "/Assets/Images/Vehicle/pickup.png",
            VehicleType.Truck => "/Assets/Images/Vehicle/truck.png",
            VehicleType.LiquidTruck => "/Assets/Images/Vehicle/liquidTruck.png",
            _ => null

        };
        #endregion
        #region Constructor
        public VehicleViewModel(Vehicle vehicle)
        {
            Vehicle = vehicle;
            DeterminePixels();

            MapX = vehicle.MapX;
            MapY = vehicle.MapY;
        }
        #endregion
        #region Public Methods
        public void RefreshVehicle(Vehicle vehicle)
        {
            Vehicle = vehicle;
            DeterminePixels();
            MapX = vehicle.MapX;
            MapY = vehicle.MapY;
            OnPropertyChanged(nameof(PixelX));
            OnPropertyChanged(nameof(PixelY));
            OnPropertyChanged(nameof(MapX));
            OnPropertyChanged(nameof(MapY));
            OnPropertyChanged(nameof(VehicleImagePath));
        }
        #endregion
        #region Private Methods
        private void DeterminePixels()
        {
            PixelX = 50 * Y;
            PixelY = 50 * X;
            switch (Direction)
            {
                case Direction.Up:
                    PixelX += 10;
                    break;
                case Direction.Down:
                    PixelX -= 10;
                    break;
                case Direction.Left:
                    PixelY -= 10;
                    break;
                case Direction.Right:
                    PixelY += 10;
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region Private event Methods
        #endregion
    }
}
