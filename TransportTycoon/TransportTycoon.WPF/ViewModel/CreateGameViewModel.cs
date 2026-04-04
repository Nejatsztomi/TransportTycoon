using CommunityToolkit.Mvvm.Input;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class CreateGameViewModel : ViewModelBase
    {
        #region Private fields
        private Difficulty _selectedGameDifficulty;
        //We need an interface for Dependecy Injection
        private int _selectedDifficulty = 1;
        #endregion

        #region Events
        public event EventHandler? BackToMainMenu;
        #endregion

        #region Properties
        public int SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (_selectedDifficulty != value)
                {
                    _selectedDifficulty = value;
                    OnPropertyChanged(nameof(SelectedDifficulty));

                    _selectedGameDifficulty = (Difficulty)_selectedDifficulty;
                }
            }
        }
        #endregion

        #region Constructors
        public CreateGameViewModel() { }
        #endregion

        #region Relay commands
        [RelayCommand]
        private void OnCreateNewGame()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private void OnBackToMainMenu()
        {
            BackToMainMenu?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
