using CommunityToolkit.Mvvm.Input;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class StartMenuViewModel : ViewModelBase, IViewConstraints
    {
        #region Properties
        #region IViewConstraints
        public double MinimumWidth => 800;
        public double MinimumHeight => 450;
        #endregion
        #endregion

        #region Events
        public event EventHandler? StartingNewGame;
        public event EventHandler<string>? LoadingGame;
        public event EventHandler? ExitingGame;
        public event EventHandler? CreateNewGame;
        #endregion

        #region Constructors
        public StartMenuViewModel() { }
        #endregion

        #region Relay commands
        [RelayCommand]
        public void NewGame()
        {
            StartingNewGame?.Invoke(this, EventArgs.Empty);
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

        [RelayCommand]
        private void OnCreateNewGame()
        {
            CreateNewGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
