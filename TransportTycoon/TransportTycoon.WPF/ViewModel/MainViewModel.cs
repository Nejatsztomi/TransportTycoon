using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        #region Relay commands
        public RelayCommand NewGameCommand { get; init; }
        public RelayCommand ExitCommand { get; init; }

        public RelayCommand NormalSpeedCommand { get; init; }
        public RelayCommand FastSpeedCommand { get; init; }
        public RelayCommand SuperFastSpeedCommand { get; init; }

        public RelayCommand PauseGameCommand { get; init; }
        public RelayCommand ResumeGameCommand { get; init; }
        public RelayCommand EditorModeCommand { get; init; }
        #endregion

        public GameModel Model { get; init; }

        public ObservableCollection<FieldViewModel> Tiles { get; init; }

        public int Balance => Model.Balance;
        public int GameTime => Model.GameTime;
        public bool IsPaused => Model.Mode == GameMode.Paused;
        public bool IsEditorMode => Model.Mode == GameMode.Editor;
        #endregion

        #region Events
        #endregion

        #region Constructors
        public MainViewModel(GameModel model)
        {
            Model = model;

            NewGameCommand = new(OnNewGame);
            ExitCommand = new(OnExit);
            NormalSpeedCommand = new(OnNormalSpeed);
            FastSpeedCommand = new(OnFastSpeed);
            SuperFastSpeedCommand = new(OnSuperFastSpeed);

            PauseGameCommand = new(OnPauseGame);
            ResumeGameCommand = new(OnResumeGame);
            EditorModeCommand = new(OnEditorMode);

            Tiles = [];
            RefreshTable();
        }
        #endregion

        #region Private methods
        private void RefreshTable()
        {
            Tiles.Clear();
            for (int x = 0; x < Model.Map.Width; x++)
            {
                for (int y = 0; y < Model.Map.Height; y++)
                {
                    //FieldViewModel tile = new(Model.Map[x, y]);
                    FieldViewModel tile = new();
                    Tiles.Add(tile);
                }
            }
        }
        #endregion

        #region Relay command methods
        private void OnNewGame()
        {
            Model.NewGame();
        }

        private void OnExit()
        {
            throw new NotImplementedException();
        }

        private void OnNormalSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.Normal);
        }

        private void OnFastSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.Fast);
        }

        private void OnSuperFastSpeed()
        {
            Model.SetTimeSpeed(TimeSpeed.SuperFast);
        }

        private void OnPauseGame()
        {
            Model.SetMode(GameMode.Paused);
        }

        private void OnResumeGame()
        {
            Model.SetMode(GameMode.Run);
        }

        private void OnEditorMode()
        {
            Model.SetMode(GameMode.Editor);
        }
        #endregion

        #region Event methods
        #endregion
    }
}
