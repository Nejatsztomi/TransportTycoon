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
        public Difficulty SelectedDifficulty { get; set; }
        #endregion
        #region Commands
        public RelayCommand NewGameCommand { get; set; }
        public RelayCommand OpenGameCommand { get; set; }
        public RelayCommand SetEasyMode { get; set; }
        public RelayCommand SetMediumMode { get; set; }
        public RelayCommand SetHardMode { get; set; }
        #endregion
        #region Events
        public EventHandler<Difficulty> StartNewGame;
        public EventHandler<string> LoadGame;
        #endregion
        #region Constructor
        public StartViewModel() 
        {

        }
        #endregion

    }
}
