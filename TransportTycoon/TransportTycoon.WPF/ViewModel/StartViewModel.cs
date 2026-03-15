using CommunityToolkit.Mvvm.Input;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class StartViewModel : ViewModelBase
    {
        #region Fields
        private Difficulty selectedGameDifficulty;
        //We need an interface for Dependecy Injection
        private int selectedDifficulty = 1;
        public int SelectedDifficulty
        {
            get => selectedDifficulty;
            set
            {
                if (selectedDifficulty != value)
                {
                    selectedDifficulty = value;
                    OnPropertyChanged(nameof(SelectedDifficulty));

                    selectedGameDifficulty = (Difficulty)selectedDifficulty;
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand NewGameCommand { get; set; }
        public RelayCommand LoadGameCommand { get; set; }
        public RelayCommand ExitGameCommand { get; set; }

        #endregion

        #region Events
        public event EventHandler<Difficulty>? StartNewGame;
        public event EventHandler<string>? LoadGame;
        public event EventHandler? ExitGame;
        #endregion


        #region Constructor
        public StartViewModel()
        {
            selectedGameDifficulty = (Difficulty)selectedDifficulty;

            NewGameCommand = new RelayCommand(() =>
            {
                StartNewGame?.Invoke(this, selectedGameDifficulty);
            });

            LoadGameCommand = new(() => LoadGame?.Invoke(this, String.Empty));

            ExitGameCommand = new RelayCommand(() =>
            {
                ExitGame?.Invoke(this, EventArgs.Empty);
            });

            
        }
        #endregion
    }
}
