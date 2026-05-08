using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData
{
    public class InfrastuctureTests
    {
        public class BridgeTests
        {
            [Fact]
            public void YellowBridge_ConstructorAndProperties_AreSetCorrectly()
            {
                // Arrange & Act
                var bridge = new YellowBridge(1, 2, BridgeType.HorizontalYellowBridge, 3);

                // Assert
                Assert.Equal(1, bridge.X);
                Assert.Equal(2, bridge.Y);
                Assert.Equal(BridgeType.HorizontalYellowBridge, bridge.BridgeType);
                Assert.Equal(3, bridge.Height);

                // Readonly / Constant property checks
                Assert.Equal(60, YellowBridge.Price);
                Assert.Equal(50, bridge.SpeedLimit);
                Assert.Equal(13, bridge.Range);
            }

            [Fact]
            public void GreenBridge_ConstructorAndProperties_AreSetCorrectly()
            {
                // Arrange & Act
                var bridge = new GreenBridge(4, 5, BridgeType.VerticalGreenBridge, 1);

                // Assert
                Assert.Equal(4, bridge.X);
                Assert.Equal(5, bridge.Y);
                Assert.Equal(BridgeType.VerticalGreenBridge, bridge.BridgeType);
                Assert.Equal(1, bridge.Height);

                // Readonly / Constant property checks
                Assert.Equal(80, GreenBridge.Price);
                Assert.Equal(50, bridge.SpeedLimit);
                Assert.Equal(15, bridge.Range);
            }

            [Fact]
            public void RedBridge_ConstructorAndProperties_AreSetCorrectly()
            {
                // Arrange & Act
                var bridge = new RedBridge(7, 8, BridgeType.HorizontalRedBridge, 2);

                // Assert
                Assert.Equal(7, bridge.X);
                Assert.Equal(8, bridge.Y);
                Assert.Equal(BridgeType.HorizontalRedBridge, bridge.BridgeType);
                Assert.Equal(2, bridge.Height);

                // Readonly / Constant property checks
                Assert.Equal(100, RedBridge.Price);
                Assert.Equal(50, bridge.SpeedLimit);
                Assert.Equal(17, bridge.Range);
            }

            [Fact]
            public void CalculateBridgeType_ReturnsAllExpectedTypes_IncludingGreen()
            {
                // Arrange
                var mockGen = Substitute.For<IMapGenerator>();
                var ctx = new MapGenerationContext(20, 20, 1, new MapGenerationSettings());
                var table = new GameTable(mockGen, ctx);

                var cases = new (int dif, string dir, BridgeType expected)[]
                {
                    // Horizontal boundaries
                    (13, "horizontal", BridgeType.HorizontalYellowBridge),
                    (14, "horizontal", BridgeType.HorizontalGreenBridge),
                    (15, "horizontal", BridgeType.HorizontalGreenBridge),
                    (16, "horizontal", BridgeType.HorizontalRedBridge),
                    (17, "horizontal", BridgeType.HorizontalRedBridge),
                    (18, "horizontal", BridgeType.Null),

                    // Vertical boundaries
                    (13, "vertical", BridgeType.VerticalYellowBridge),
                    (14, "vertical", BridgeType.VerticalGreenBridge),
                    (15, "vertical", BridgeType.VerticalGreenBridge),
                    (16, "vertical", BridgeType.VerticalRedBridge),
                    (17, "vertical", BridgeType.VerticalRedBridge),
                    (18, "vertical", BridgeType.Null),
                };

                // Act & Assert
                foreach (var (dif, dir, expected) in cases)
                {
                    var actual = table.CalculateBridgeType(dif, dir);
                    Assert.Equal(expected, actual);
                }
            }
        }
        [Fact]
        public void CreateHorizontalBridge_AddsBridges_ReturnsCorrectCost_AndUpdatesChangedFieldsAndAdjRoads()
        {
            // Arrange
            var mockGen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
            var table = new GameTable(mockGen, ctx);

            // initialize all fields to Terrain
            for (int x = 0; x < table.Width; x++)
                for (int y = 0; y < table.Height; y++)
                    table.UpdateTable(x, y, new Terrain(x, y, 1));

            int a = 2, b = 2, yCoord = 3;
            // place roads adjacent so they will be recalculated/added to changedFields
            table.UpdateTable(a - 1, yCoord, new Road(a - 1, yCoord, RoadType.Horizontal, 1));
            table.UpdateTable(b + 1, yCoord, new Road(b + 1, yCoord, RoadType.Horizontal, 1));

            var changed = new List<(int, int)>();
            // Act
            int cost = table.CreateHorizontalBridge(yCoord, a, b, BridgeType.HorizontalGreenBridge, ref changed);

            // Assert
            Assert.Equal(GreenBridge.Price, cost);
            Assert.Contains((a, yCoord), changed);
            Assert.Contains((a - 1, yCoord), changed); // left road updated
            Assert.Contains((b + 1, yCoord), changed); // right road updated
            Assert.IsType<GreenBridge>(table[a, yCoord]);
        }

        [Fact]
        public void CreateVerticalBridge_AddsBridges_ReturnsCorrectCost_AndUpdatesChangedFieldsAndAdjRoads()
        {
            // Arrange
            var mockGen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
            var table = new GameTable(mockGen, ctx);

            // initialize all fields to Terrain
            for (int x = 0; x < table.Width; x++)
                for (int y = 0; y < table.Height; y++)
                    table.UpdateTable(x, y, new Terrain(x, y, 1));

            int xCoord = 4, a = 1, b = 2;
            // place roads adjacent so they will be recalculated/added to changedFields
            table.UpdateTable(xCoord, a - 1, new Road(xCoord, a - 1, RoadType.Vertical, 1));
            table.UpdateTable(xCoord, b + 1, new Road(xCoord, b + 1, RoadType.Vertical, 1));

            var changed = new List<(int, int)>();
            // Act
            int cost = table.CreateVerticalBridge(xCoord, a, b, BridgeType.VerticalYellowBridge, ref changed);

            // Assert
            Assert.Equal(YellowBridge.Price * (b - a + 1), cost);
            Assert.Contains((xCoord, a), changed);
            Assert.Contains((xCoord, a - 1), changed); // up road updated
            Assert.Contains((xCoord, b + 1), changed); // down road updated
            Assert.IsType<YellowBridge>(table[xCoord, a]);
            Assert.IsType<YellowBridge>(table[xCoord, b]);
        }
        public class RoadTests
        {
            [Fact]
            public void Constructor_SetsPropertiesAndInitializesPointerToNull()
            {
                // Arrange & Act
                var road = new Road(3, 4, RoadType.UpperRightTurn, 2);

                // Assert
                Assert.Equal(3, road.X);
                Assert.Equal(4, road.Y);
                Assert.Equal(RoadType.UpperRightTurn, road.RoadType);
                Assert.Equal(2, road.Height);
                Assert.Null(road.Pointer);
                Assert.Equal(10, Road.Price);
            }

            [Fact]
            public void ChangeType_UpdatesRoadTypeCorrectly()
            {
                // Arrange
                var road = new Road(0, 0, RoadType.Horizontal, 1);

                // Act
                road.ChangeType(RoadType.Vertical);

                // Assert
                Assert.Equal(RoadType.Vertical, road.RoadType);
            }

            [Fact]
            public void InCity_ReturnsFalse_WhenPointerIsNull()
            {
                // Arrange
                var road = new Road(0, 0, RoadType.Horizontal, 1);

                // Act
                bool inCity = road.InCity();

                // Assert
                Assert.False(inCity);
            }
        }

        public class StopTests
        {
            [Fact]
            public void Constructor_SetsPropertiesAndInitializesConnections()
            {
                // Arrange & Act
                var stop = new Stop(5, 6, 1);

                // Assert
                Assert.Equal(5, stop.X);
                Assert.Equal(6, stop.Y);
                Assert.Equal(1, stop.Height);
                Assert.Equal(300, Stop.Price);
                Assert.NotNull(stop.Connections);
                Assert.Empty(stop.Connections);
            }

            [Fact]
            public void SetBuildingBlocks_AddsBuildingBlockToConnections()
            {
                // Arrange
                var stop = new Stop(0, 0, 1);
                var mockBuildingBlock = Substitute.For<IBuildingBlocks>(); // NSubstitute mock

                // Act
                stop.SetBuildingBlocks(mockBuildingBlock);

                // Assert
                Assert.NotNull(stop.Connections);
                Assert.Single(stop.Connections); // Pontosan 1 elemnek kell lennie
                Assert.Contains(mockBuildingBlock, stop.Connections);
            }
        }
    }
}
