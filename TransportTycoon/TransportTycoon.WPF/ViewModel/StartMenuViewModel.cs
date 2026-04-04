using CommunityToolkit.Mvvm.Input;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class StartMenuViewModel : ViewModelBase, IViewConstraints
    {
        #region Private fields
        private Difficulty _selectedGameDifficulty;
        //We need an interface for Dependecy Injection
        private int _selectedDifficulty = 1;
        #endregion

        #region Properties
        #region IViewConstraints
        public double MinimumWidth => 800;
        public double MinimumHeight => 450;
        #endregion


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

        #region Events
        public event EventHandler<Difficulty>? StartingNewGame;
        public event EventHandler<string>? LoadingGame;
        public event EventHandler? ExitingGame;
        #endregion

        #region Constructor
        public StartMenuViewModel()
        {
            _selectedGameDifficulty = (Difficulty)_selectedDifficulty;
        }
        #endregion

        #region Relay commands
        [RelayCommand]
        public void NewGame()
        {
            StartingNewGame?.Invoke(this, _selectedGameDifficulty);
        }

        [RelayCommand]
        private void LoadGame()
        {
            LoadingGame?.Invoke(this, String.Empty);
        }

        [RelayCommand]
        private void ExitGame()
        {
            ExitingGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
