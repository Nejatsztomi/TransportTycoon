using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using ITimer = TransportTycoon.Model.ITimer;

namespace TransportTycoon.Test.Model
{
    public class GameModelPersistenceTests
    {
        [Fact]
        public void GameModel_CanBeRestored_FromGameSaveData()
        {
            // Arrange: create a model, modify state, save
            var context = new MapGenerationContext(5, 5, 42, new MapGenerationSettings());
            var table = new GameTable(new DummyMapGenerator(), context);
            table.GenerateMap();
            var timer = Substitute.For<ITimer>();
            var creationData = new GameCreationData(context, "Save1", TransportTycoon.Model.Difficulty.Hard, 12345);
            var model = new GameModel(table, timer, creationData)
            {
                Mode = GameMode.Editor
            };
            model.IncreaseHeight(1, 1);
            model.BuildRoad(2, 2);
            model.BuildStop(2, 3);
            model.BuyVehicle(2, 3, TransportTycoon.Model.VehicleType.Van);
            var saveData = model.GetGameSaveData();

            // Act: restore from save
            var restoredTable = new GameTable(new DummyMapGenerator(), context);
            restoredTable.GenerateMap();
            var restored = new GameModel(restoredTable, timer, saveData, "Save1");

            // Assert: basic state
            Assert.Equal(model.Balance, restored.Balance);
            Assert.Equal(model.Difficulty, restored.Difficulty);
            Assert.Equal(model.GameTime, restored.GameTime);
            Assert.Equal(model.Vehicles.Count, restored.Vehicles.Count);
            Assert.Equal(model.Map.Width, restored.Map.Width);
            Assert.Equal(model.Map.Height, restored.Map.Height);
            // Check a modified tile
            Assert.Equal(model.Map[1, 1].Height, restored.Map[1, 1].Height);
            Assert.IsType<Road>(restored.Map[2, 2]);
            Assert.IsType<Stop>(restored.Map[2, 3]);
        }

        [Fact]
        public void GameModel_GetGameSaveData_RoundTripRestoresState()
        {
            // Arrange
            var context = new MapGenerationContext(4, 4, 99, new MapGenerationSettings());
            var table = new GameTable(new DummyMapGenerator(), context);
            table.GenerateMap();
            var timer = Substitute.For<ITimer>();
            var creationData = new GameCreationData(context, "Save2", TransportTycoon.Model.Difficulty.Medium, 2222);
            var model = new GameModel(table, timer, creationData)
            {
                Mode = TransportTycoon.Model.GameMode.Editor
            };
            model.IncreaseHeight(0, 0);
            model.BuildRoad(1, 1);
            model.BuildStop(1, 2);
            model.BuyVehicle(1, 2, TransportTycoon.Model.VehicleType.Truck);
            var saveData = model.GetGameSaveData();

            // Act
            var restoredTable = new GameTable(new DummyMapGenerator(), context);
            restoredTable.GenerateMap();
            var restored = new GameModel(restoredTable, timer, saveData, "Save2");

            // Assert
            Assert.Equal(model.Balance, restored.Balance);
            Assert.Equal(model.Difficulty, restored.Difficulty);
            Assert.Equal(model.GameTime, restored.GameTime);
            Assert.Equal(model.Vehicles.Count, restored.Vehicles.Count);
            Assert.Equal(model.Map[0, 0].Height, restored.Map[0, 0].Height);
            Assert.IsType<Road>(restored.Map[1, 1]);
            Assert.IsType<Stop>(restored.Map[1, 2]);
        }

        // DummyMapGenerator returns a plain terrain grid
        private class DummyMapGenerator : IMapGenerator
        {
            public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context)
            {
                var fields = new IField[context.Width, context.Height];
                for (int i = 0; i < context.Width; i++)
                    for (int j = 0; j < context.Height; j++)
                        fields[i, j] = new Terrain(i, j, 1);
                return (fields, new List<BuildingEntity>());
            }
        }
    }
}
