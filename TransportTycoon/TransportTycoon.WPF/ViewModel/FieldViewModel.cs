using TransportTycoon.MapData;

namespace TransportTycoon.WPF.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        #region Fields
        private Field _field;
        #endregion
        #region Properties
        public int X => _field.X;
        public int Y => _field.Y;
        public int Height => _field.Height;
        public int TreeCounter
        {
            get
            {
                if (_field is Terrain terrain)
                {
                    return terrain.Trees;
                }
                else return 0;
            }
        }

        #endregion
        #region Constructor
        public FieldViewModel(Field field)
        {
            _field = field;
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
