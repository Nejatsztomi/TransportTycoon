using System;
using System.Collections.Generic;
using System.Data;
using System.Printing;
using System.Security.Policy;
using System.Text;
using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class FieldInfoViewModel : ViewModelBase
    {
        public string Type { get; init; } = "";
        public int Height { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
    }

    public class TerrainFieldInfoViewModel : FieldInfoViewModel
    {
        public int TreeCount { get; init; }
    }

    public class RoadFieldInfoViewmodel : FieldInfoViewModel
    {
        public string RoadType { get; init; } = "";
        public bool InCity { get; init; }
    }

    public class BridgeFieldInfoViewModel : FieldInfoViewModel
    {
        public string BridgeType { get; init; } = "";
        public int Range { get; init; }
        public int SpeedLimit { get; init; }
    }

    public class StopFieldInfoViewModel : FieldInfoViewModel
    {
        public List<string?> Connections { get; init; } = [];
    }

    public static class FieldInfoFactory
    {
        public static FieldInfoViewModel Create(IField field)
        {
            return field switch
            {
                Terrain t => new TerrainFieldInfoViewModel
                {
                    Type = t.TerrainType.ToString(),
                    Height = t.Height,
                    X = t.X,
                    Y = t.Y,
                    TreeCount = t.Trees
                },

                Road r => new RoadFieldInfoViewmodel
                {
                    Type = "Road",
                    Height = r.Height,
                    X = r.X,
                    Y = r.Y,
                    RoadType = r.RoadType.ToString(),
                    InCity = r.InCity()
                },

                IBridge b => new BridgeFieldInfoViewModel
                {
                    Type = "Bridge",
                    Height = b.Height,
                    X = b.X,
                    Y = b.Y,
                    BridgeType = b.BridgeType.ToString(),
                    Range = b.Range,
                    SpeedLimit = b.SpeedLimit
                },

                Stop s => new StopFieldInfoViewModel
                {
                    Type = "Stop",
                    Height = s.Height,
                    X = s.X,
                    Y = s.Y,
                    Connections = s.Connections?.Select(c => c.ToString()).ToList() ?? []
                },

                _ => new FieldInfoViewModel
                {
                    Type = "Water",
                    Height = field.Height,
                    X = field.X,
                    Y = field.Y
                }
            };
        }
    }
}
