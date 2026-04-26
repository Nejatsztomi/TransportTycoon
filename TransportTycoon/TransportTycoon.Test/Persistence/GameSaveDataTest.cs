using System.Text.Json;
using System.Text.Json.Serialization;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class GameSaveDataTest
    {
        [Fact]
        public void CanConstructAndSetProperties()
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
                GameTime = 42UL,
                PlayerBalance = 1000,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8, new())],
                BuildingEntities = [new(9, 10, 11, 12)]
            };

            Assert.Equal(42UL, data.GameTime);
            Assert.Equal(1000, data.PlayerBalance);
            Assert.Single(data.ModifiedTiles);
            Assert.Single(data.ModifiedTrees);
            Assert.Single(data.Vehicles);
            Assert.Single(data.BuildingEntities);
        }

        [Fact]
        public void CanSerializeAndDeserialize()
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
                GameTime = 99UL,
                PlayerBalance = 1234,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8, new())],
                BuildingEntities = [new(9, 10, 11, 12)]
            };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IndentSize = 4,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new BiomeJsonConverter(),
                    new WaterBiomeJsonConverter(),
                },
            };
            var json = JsonSerializer.Serialize(data, options);
            var deserialized = JsonSerializer.Deserialize<GameSaveData>(json, options);
            Assert.NotNull(deserialized);
            Assert.Equal(data.GameTime, deserialized.GameTime);
            Assert.Equal(data.PlayerBalance, deserialized.PlayerBalance);
        }
    }
}
