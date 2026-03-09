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
        public int TreeCounter => _field.GetTrees();

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
