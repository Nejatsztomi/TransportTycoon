using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public class StartViewModel : ViewModelBase
    {
        //We need an interface for Dependecy Injection

        public Difficulty SelectedDifficulty { get; set; }
        public RelayCommand NewGameCommand { get; private set; }
        public RelayCommand OpenGameCommand { get; private set; }
    }
}
