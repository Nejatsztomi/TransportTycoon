using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class StartViewModel : ViewModelBase
    {
        #region Fields
        //We need an interface for Dependecy Injection
        private int selectedDifficulty;
        public int SelectedDifficulty 
        {

        }
        #endregion
        #region Commands
        public RelayCommand NewGameCommand { get; set; }
        public RelayCommand OpenGameCommand { get; set; }
        public RelayCommand ExitGameCommand { get; set; }

        #endregion
        #region Events
        public EventHandler<Difficulty> StartNewGame;
        public EventHandler<string> LoadGame;
        public event EventHandler? ExitGame;
        #endregion
        #region Constructor
        public StartViewModel() 
        {
            SelectedDifficulty = Difficulty.Medium;

            NewGameCommand = new RelayCommand(() =>
            {
                StartNewGame?.Invoke(this, SelectedDifficulty);
            });

            OpenGameCommand = new RelayCommand(() =>
            {
                throw new NotImplementedException();
            });
            ExitGameCommand = new RelayCommand(() =>
            {
                ExitGame?.Invoke(this, EventArgs.Empty);
            });
        }
        #endregion

    }
}
