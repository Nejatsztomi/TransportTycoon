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
        private Difficulty selectedGameDifficulty;
        //We need an interface for Dependecy Injection
        private int selectedDifficulty=1;
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
        public EventHandler<Difficulty> StartNewGame;
        public EventHandler<string> LoadGame;
        public event EventHandler? ExitGame;
        #endregion
        #region Constructor
        public StartViewModel() 
        {
            selectedDifficulty = 1;
            selectedGameDifficulty = (Difficulty)selectedDifficulty; 
            NewGameCommand = new RelayCommand(() =>
            {
                StartNewGame?.Invoke(this, selectedGameDifficulty);
            });

            LoadGameCommand = new RelayCommand(() =>
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
