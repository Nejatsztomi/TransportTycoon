using Microsoft.Win32;
using System.Windows;
using TransportTycoon.Persistence;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private MainViewModel? _mainViewModel;
        private MainWindow? _mainView;
        #endregion

        #region Properties
        private MainViewModel MainViewModel
        {
            get => _mainViewModel ?? throw new InvalidOperationException("MainViewModel is not initialized.");
            set => _mainViewModel = value;
        }

        private MainWindow MainView
        {
            get => _mainView ?? throw new InvalidOperationException("MainWindow is not initialized.");
            set => _mainView = value;
        }
        #endregion

        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(ShowStartMenu);
        }
        #endregion

        #region Private event methods
        private void ShowStartMenu(object? _1, StartupEventArgs _2)
        {
            MainViewModel = new(JsonSaveManagerFactory.Get());

            MainViewModel.SaveGame += MainViewModel_SaveGame;
            MainViewModel.LoadGame += MainViewModel_LoadGame;

            MainView = new()
            {
                DataContext = MainViewModel,
            };
            MainView.Show();
        }

        private async void MainViewModel_LoadGame()
        {
            var fileDiag = new OpenFileDialog
            {
                Title = "Choose a save location",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                RestoreDirectory = true,
            };

            bool? result = fileDiag.ShowDialog();
            if (result != true) return;

            var uri = fileDiag.FileName;
            _mainViewModel?.LoadGameFrom(uri);
        }

        private async void MainViewModel_SaveGame(string saveName)
        {
            var folderDiag = new SaveFileDialog
            {
                Title = "Choose a save location",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "JSON files (*.json)|*.json",
                RestoreDirectory = true,
                AddExtension = true,
                OverwritePrompt = true,
                CheckPathExists = true,
                DefaultExt = "json",
                FileName = saveName
            };

            bool? result = folderDiag.ShowDialog();
            if (result != true) return;

            var uri = folderDiag.FileName;
            _mainViewModel?.SaveGameAt(uri);
        }
        #endregion
    }
}
