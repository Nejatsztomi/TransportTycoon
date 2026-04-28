using System;
using System.Collections.Generic;
using System.Data;
using System.Printing;
using System.Security.Policy;
using System.Text;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.Model;

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

    public abstract class BuildingBlocksFieldInfoViewModel : FieldInfoViewModel
    {
        public int MaxCapacity { get; init; }
        public int CurrentCapacity { get; init; }
        public int Productivity { get; init; }
        public int Scaler { get; init; }
        public int Offset { get; init; }
    }

    public class HouseFieldInfoViewModel : BuildingBlocksFieldInfoViewModel { }

    public class SiteFieldInfoViewModel : BuildingBlocksFieldInfoViewModel { }

    public class IndustryFieldInfoViewModel : BuildingBlocksFieldInfoViewModel
    {
        public int MaxConsumeCapacity { get; init; }
        public int ConsumeCapacity { get; init; } 
    }

    public class VehicleFieldInfoViewModel : FieldInfoViewModel
    {
        public double TopSpeed { get; init; }
        public double CurrentSpeed { get; init; }
        public string Direction { get; init; } = "";
        public List<string>? AcceptedLoads { get; init; } = [];
        public Load? CurrentLoad { get; init; }
        public int MaxCapacity { get; init; }
        public int CurrentCapacity { get; init; }
        public int Maintance { get; init; }
    }

    public static class FieldInfoFactory
    {
        public static FieldInfoViewModel CreateField(IField field)
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
                    Type = r.GetType().Name,
                    Height = r.Height,
                    X = r.X,
                    Y = r.Y,
                    RoadType = r.RoadType.ToString(),
                    InCity = r.InCity()
                },

                IBridge b => new BridgeFieldInfoViewModel
                {
                    Type = b.GetType().Name,
                    Height = b.Height,
                    X = b.X,
                    Y = b.Y,
                    BridgeType = b.BridgeType.ToString(),
                    Range = b.Range,
                    SpeedLimit = b.SpeedLimit
                },

                Stop s => new StopFieldInfoViewModel
                {
                    Type = s.GetType().Name,
                    Height = s.Height,
                    X = s.X,
                    Y = s.Y,
                    Connections = s.Connections?.Select(c => c.ToString()).ToList() ?? []
                },

                House h => new HouseFieldInfoViewModel
                {
                    Type = h.GetType().Name,
                    Height = h.Height,
                    X = h.X,
                    Y =h.Y,
                    MaxCapacity = h.BuildingEntity.MaxCapacity,
                    CurrentCapacity = h.BuildingEntity.CurrentCapacity,
                    Productivity = h.BuildingEntity.Productivity,
                    Scaler = h.BuildingEntity.Scaler,
                    Offset = h.BuildingEntity.Offset
                },

                ISite s => new SiteFieldInfoViewModel
                {
                    Type = s.GetType().Name,
                    Height = s.Height,
                    X = s.X,
                    Y = s.Y,
                    MaxCapacity = s.BuildingEntity.MaxCapacity,
                    CurrentCapacity = s.BuildingEntity.CurrentCapacity,
                    Productivity = s.BuildingEntity.Productivity,
                    Scaler = s.BuildingEntity.Scaler,
                    Offset = s.BuildingEntity.Offset
                },

                IIndustry i => new IndustryFieldInfoViewModel
                {
                    Type = i.GetType().Name,
                    Height = i.Height,
                    X = i.X,
                    Y = i.Y,
                    MaxConsumeCapacity = 0,
                    ConsumeCapacity = 0,
                    MaxCapacity = i.BuildingEntity.MaxCapacity,
                    CurrentCapacity = i.BuildingEntity.CurrentCapacity,
                    Productivity = i.BuildingEntity.Productivity,
                    Scaler = i.BuildingEntity.Scaler,
                    Offset = i.BuildingEntity.Offset
                },

                _ => new FieldInfoViewModel
                {
                    Type = field.GetType().Name,
                    Height = field.Height,
                    X = field.X,
                    Y = field.Y
                }
            };
        }
        public static FieldInfoViewModel ShowVehicle(Vehicle v,IField f)
        {
            return new VehicleFieldInfoViewModel
            {
                Type = v.GetType().Name,
                Height = f.Height,
                X = v.MapX,
                Y = v.MapY,
                TopSpeed = v.TopSpeed * 100,
                CurrentSpeed = v.CurrentSpeed * 100,
                Direction = v.Direction.ToString(),
                AcceptedLoads = v.AcceptedGoods?.Select(l => l.ToString()).ToList() ?? [],
                CurrentLoad = v.CurrentLoad,
                MaxCapacity = v.MaxCapacity,
                CurrentCapacity = v.CurrentCapacity,
                Maintance = v.Maintance
            };
        }
    }
}
