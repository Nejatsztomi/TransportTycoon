using System.Data;
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

    public class RoadFieldInfoViewModel : FieldInfoViewModel
    {
        public string RoadType { get; init; } = "";
        public bool InCity { get; init; }
    }

    public class BridgeFieldInfoViewModel : FieldInfoViewModel
    {
        public string BridgeType { get; init; } = "";
        public int Range { get; init; }
        public double SpeedLimit { get; init; }
    }

    public class StopFieldInfoViewModel : FieldInfoViewModel
    {
        public List<string> Connections { get; init; } = [];
    }

    public abstract class BuildingBlocksFieldInfoViewModel : FieldInfoViewModel
    {
        public int MaxCapacity { get; init; }
        public double CurrentCapacity { get; init; }
        public double Productivity { get; init; }
    }

    public class HouseFieldInfoViewModel : BuildingBlocksFieldInfoViewModel { }

    public class SiteFieldInfoViewModel : BuildingBlocksFieldInfoViewModel { }

    public class IndustryFieldInfoViewModel : BuildingBlocksFieldInfoViewModel
    {
        public int MaxConsumeCapacity { get; init; }
        public double ConsumeCapacity { get; init; }
    }

    public class VehicleFieldInfoViewModel : FieldInfoViewModel
    {
        public double TopSpeed { get; init; }
        public double CurrentSpeed { get; init; }
        public double Direction { get; init; }
        public List<string>? AcceptedLoads { get; init; } = [];
        public string? CurrentLoad { get; init; }
        public int MaxCapacity { get; init; }
        public double CurrentCapacity { get; init; }
        public double Maintenance { get; init; }
    }

    public static class FieldInfoFactory
    {
        public static FieldInfoViewModel CreateField(Field field)
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

                Road r => new RoadFieldInfoViewModel
                {
                    Type = r.GetType().Name,
                    Height = r.Height,
                    X = r.X,
                    Y = r.Y,
                    RoadType = r.RoadType.ToString(),
                    InCity = r.InCity()
                },

                Bridge b => new BridgeFieldInfoViewModel
                {
                    Type = b.GetType().Name,
                    Height = b.Height,
                    X = b.X,
                    Y = b.Y,
                    BridgeType = b.BridgeType.ToString().Contains("Vertical") ? "Vertical" : "Horizontal",
                    Range = b.Range,
                    SpeedLimit = b.SpeedLimit
                },

                Stop s => new StopFieldInfoViewModel
                {
                    Type = s.GetType().Name,
                    Height = s.Height,
                    X = s.X,
                    Y = s.Y,
                    Connections = s.Connections?.Select(c => c.GetType().Name).ToList() ?? []
                },

                House h => new HouseFieldInfoViewModel
                {
                    Type = h.GetType().Name,
                    Height = h.Height,
                    X = h.X,
                    Y = h.Y,
                    MaxCapacity = h.BuildingEntity.MaxCapacity,
                    CurrentCapacity = Math.Round(h.BuildingEntity.CurrentCapacity),
                    Productivity = h.BuildingEntity.MaxCapacity > Math.Round(h.BuildingEntity.CurrentCapacity) ? Math.Round(h.BuildingEntity.Productivity, 2) : 0,
                },

                Site s => new SiteFieldInfoViewModel
                {
                    Type = s.GetType().Name,
                    Height = s.Height,
                    X = s.X,
                    Y = s.Y,
                    MaxCapacity = s.BuildingEntity.MaxCapacity,
                    CurrentCapacity = Math.Round(s.BuildingEntity.CurrentCapacity),
                    Productivity = s.BuildingEntity.MaxCapacity > Math.Round(s.BuildingEntity.CurrentCapacity) ? Math.Round(s.BuildingEntity.Productivity, 2) : 0,
                },

                Industry i => new IndustryFieldInfoViewModel
                {
                    Type = i.GetType().Name,
                    Height = i.Height,
                    X = i.X,
                    Y = i.Y,
                    MaxConsumeCapacity = ((IndustryEntity)i.BuildingEntity).MaxConsumeCapacity,
                    ConsumeCapacity = Math.Round(((IndustryEntity)i.BuildingEntity).ConsumeCapacity),
                    MaxCapacity = i.BuildingEntity.MaxCapacity,
                    CurrentCapacity = Math.Round(i.BuildingEntity.CurrentCapacity),
                    Productivity = Math.Round(((IndustryEntity)i.BuildingEntity).ConsumeCapacity) > 0
                                   ? i.BuildingEntity.MaxCapacity > Math.Round(i.BuildingEntity.CurrentCapacity)
                                        ? Math.Round(i.BuildingEntity.Productivity, 2) : 0
                                   : 0,
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
        public static FieldInfoViewModel ShowVehicle(Vehicle v, Field f)
        {
            return new VehicleFieldInfoViewModel
            {
                Type = v.GetType().Name,
                Height = f.Height,
                X = v.MapX,
                Y = v.MapY,
                TopSpeed = v.TopSpeed * 100,
                CurrentSpeed = v.CurrentSpeed * 100,
                Direction = v.Angle,
                AcceptedLoads = v.AcceptedGoods?.Select(l => l.GetType().Name).ToList() ?? [],
                CurrentLoad = v.CurrentLoad?.GetType().Name,
                MaxCapacity = v.MaxCapacity,
                CurrentCapacity = v.CurrentCapacity,
                Maintenance = v.Maintenance
            };
        }
    }
}
