using System.Text.Json;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class GameSaveDataTest
    {
        [Fact]
        public void CanConstructAndSetProperties()
        {
            var data = new GameSaveData
            {
                GameTime = 42UL,
                PlayerBalance = 1000,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8)],
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
            var data = new GameSaveData
            {
                GameTime = 99UL,
                PlayerBalance = 1234,
                ModifiedTiles = [new(1, 2, SaveFieldType.Road)],
                ModifiedTrees = [new(3, 4, 5)],
                Vehicles = [new(VehicleType.Van, 6, 7, LoadType.Wheat, 8)],
                BuildingEntities = [new(9, 10, 11, 12)]
            };
            var json = JsonSerializer.Serialize(data);
            var deserialized = JsonSerializer.Deserialize<GameSaveData>(json);
            Assert.NotNull(deserialized);
            Assert.Equal(data.GameTime, deserialized.GameTime);
            Assert.Equal(data.PlayerBalance, deserialized.PlayerBalance);
        }
    }
}
