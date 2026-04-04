using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class CreateGameViewModel : ViewModelViewConstraintBase
    {
        #region Private fields
        #endregion

        #region Events
        public event EventHandler? BackToMainMenu;
        #endregion

        #region Properties
        #region IViewConstraint
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 600;
        #endregion

        #region Game specific settings
        public string SaveName { get; set; } = string.Empty;
        public string GameHeight { get; set; } = "100";
        public string GameWidth { get; set; } = "100";
        public string GameSeed { get; set; } = "42";
        public string GameStarterBalance { get; set; } = "1000000";
        public int SelectedDifficulty { get; set; } = (int)GameModel.DefaultDifficulty;
        #endregion

        #region Map generation settings
        public int SettingsBiome { get; set; } = 0;
        public int SettingsWaterBiome { get; set; } = 0;
        public float SettingsForestPercentage { get; set; } = 0.2f;
        public string SettingsRiverCount { get; set; } = "3";
        public string SettingsMinCities { get; set; } = "5";
        public string SettingsMaxCities { get; set; } = "10";
        public string SettingsMinStructures { get; set; } = "5";
        public string SettingsMaxStructures { get; set; } = "15";
        #endregion
        #endregion

        #region Constructors
        public CreateGameViewModel()
        { }
        #endregion

        #region Relay commands
        [RelayCommand]
        private void OnCreateNewGame()
        {
            Debug.WriteLineIf(SaveName == String.Empty, "No save name provided.");
            Debug.WriteLineIf(SaveName != String.Empty, $"Save name: {SaveName}");
            Debug.WriteLine($"Game size: {GameWidth}x{GameHeight}");
            Debug.WriteLine($"Game seed: {GameSeed}");
            Debug.WriteLine($"Game starter balance: {GameStarterBalance}");
            Debug.WriteLine($"Difficutly index: {SelectedDifficulty} which is {(Difficulty)SelectedDifficulty}");

            Debug.WriteLine($"Map generation settings:");
            Debug.WriteLine($"Biome: {SettingsBiome}");
            Debug.WriteLine($"Water biome: {SettingsWaterBiome}");
            Debug.WriteLine($"Forest percentage: {SettingsForestPercentage}");
            Debug.WriteLine($"River count: {SettingsRiverCount}");
            Debug.WriteLine($"Min cities: {SettingsMinCities}");
            Debug.WriteLine($"Max cities: {SettingsMaxCities}");
            Debug.WriteLine($"Min structures: {SettingsMinStructures}");
            Debug.WriteLine($"Max structures: {SettingsMaxStructures}");
        }

        [RelayCommand]
        private void OnBackToMainMenu()
        {
            BackToMainMenu?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
