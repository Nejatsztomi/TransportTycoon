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
        public int BridgePrice { get; } = 100;
        public int StopPrice { get; } = Stop.Price;
        public int PickupPrice { get; } = 600;
        public int VanPrice { get; } = 1200;
        public int TruckPrice { get; } = 1400;
        public int LiquidTruckPrice { get; } = 1800;
        public int SmallBusPrice { get; } = 500;
        public int BigBusPrice { get; } = 1200;
    }
}
