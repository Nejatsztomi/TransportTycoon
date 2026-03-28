using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Properties
        public string ImagePath { get; init; }

        private Field Field { get; init; }
        public int X => Field.X;
        public int Y => Field.Y;
        public int Height => Field.Height;
        public int TreeCounter => Field.GetTrees();
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
                    if(road.RoadType==RoadType.RightTurn || road.RoadType==RoadType.LeftTurn || road.RoadType == RoadType.UpperRightTurn || road.RoadType==RoadType.UpperLeftTurn)
                        return $"/Assets/Images/Road/turn.png";
                    else if (road.RoadType == RoadType.UpperTRoad || road.RoadType == RoadType.RightTRoad || road.RoadType == RoadType.DownTRoad || road.RoadType == RoadType.LeftTRoad)
                        return $"/Assets/Images/Road/crossT.png";
                    else if (road.RoadType == RoadType.XRoad)
                        return $"/Assets/Images/Road/crossX.png";
                    else return $"/Assets/Images/Road/road.png";
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
                        RoadType.Horizontal or RoadType.LeftTRoad or RoadType.LeftTurn=> 90,
                        RoadType.UpperTRoad or RoadType.UpperLeftTurn => 180,
                        RoadType.RightTRoad or RoadType.UpperRightTurn => 270,
                        _ => 0
                    };
                }
                return 0;
            }
        }
        #endregion

        #region Constructor
        public FieldViewModel(Field field, string imagePath)
        {
            Field = field;
            ImagePath = imagePath;
        }
        #endregion

        #region Public Methods
        public void RefreshTreeCount()
        {
            OnPropertyChanged(nameof(TreeCounter));
            OnPropertyChanged(nameof(TreeImagePath));
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private event Methods
        #endregion
    }
}
