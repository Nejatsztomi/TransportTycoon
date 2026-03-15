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
                return Field.Type switch
                {

                };
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
        #endregion

        #region Private Methods
        #endregion

        #region Private event Methods
        #endregion
    }
}
