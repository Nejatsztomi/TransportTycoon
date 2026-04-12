using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class FieldViewModel : ViewModelBase
    {
        #region Properties
        [ObservableProperty]
        private bool _isPath;
        public string ImagePath { get; set; }

        private Field Field { get; set; }
        public int X => Field.X;
        public int Y => Field.Y;
        public int Height => Field.Height;
        public int TreeCounter => Field.GetTrees();
        [ObservableProperty]
        private bool _isSelected;
        public SolidColorBrush MinimapColor
        {
            get
            {
                return Field.FieldType switch
                {
                    // Terrain
                    FieldType.Water => Brushes.Blue,
                    FieldType.Plain => Brushes.Green,
                    FieldType.Hill => Brushes.DarkGreen,
                    FieldType.Mountain => Brushes.Gray,
                    FieldType.HighMountain => Brushes.DarkGreen,

                    // Structures
                    FieldType.House => Brushes.Yellow,

                    FieldType.Farm => Brushes.LightGreen,
                    FieldType.Mine => Brushes.DarkOrange,
                    FieldType.LumberCamp => Brushes.SaddleBrown,

                    FieldType.Mill => Brushes.LightGray,
                    FieldType.Factory => Brushes.DimGray,
                    FieldType.Road => Brushes.Black,

                    // Infrastructure
                    FieldType.Bridge => Brushes.Peru,
                    FieldType.Stop => Brushes.Red,
                    //_ => throw new InvalidOperationException("Unknown field type.")
                    _ => Brushes.Magenta,
                };
            }
        }
        public string? TreeImagePath
        {
            get
            {
                if (TreeCounter == 0) return null;

                if (Field is Terrain)
                {
                    return $"/Assets/Images/Trees/tree{TreeCounter}.png";
                }
                return null;
            }
        }
        public string? InfrastructureImagePath
        {
            get
            {
                if (Field is not Infrastructure) return null;

                if (Field is Road road)
                {
                    if (road.RoadType == RoadType.RightTurn || road.RoadType == RoadType.LeftTurn || road.RoadType == RoadType.UpperRightTurn || road.RoadType == RoadType.UpperLeftTurn)
                        return $"/Assets/Images/Road/turn.png";
                    else if (road.RoadType == RoadType.UpperTRoad || road.RoadType == RoadType.RightTRoad || road.RoadType == RoadType.DownTRoad || road.RoadType == RoadType.LeftTRoad)
                        return $"/Assets/Images/Road/crossT.png";
                    else if (road.RoadType == RoadType.XRoad)
                        return $"/Assets/Images/Road/crossX.png";
                    else return $"/Assets/Images/Road/road.png";
                }
                else if (Field is Bridge bridge)
                {
                    return bridge.BridgeType switch
                    {
                        BridgeType.VerticalYellowBridge or BridgeType.HorizontalYellowBridge => $"/Assets/Images/Bridge/yellowBridge.png",
                        BridgeType.VerticalGreenBridge or BridgeType.HorizontalGreenBridge => $"/Assets/Images/Bridge/greenBridge.png",
                        BridgeType.VerticalRedBridge or BridgeType.HorizontalRedBridge => $"/Assets/Images/Bridge/redBridge.png",
                        _ => null
                    };
                }
                else if (Field is Stop) return $"/Assets/Images/Stop/stop.png";
                return null;
            }
        }
        public double InfrastructureRotation
        {
            get
            {
                if (Field is Road road)
                {
                    return road.RoadType switch
                    {
                        RoadType.Horizontal or RoadType.LeftTRoad or RoadType.LeftTurn => 90,
                        RoadType.UpperTRoad or RoadType.UpperLeftTurn => 180,
                        RoadType.RightTRoad or RoadType.UpperRightTurn => 270,
                        _ => 0
                    };
                }
                else if (Field is Bridge bridge)
                {
                    return bridge.BridgeType switch
                    {
                        BridgeType.HorizontalYellowBridge or BridgeType.HorizontalGreenBridge or BridgeType.HorizontalRedBridge => 90,
                        _ => 0
                    };
                }
                return 0;
            }
        }

        public string? StructureImage
        {
            get
            {
                return Field.FieldType switch
                {
                    FieldType.House => $"/Assets/Images/Structures/house.jpg",

                    FieldType.Farm => $"/Assets/Images/Structures/farm.png",
                    FieldType.Mine => $"/Assets/Images/Structures/oil.jpg",
                    FieldType.LumberCamp => $"/Assets/Images/Structures/lumbercamp.png",

                    FieldType.Mill => $"/Assets/Images/Structures/mill.png",
                    FieldType.Plant => $"/Assets/Images/Structures/rubber.jpg",
                    FieldType.Factory => $"/Assets/Images/Structures/factory.png",

                    _ => null
                };
            }
        }
        #endregion

        #region Constructor
        public FieldViewModel(Field field, string path)
        {
            Field = field;
            ImagePath = path;
        }
        public FieldViewModel(Field field)
        {
            Field = field;
            ImagePath = DetermineImagePath();
        }
        #endregion

        #region Public Methods
        public void RefreshTreeCount()
        {
            OnPropertyChanged(nameof(TreeCounter));
            OnPropertyChanged(nameof(TreeImagePath));
        }

        public void RefreshTerrain(Field field)
        {
            Field = field;
            ImagePath = DetermineImagePath();
            OnPropertyChanged(nameof(ImagePath));
        }
        public void RefreshInfrastructure()
        {
            OnPropertyChanged(nameof(InfrastructureRotation));
            OnPropertyChanged(nameof(InfrastructureImagePath));
        }
        #endregion

        #region Private Methods
        private string DetermineImagePath()
        {
            return Field.FieldType switch
            {
                FieldType.Plain => "Assets/Images/Terrain/field.png",
                FieldType.Hill => "Assets/Images/Terrain/hill.png",
                FieldType.Water => "Assets/Images/Terrain/water2.png",
                FieldType.Mountain => "Assets/Images/Terrain/mountain3.png",
                FieldType.HighMountain => "Assets/Images/Terrain/highmountain3.png",
                _ => "Assets/Images/Terrain/field.png"
            };
        }
        #endregion
    }
}
