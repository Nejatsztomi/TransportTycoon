using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

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
        public event EventHandler<string>? LoadingGame;
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
            var fileDiag = new OpenFileDialog
            {
                Title = "Choose a save location",
                Filter = "JSON files|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                RestoreDirectory = true,
            };

            bool? result = fileDiag.ShowDialog();
            if (result is null || result == false) return;

            var uri = fileDiag.FileName;
            LoadingGame?.Invoke(this, uri);
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
