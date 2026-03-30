using CommunityToolkit.Mvvm.Input;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class StartMenuViewModel : ViewModelBase, IViewConstraints
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

        #region Commands
        public RelayCommand NewGameCommand { get; }
        public RelayCommand LoadGameCommand { get; }
        public RelayCommand ExitGameCommand { get; }
        #endregion

        #region Events
        public event EventHandler<Difficulty>? StartNewGame;
        public event EventHandler<string>? LoadGame;
        public event EventHandler? ExitGame;
        #endregion

        #region Constructor
        public StartMenuViewModel()
        {
            _selectedGameDifficulty = (Difficulty)_selectedDifficulty;

            NewGameCommand = new(() =>
            {
                StartNewGame?.Invoke(this, _selectedGameDifficulty);
            });

            LoadGameCommand = new(() => LoadGame?.Invoke(this, String.Empty));

            ExitGameCommand = new(() =>
            {
                ExitGame?.Invoke(this, EventArgs.Empty);
            });
        }
        #endregion
    }
}
