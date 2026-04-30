using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;
using TransportTycoon.Model;

namespace TransportTycoon.WPF.ViewModel
{
    public sealed partial class CreateGameViewModel : ViewModelViewConstraintBase
    {
        #region Properties
        #region IViewConstraint
        public override double? MinimumWidth => 800;
        public override double? MinimumHeight => 600;
        #endregion

        #region Game specific settings
        public string SaveName { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
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

        #region Events
        public event Action? BackToMainMenu;
        public event Action<GameCreationData>? CreateGame;
        #endregion

        #region Constructors
        public CreateGameViewModel() { }
        #endregion

        #region Relay commands
        [RelayCommand]
        private void OnCreateNewGame()
        {
            try
            {
                if (SaveName.Length <= 0 || string.IsNullOrWhiteSpace(SaveName))
                {
                    throw new ArgumentException("Invalid save name.");
                }
                Debug.WriteLine($"Save name: {SaveName}");
                int width = int.Parse(GameWidth);
                int height = int.Parse(GameHeight);
                Debug.WriteLine($"Game size: {width}x{height}");
                int seed = int.Parse(GameSeed);
                Debug.WriteLine($"Game seed: {seed}");
                int starterBalance = int.Parse(GameStarterBalance);
                Debug.WriteLine($"Game starter balance: {starterBalance}");
                Difficulty difficulty = (Difficulty)SelectedDifficulty;
                Debug.WriteLine($"Difficutly index: {SelectedDifficulty} which is {difficulty}");

                Debug.WriteLine($"Map generation settings:");
                Debug.WriteLine($"Biome: {SettingsBiome}");
                IBiome biome = TryConvertToBiome(SettingsBiome);
                Debug.WriteLine($"Water biome: {SettingsWaterBiome}");
                IWaterBiome waterBiome = TryConvertToWaterBiome(SettingsWaterBiome);
                float forestPercentage = SettingsForestPercentage / 100.0f;
                Debug.WriteLine($"Forest percentage: {forestPercentage}");
                int riverCount = int.Parse(SettingsRiverCount);
                Debug.WriteLine($"River count: {riverCount}");
                int minCities = int.Parse(SettingsMinCities);
                Debug.WriteLine($"Min cities: {minCities}");
                int maxCities = int.Parse(SettingsMaxCities);
                Debug.WriteLine($"Max cities: {maxCities}");
                int minStructures = int.Parse(SettingsMinStructures);
                Debug.WriteLine($"Min structures: {minStructures}");
                int maxStructures = int.Parse(SettingsMaxStructures);
                Debug.WriteLine($"Max structures: {maxStructures}");

                var settings = new MapGenerationSettings()
                {
                    Biome = biome,
                    WaterBiome = waterBiome,
                    ForestPercentage = forestPercentage,
                    RiverCount = riverCount,
                    MinCities = minCities,
                    MaxCities = maxCities,
                    MinStructure = minStructures,
                    MaxStructure = maxStructures,
                };
                var context = new MapGenerationContext(width, height, seed, settings);

                var data = new GameCreationData(context, SaveName, difficulty, starterBalance);

                CreateGame?.Invoke(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating new game: {ex.Message}");
                ShowErrorMsgBox(ex.Message);
            }
        }

        [RelayCommand]
        private void OnBackToMainMenu()
        {
            BackToMainMenu?.Invoke();
        }
        #endregion

        #region Private methods
        private void ShowErrorMsgBox(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private IWaterBiome TryConvertToWaterBiome(int num)
        {
            return num switch
            {
                0 => WaterBiomes.Normal,
                1 => WaterBiomes.Dry,
                2 => WaterBiomes.Wet,
                _ => throw new ArgumentException("Invalid water biome!")
            };
        }

        private IBiome TryConvertToBiome(int num)
        {
            return num switch
            {
                0 => Biomes.Default,
                1 => Biomes.Flat,
                2 => Biomes.Mountainous,
                _ => throw new ArgumentException("Invalid biome!")
            };
        }
        #endregion
    }
}
