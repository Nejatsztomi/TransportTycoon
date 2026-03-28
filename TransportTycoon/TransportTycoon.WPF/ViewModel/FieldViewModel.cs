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

        internal void RefreshTerrain()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private event Methods
        #endregion
    }
}
