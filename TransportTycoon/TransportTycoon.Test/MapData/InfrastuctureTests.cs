using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

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
                Assert.Equal(50, bridge.Price);
                Assert.Equal(100, bridge.SpeedLimit);
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
                Assert.Equal(100, bridge.Price);
                Assert.Equal(100, bridge.SpeedLimit);
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
                Assert.Equal(150, bridge.Price);
                Assert.Equal(100, bridge.SpeedLimit);
                Assert.Equal(17, bridge.Range);
            }
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
                Assert.Equal(100, road.Price);
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
                Assert.Equal(200, stop.Price);
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
                Assert.Single(stop.Connections!); // Pontosan 1 elemnek kell lennie
                Assert.Contains(mockBuildingBlock, stop.Connections!);
            }
        }
    }
}
