using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class PriceViewModel : ViewModelBase
    {
        public int TerrainPrice { get; } = Terrain.Price;
        public int RoadPrice { get; } = Road.Price;
        public int BridgePrice { get; } = 80;
        public int StopPrice { get; } = Stop.Price;
        public int PickupPrice { get; } = Pickup.Price;
        public int VanPrice { get; } = Van.Price;
        public int TruckPrice { get; } = Truck.Price;
        public int LiquidTruckPrice { get; } = LiquidTruck.Price;
        public int SmallBusPrice { get; } = SmallBus.Price;
        public int BigBusPrice { get; } = BigBus.Price;
    }
}
