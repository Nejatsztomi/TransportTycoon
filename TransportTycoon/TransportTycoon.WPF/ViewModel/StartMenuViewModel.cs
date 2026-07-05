using CommunityToolkit.Mvvm.Input;

namespace TransportTycoon.WPF.ViewModel
{
    public sealed partial class StartMenuViewModel : ViewModelViewConstraintBase
    {
        #region Properties
        #region IViewConstraints
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 450;
        #endregion
        #endregion

        #region Events
        public event EventHandler? StartingNewGame;
        public event Action? LoadingGame;
        public event EventHandler? ExitingGame;
        public event EventHandler? ShowGameCreationView;
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
            LoadingGame?.Invoke();
        }

        [RelayCommand]
        private void ExitGame()
        {
            ExitingGame?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void OnCreateNewGame()
        {
            ShowGameCreationView?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
