using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;
using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public class VehicleViewModel
    {
        #region Properties
        private Vehicle Vehicle { get; set; }
        public double X => Vehicle.X;
        public double Y => Vehicle.Y;
        //public FieldType Type => Vehicle.Type;
        public Direction Direction => Vehicle.Direction;
        public VehicleType VehicleType => Vehicle.Type;

        public string ImagePath { get; set; }

        public double PixelX { get; set; }
        public double PixelY { get; set; }

        #endregion
        #region Constructor
        public VehicleViewModel(Vehicle vehicle)
        {
            Vehicle = vehicle;
            ImagePath = Vehicle.Type switch
            {
                VehicleType.SmallBus => "/Assets/Images/Vehicle/smallBus.png",
                VehicleType.BigBus => "/Assets/Images/Vehicle/largeBus.png",
                _ => throw new InvalidOperationException("Unknown vehicle type.")
                //Van = 0, Pickup = 1, Truck = 2, LiquidTruck = 3,
            };
        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        #endregion
        #region Private event Methods
        #endregion
    }
}
