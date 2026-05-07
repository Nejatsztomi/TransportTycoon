using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class JsonSaveManagerTest
    {
        [Fact]
        public async Task SaveGame_CreatesFile_AndCanLoadItBack()
        {
            // Arrange
            var saveManager = JsonSaveManagerFactory.Get();
            var tempFile = Path.GetTempFileName();
            try
            {
                var settings = new MapGenerationSettings();
                var data = new GameSaveData
                {
                    MapContextData = new(
                        width: 10,
                        height: 10,
                        seed: 1,
                        settings: settings
                    ),
                    GameTime = 1234UL,
                    PlayerBalance = 5678,
                    ModifiedTiles = [new(1, 2, SaveFieldType.Road, 1)],
                    ModifiedTrees = [new(3, 4, 10)],
                    Vehicles = [new(VehicleType.Van, 5, 6, LoadType.Wheat, 20, 0d, new([]))],
                    BuildingEntities = [new(7, 8, 30)]
                };

                // Act
                await saveManager.SaveGame(tempFile, data);
                var loaded = await saveManager.LoadGame(tempFile);

                // Assert
                Assert.NotNull(loaded);
                Assert.Equal(data.GameTime, loaded!.GameTime);
                Assert.Equal(data.PlayerBalance, loaded.PlayerBalance);
                Assert.Single(loaded.ModifiedTiles);
                Assert.Single(loaded.ModifiedTrees);
                Assert.Single(loaded.Vehicles);
                Assert.Single(loaded.BuildingEntities);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task LoadGame_ThrowsFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var saveManager = JsonSaveManagerFactory.Get();
            var nonExistentFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => saveManager.LoadGame(nonExistentFile));
        }

        [Fact]
        public async Task SaveGame_OverwritesExistingFile()
        {
            // Arrange
            var saveManager = JsonSaveManagerFactory.Get();
            var tempFile = Path.GetTempFileName();
            try
            {
                var settings = new MapGenerationSettings();
                var data1 = new GameSaveData
                {
                    MapContextData = new(10, 10, 1, settings),
                    GameTime = 1,
                    PlayerBalance = 2
                };
                var data2 = new GameSaveData
                {
                    MapContextData = new(10, 10, 1, settings),
                    GameTime = 3,
                    PlayerBalance = 4
                };

                // Act
                await saveManager.SaveGame(tempFile, data1);
                await saveManager.SaveGame(tempFile, data2);
                var loaded = await saveManager.LoadGame(tempFile);

                // Assert
                Assert.NotNull(loaded);
                Assert.Equal(3UL, loaded!.GameTime);
                Assert.Equal(4, loaded.PlayerBalance);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
