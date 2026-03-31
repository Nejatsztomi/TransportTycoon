using CommunityToolkit.Mvvm.ComponentModel;
using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class FieldViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Properties
        public string ImagePath { get; set; }

        private Field Field { get; set; }
        public int X => Field.X;
        public int Y => Field.Y;
        public int Height => Field.Height;
        public int TreeCounter => Field.GetTrees();
        [ObservableProperty]
        private bool _isSelected;
        public string MinimapColor
        {
            get
            {
                return Field.FieldType switch
                {
                    FieldType.Water => "Blue",
                    FieldType.Plain => "Green",
                    FieldType.Hill => "DarkGreen",
                    FieldType.Mountain => "Gray",
                    FieldType.HighMountain => "DarkGray",
                    FieldType.House => "Yellow",
                    FieldType.Farm => "LightGreen",
                    FieldType.Mine => "DarkYellow",
                    FieldType.LumberCamp => "SaddleBrown",
                    FieldType.Mill => "LightGray",
                    FieldType.Factory => "DimGray",
                    FieldType.Road => "Black",
                    FieldType.Bridge => "Peru",
                    FieldType.Stop => "Red",
                    _ => throw new InvalidOperationException("Unknown field type.")
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
                        BridgeType.VerticalBlueBridge or BridgeType.HorizontalBlueBridge => $"/Assets/Images/Bridge/blueBridge.png",
                        BridgeType.VerticalRedBridge or BridgeType.HorizontalRedBridge => $"/Assets/Images/Bridge/redBridge.png",
                        _ => null
                    };
                }
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
                else if(Field is Bridge bridge)
                {
                    return bridge.BridgeType switch
                    {
                        BridgeType.VerticalYellowBridge or BridgeType.VerticalBlueBridge or BridgeType.VerticalRedBridge => 90,
                        _ => 0
                    };
                }
                return 0;
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

        #region Private event Methods
        #endregion
    }
}
