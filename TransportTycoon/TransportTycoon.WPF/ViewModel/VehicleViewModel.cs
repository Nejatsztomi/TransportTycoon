using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

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
        public VehicleType VehicleType => Vehicle.VehicleType;

        public string ImagePath { get; set; }

        public double PixelX { get; set; }
        public double PixelY { get; set; }

        #endregion
        #region Constructor
        public VehicleViewModel(Vehicle vehicle)
        {
            Vehicle = vehicle;
            ImagePath = Vehicle.VehicleType switch
            {
                VehicleType.Truck => "/Assets/Images/Vehicles/truck.png",
                VehicleType.Train => "/Assets/Images/Vehicles/train.png",
                VehicleType.Ship => "/Assets/Images/Vehicles/ship.png",
                _ => throw new InvalidOperationException("Unknown vehicle type.")
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
