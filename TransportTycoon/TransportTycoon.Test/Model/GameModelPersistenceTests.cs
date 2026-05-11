using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using ITimer = TransportTycoon.Model.ITimer;
using TPersistence = global::TransportTycoon.Persistence;

namespace TransportTycoon.Test.Model
{
    public class GameModelPersistenceTests
    {
        [Fact]
        public void GameModel_CanBeRestored_FromGameSaveData()
        {
            var context = new MapGenerationContext(5, 5, 42, new MapGenerationSettings());
            var (model, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "Save1", TransportTycoon.Model.Difficulty.Hard, 12345, game =>
            {
                game.IncreaseHeight(1, 1);
                game.BuildRoad(2, 2);
                game.BuildStop(2, 3);
                game.BuyVehicle(2, 3, TransportTycoon.Model.VehicleType.Van);
            });

            Assert.Equal(model.Balance, restored.Balance);
            Assert.Equal(model.Difficulty, restored.Difficulty);
            Assert.Equal(model.GameTime, restored.GameTime);
            Assert.Equal(model.Vehicles.Count, restored.Vehicles.Count);
            Assert.Equal(model.Map.Width, restored.Map.Width);
            Assert.Equal(model.Map.Height, restored.Map.Height);
            Assert.Equal(model.Map[1, 1].Height, restored.Map[1, 1].Height);
            Assert.IsType<Road>(restored.Map[2, 2]);
            Assert.IsType<Stop>(restored.Map[2, 3]);
        }

        [Fact]
        public void GameModel_GetGameSaveData_RoundTripRestoresState()
        {
            var context = new MapGenerationContext(4, 4, 99, new MapGenerationSettings());
            var (model, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "Save2", TransportTycoon.Model.Difficulty.Medium, 2222, game =>
            {
                game.IncreaseHeight(0, 0);
                game.BuildRoad(1, 1);
                game.BuildStop(1, 2);
                game.BuyVehicle(1, 2, TransportTycoon.Model.VehicleType.Truck);
            });

            Assert.Equal(model.Balance, restored.Balance);
            Assert.Equal(model.Difficulty, restored.Difficulty);
            Assert.Equal(model.GameTime, restored.GameTime);
            Assert.Equal(model.Vehicles.Count, restored.Vehicles.Count);
            Assert.Equal(model.Map[0, 0].Height, restored.Map[0, 0].Height);
            Assert.IsType<Road>(restored.Map[1, 1]);
            Assert.IsType<Stop>(restored.Map[1, 2]);
        }

        [Fact]
        public void GameModel_TerrainTrees_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(5, 5, 17, new MapGenerationSettings());
            var (_, restored, _) = RoundTrip(context, ctx => CreateTerrainMapWithTrees(ctx, (1, 1, 3), (3, 2, 1)), "Trees", TransportTycoon.Model.Difficulty.Easy, 1000, _ => { });

            Assert.Equal(3, restored.Map[1, 1].GetTrees());
            Assert.Equal(1, restored.Map[3, 2].GetTrees());
        }

        [Fact]
        public void GameModel_RoadAndStop_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(5, 5, 23, new MapGenerationSettings());
            var (_, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "RoadStop", TransportTycoon.Model.Difficulty.Medium, 5000, game =>
            {
                game.BuildRoad(1, 2);
                game.BuildStop(2, 2);
            });

            Assert.IsType<Road>(restored.Map[1, 2]);
            Assert.IsType<Stop>(restored.Map[2, 2]);
        }

        [Theory]
        [InlineData(13, 1, 2, 14, 2, typeof(YellowBridge), BridgeType.HorizontalYellowBridge)]
        [InlineData(15, 1, 4, 16, 4, typeof(GreenBridge), BridgeType.HorizontalGreenBridge)]
        [InlineData(17, 1, 6, 18, 6, typeof(RedBridge), BridgeType.HorizontalRedBridge)]
        [InlineData(13, 2, 1, 2, 14, typeof(YellowBridge), BridgeType.VerticalYellowBridge)]
        [InlineData(15, 4, 1, 4, 16, typeof(GreenBridge), BridgeType.VerticalGreenBridge)]
        [InlineData(17, 6, 1, 6, 18, typeof(RedBridge), BridgeType.VerticalRedBridge)]
        public void GameModel_BridgeTypes_CanBeSavedAndRestored(
            int distance,
            int startX,
            int startY,
            int endX,
            int endY,
            Type expectedFieldType,
            BridgeType expectedBridgeType)
        {
            var context = new MapGenerationContext(20, 20, 31, new MapGenerationSettings());
            var (_, restored, _) = RoundTrip(context, CreateBridgeMap, "Bridge", TransportTycoon.Model.Difficulty.Medium, 10000, game =>
            {
                game.SetSelectedField(startX, startY);
                game.BuildBridge(endX, endY);
            });

            Assert.IsType(expectedFieldType, restored.Map[startX, startY]);
            Assert.Equal(expectedBridgeType, ((IBridge)restored.Map[startX, startY]).BridgeType);
            Assert.IsType(expectedFieldType, restored.Map[endX, endY]);
            Assert.Equal(expectedBridgeType, ((IBridge)restored.Map[endX, endY]).BridgeType);
            Assert.True(distance > 0);
        }

        [Fact]
        public void GameModel_Vehicles_WithDifferentLoads_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(10, 2, 61, new MapGenerationSettings());
            var loads = new Load?[]
            {
                null,
                new Wheat(),
                new Oil(),
                new Wood(),
                new Flour(),
                new Rubber(),
                new Paper(),
                new People(),
            };

            var vehicleTypes = new[]
            {
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.LiquidTruck,
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.Van,
                TransportTycoon.Model.VehicleType.SmallBus,
            };

            var (_, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "Vehicles", TransportTycoon.Model.Difficulty.Easy, 25000, game =>
            {
                game.Vehicles.Clear();

                for (int i = 0; i < loads.Length; i++)
                {
                    game.Vehicles.Add(new TestVehicle(i, 0, vehicleTypes[i], loads[i]));
                }
            });

            Assert.Equal(loads.Length, restored.Vehicles.Count);
            Assert.Equal(TransportTycoon.Model.VehicleType.Van, restored.Vehicles[0].Type);
            Assert.Null(restored.Vehicles[0].CurrentLoad);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Wheat, restored.Vehicles[1].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Oil, restored.Vehicles[2].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Wood, restored.Vehicles[3].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Flour, restored.Vehicles[4].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Rubber, restored.Vehicles[5].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.Paper, restored.Vehicles[6].CurrentLoad?.LoadType);
            Assert.Equal<TransportTycoon.MapData.LoadType?>(TransportTycoon.MapData.LoadType.People, restored.Vehicles[7].CurrentLoad?.LoadType);
        }

        [Fact]
        public void GameModel_Vehicles_WithDifferentTypes_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(10, 2, 62, new MapGenerationSettings());

            var (_, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "VehicleTypes", TransportTycoon.Model.Difficulty.Easy, 25000, game =>
            {
                game.Vehicles.Clear();
                game.Vehicles.Add(new TestVehicle(0, 0, TransportTycoon.Model.VehicleType.Van, null));
                game.Vehicles.Add(new TestVehicle(1, 0, TransportTycoon.Model.VehicleType.Pickup, null));
                game.Vehicles.Add(new TestVehicle(2, 0, TransportTycoon.Model.VehicleType.Truck, null));
                game.Vehicles.Add(new TestVehicle(3, 0, TransportTycoon.Model.VehicleType.LiquidTruck, null));
                game.Vehicles.Add(new TestVehicle(4, 0, TransportTycoon.Model.VehicleType.SmallBus, null));
                game.Vehicles.Add(new TestVehicle(5, 0, TransportTycoon.Model.VehicleType.BigBus, null));
            });

            Assert.Collection(restored.Vehicles,
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.Van, vehicle.Type),
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.Pickup, vehicle.Type),
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.Truck, vehicle.Type),
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.LiquidTruck, vehicle.Type),
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.SmallBus, vehicle.Type),
                vehicle => Assert.Equal(TransportTycoon.Model.VehicleType.BigBus, vehicle.Type));
        }

        [Fact]
        public void GameModel_VehicleProuth_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(6, 6, 71, new MapGenerationSettings());
            var (_, restored, _) = RoundTrip(context, CreatePlainTerrainMap, "Prouth", TransportTycoon.Model.Difficulty.Medium, 50000, game =>
            {
                game.BuildRoad(1, 2);
                game.BuildStop(2, 2);
                game.BuildRoad(3, 2);
                game.BuildStop(4, 2);

                game.BuyVehicle(2, 2, TransportTycoon.Model.VehicleType.Van);
                game.DefineRoute(2, 2);
                game.DefineRoute(4, 2);
                game.AssignRoute(2, 2);
            });

            var restoredVehicle = Assert.Single(restored.Vehicles);
            Assert.NotNull(restoredVehicle.Prouth);
            Assert.Equal(2, restoredVehicle.Prouth!.Stops.Count);
            Assert.Equal(2, restoredVehicle.Prouth.Stops[0].X);
            Assert.Equal(2, restoredVehicle.Prouth.Stops[0].Y);
            Assert.Equal(4, restoredVehicle.Prouth.Stops[1].X);
            Assert.Equal(2, restoredVehicle.Prouth.Stops[1].Y);
        }

        [Fact]
        public void GameModel_BuildingEntities_CanBeSavedAndRestored()
        {
            var context = new MapGenerationContext(6, 6, 81, new MapGenerationSettings());
            var (_, restored, _) = RoundTrip(context, CreateBuildingMap, "Buildings", TransportTycoon.Model.Difficulty.Hard, 100000, _ => { });

            Assert.Equal(2, restored.Map.BuildingEntities.Count);
            Assert.Equal(new[] { (0, 0), (2, 2) }, restored.Map.BuildingEntities.Select(entity => entity.TopLeftPoints).ToArray());
            Assert.Equal(123, restored.Map.BuildingEntities[0].CurrentCapacity);
            Assert.Equal(456, restored.Map.BuildingEntities[1].CurrentCapacity);
        }

        [Fact]
        public void GameModel_Loading_Throws_WhenBuildingEntityLocationIsInvalid()
        {
            var context = new MapGenerationContext(6, 6, 82, new MapGenerationSettings());
            var model = CreateEditorModel(context, CreateBuildingMap, "InvalidBuildingLocation", TransportTycoon.Model.Difficulty.Hard, 100000);
            var saveData = model.GetGameSaveData();

            saveData.BuildingEntities[0] = new TPersistence.BuildingEntitySaveData(5, 5, saveData.BuildingEntities[0].CurrentCapacity);

            var table = new GameTable(new ScenarioMapGenerator(CreateBuildingMap), context);
            Assert.Throws<Exception>(() => new GameModel(table, Substitute.For<ITimer>(), saveData, "InvalidBuildingLocation"));
        }

        [Fact]
        public void GameModel_GetGameSaveData_Throws_WhenVehicleHasInvalidLoadType()
        {
            var context = new MapGenerationContext(3, 3, 91, new MapGenerationSettings());
            var model = CreateEditorModel(context, CreatePlainTerrainMap, "InvalidLoad", TransportTycoon.Model.Difficulty.Easy, 1000);

            model.Vehicles.Clear();
            model.Vehicles.Add(new TestVehicle(0, 0, TransportTycoon.Model.VehicleType.Van, new InvalidLoad()));

            var exception = Record.Exception(() => model.GetGameSaveData());
            Assert.IsType<Exception>(exception);
        }

        [Fact]
        public void GameModel_Loading_Throws_WhenVehicleTypeInSaveDataIsInvalid()
        {
            var context = new MapGenerationContext(3, 3, 101, new MapGenerationSettings());
            var saveData = new TPersistence.GameSaveData
            {
                MapContextData = new(context),
                GameTime = 0,
                PlayerBalance = 1000,
                Difficulty = TPersistence.Difficulty.Easy,
                Vehicles =
                [
                    new TPersistence.VehicleSaveData((TPersistence.VehicleType)255, 0, 0, TPersistence.LoadType.None, 0, 0.0, new TPersistence.ProuthData([]))
                ]
            };

            var table = new GameTable(new ScenarioMapGenerator(CreatePlainTerrainMap), context);
            Assert.Throws<ArgumentException>(() => new GameModel(table, Substitute.For<ITimer>(), saveData, "InvalidVehicleType"));
        }

        [Fact]
        public void GameModel_Loading_Throws_WhenVehicleLoadTypeInSaveDataIsInvalid()
        {
            var context = new MapGenerationContext(3, 3, 102, new MapGenerationSettings());
            var saveData = new TPersistence.GameSaveData
            {
                MapContextData = new(context),
                GameTime = 0,
                PlayerBalance = 1000,
                Difficulty = TPersistence.Difficulty.Easy,
                Vehicles =
                [
                    new TPersistence.VehicleSaveData(TPersistence.VehicleType.Van, 0, 0, (TPersistence.LoadType)255, 0, 0.0, new TPersistence.ProuthData([]))
                ]
            };

            var table = new GameTable(new ScenarioMapGenerator(CreatePlainTerrainMap), context);
            Assert.Throws<ArgumentException>(() => new GameModel(table, Substitute.For<ITimer>(), saveData, "InvalidVehicleLoad"));
        }

        [Fact]
        public void GameModel_Loading_IgnoresUnknownTileType_AndKeepsBaseTerrain()
        {
            var context = new MapGenerationContext(3, 3, 103, new MapGenerationSettings());
            var saveData = new TPersistence.GameSaveData
            {
                MapContextData = new(context),
                GameTime = 0,
                PlayerBalance = 1000,
                Difficulty = TPersistence.Difficulty.Easy,
                ModifiedTiles =
                [
                    new TPersistence.TileSaveData(1, 1, (TPersistence.SaveFieldType)254, 1)
                ]
            };

            var table = new GameTable(new ScenarioMapGenerator(CreatePlainTerrainMap), context);
            var model = new GameModel(table, Substitute.For<ITimer>(), saveData, "InvalidTileType");

            Assert.Equal(table[1, 1], model.Map[1, 1]);
        }

        [Fact]
        public void GameModel_GetGameSaveData_SavesNoneLoadType_ForEmptyVehicleLoad()
        {
            var context = new MapGenerationContext(3, 3, 104, new MapGenerationSettings());

            var (model, _, saveData) = RoundTrip(context, CreatePlainTerrainMap, "NoneLoad", TransportTycoon.Model.Difficulty.Easy, 1000, game =>
            {
                game.Vehicles.Clear();
                game.Vehicles.Add(new TestVehicle(0, 0, TransportTycoon.Model.VehicleType.Van, null));
            });

            Assert.Single(model.Vehicles);
            Assert.Single(saveData.Vehicles);
            Assert.Equal(TPersistence.LoadType.None, saveData.Vehicles[0].CurrentLoad);
        }

        [Fact]
        public void GameModel_Loading_IgnoresTreeData_WhenTargetTileIsNotTerrain()
        {
            var context = new MapGenerationContext(5, 5, 105, new MapGenerationSettings());
            var (_, _, saveData) = RoundTrip(context, ctx => CreateTerrainMapWithTrees(ctx, (1, 1, 3)), "TreesInvalidTarget", TransportTycoon.Model.Difficulty.Easy, 1000, _ => { });

            saveData.ModifiedTrees[0] = new TPersistence.TreeSaveData(2, 2, 3);

            var table = new GameTable(new ScenarioMapGenerator(CreateTerrainMapWithWaterAt(2, 2)), context);
            var model = new GameModel(table, Substitute.For<ITimer>(), saveData, "TreesInvalidTarget");

            Assert.IsType<Water>(model.Map[2, 2]);
            Assert.Equal(0, model.Map[2, 2].GetTrees());
        }

        [Fact]
        public void GameModel_GetGameSaveData_SavesNoneLoadType_ForNoneLoad()
        {
            var context = new MapGenerationContext(3, 3, 106, new MapGenerationSettings());
            var (model, _, saveData) = RoundTrip(context, CreatePlainTerrainMap, "NoneLoadObj", TransportTycoon.Model.Difficulty.Easy, 1000, game =>
            {
                game.Vehicles.Clear();
                // Give the vehicle an actual Load object, but with LoadType = None
                game.Vehicles.Add(new TestVehicle(0, 0, TransportTycoon.Model.VehicleType.Van, new NoneLoad()));
            });

            Assert.Single(saveData.Vehicles);
            Assert.Equal(TPersistence.LoadType.None, saveData.Vehicles[0].CurrentLoad);
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

        private sealed class NoneLoad : Load
        {
            public NoneLoad()
            {
                Price = 0;
                LoadType = TransportTycoon.MapData.LoadType.None;
            }
        }

        private sealed class ScenarioMapGenerator : IMapGenerator
        {
            private readonly Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> _generate;

            public ScenarioMapGenerator(Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> generate)
            {
                _generate = generate;
            }

            public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context) => _generate(context);
        }

        private static (GameModel Saved, GameModel Restored, TPersistence.GameSaveData SaveData) RoundTrip(
            MapGenerationContext context,
            Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> mapFactory,
            string saveName,
            TransportTycoon.Model.Difficulty difficulty,
            int balance,
            Action<GameModel> arrange)
        {
            var model = CreateEditorModel(context, mapFactory, saveName, difficulty, balance);
            arrange(model);
            var saveData = model.GetGameSaveData();
            var restored = CreateRestoredModel(context, mapFactory, saveData, saveName);
            return (model, restored, saveData);
        }

        private static GameModel CreateEditorModel(
            MapGenerationContext context,
            Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> mapFactory,
            string saveName,
            TransportTycoon.Model.Difficulty difficulty,
            int balance)
        {
            var table = new GameTable(new ScenarioMapGenerator(mapFactory), context);
            table.GenerateMap();
            var timer = Substitute.For<ITimer>();
            var creationData = new GameCreationData(context, saveName, difficulty, balance);
            return new GameModel(table, timer, creationData)
            {
                Mode = GameMode.Editor
            };
        }

        private static GameModel CreateRestoredModel(
            MapGenerationContext context,
            Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> mapFactory,
            TPersistence.GameSaveData saveData,
            string saveName)
        {
            var table = new GameTable(new ScenarioMapGenerator(mapFactory), context);
            return new GameModel(table, Substitute.For<ITimer>(), saveData, saveName);
        }

        private static (IField[,], List<BuildingEntity>) CreatePlainTerrainMap(MapGenerationContext context) => (CreateTerrainGrid(context), []);

        private static (IField[,], List<BuildingEntity>) CreateTerrainMapWithTrees(MapGenerationContext context, params (int X, int Y, int Amount)[] trees)
        {
            var fields = CreateTerrainGrid(context);
            foreach (var (x, y, amount) in trees)
            {
                var terrain = (Terrain)fields[x, y];
                terrain.Trees = amount;
                fields[x, y] = terrain;
            }

            return (fields, []);
        }

        private static Func<MapGenerationContext, (IField[,], List<BuildingEntity>)> CreateTerrainMapWithWaterAt(int waterX, int waterY)
        {
            return context =>
            {
                var fields = CreateTerrainGrid(context);
                fields[waterX, waterY] = new Water(waterX, waterY);
                return (fields, []);
            };
        }

        private static (IField[,], List<BuildingEntity>) CreateBridgeMap(MapGenerationContext context)
        {
            var fields = CreateTerrainGrid(context);

            for (int x = 1; x <= 14 && x < context.Width - 1; x++)
            {
                fields[x, 2] = new Water(x, 2);
            }

            for (int x = 1; x <= 16 && x < context.Width - 1; x++)
            {
                fields[x, 4] = new Water(x, 4);
            }

            for (int x = 1; x <= 18 && x < context.Width - 1; x++)
            {
                fields[x, 6] = new Water(x, 6);
            }

            for (int y = 1; y <= 14 && y < context.Height - 1; y++)
            {
                fields[2, y] = new Water(2, y);
            }

            for (int y = 1; y <= 16 && y < context.Height - 1; y++)
            {
                fields[4, y] = new Water(4, y);
            }

            for (int y = 1; y <= 18 && y < context.Height - 1; y++)
            {
                fields[6, y] = new Water(6, y);
            }

            return (fields, []);
        }

        private static (IField[,], List<BuildingEntity>) CreateBuildingMap(MapGenerationContext context)
        {
            var fields = CreateTerrainGrid(context);
            var heightMap = CreateHeightMap(context, 1);

            var city = new CityEntity(2, 2);
            city.GenerateBuildingPoints(0, 0, heightMap);
            city.SetCurrentCapacity(123);

            var camp = new LumberCampEntity();
            camp.GenerateBuildingPoints(2, 2, heightMap);
            camp.SetCurrentCapacity(456);

            foreach (var point in city.MapPoints)
            {
                fields[point.Key.X, point.Key.Y] = point.Value;
            }

            foreach (var point in camp.MapPoints)
            {
                fields[point.Key.X, point.Key.Y] = point.Value;
            }

            return (fields, [city, camp]);
        }

        private static IField[,] CreateTerrainGrid(MapGenerationContext context, int height = 1)
        {
            var fields = new IField[context.Width, context.Height];
            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
                {
                    fields[x, y] = new Terrain(x, y, height);
                }
            }

            return fields;
        }

        private static int[,] CreateHeightMap(MapGenerationContext context, int height = 1)
        {
            var heights = new int[context.Width, context.Height];
            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
                {
                    heights[x, y] = height;
                }
            }

            return heights;
        }

        private sealed class TestVehicle : Vehicle
        {
            public TestVehicle(int x, int y, TransportTycoon.Model.VehicleType type, Load? load)
                : base()
            {
                X = x;
                Y = y;
                Angle = 0.0;
                Type = type;
                TopSpeed = 1.0;
                MaxCapacity = 100;

                Maintenance = 1;
                CurrentSpeed = TopSpeed;
                CurrentLoad = load;
                CurrentCapacity = load is null ? 0 : 1;
                AcceptedGoods = [LoadType.None, LoadType.Wheat, LoadType.Oil, LoadType.Wood, LoadType.Flour, LoadType.Rubber, LoadType.Paper, LoadType.People];
            }
        }

        private sealed class InvalidLoad : Load
        {
            public InvalidLoad()
            {
                Price = 0;
                LoadType = (LoadType)255;
            }
        }
    }
}
