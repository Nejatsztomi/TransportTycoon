using System.Text.Json;
using System.Text.Json.Serialization;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class GameSaveDataTest
    {
        public class EnumEnumerable<T> : TheoryData<T> where T : struct, Enum
        {
            public EnumEnumerable()
            {
                foreach (var value in Enum.GetValues<T>())
                {
                    Add(value);
                }
            }
        }

        [Theory]
        [ClassData(typeof(EnumEnumerable<Difficulty>))]
        public void CanConstructAndSetProperties(Difficulty diff)
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
                Difficulty = diff,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road, 1)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8, 9d, new([]))],
                BuildingEntities = [new(9, 10, 11)]
            };

            Assert.Equal(42UL, data.GameTime);
            Assert.Equal(1000, data.PlayerBalance);
            Assert.Equal(diff, data.Difficulty);
            Assert.Single(data.ModifiedTiles);
            Assert.Single(data.ModifiedTrees);
            Assert.Single(data.Vehicles);
            Assert.Single(data.BuildingEntities);
        }

        [Theory]
        [ClassData(typeof(EnumEnumerable<Difficulty>))]
        public void CanSerializeAndDeserialize(Difficulty diff)
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
                Difficulty = diff,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road, 1)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8, 9d, new([]))],
                BuildingEntities = [new(9, 10, 11)]
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
            Assert.Equal(data.Difficulty, deserialized.Difficulty);
        }
    }
}
