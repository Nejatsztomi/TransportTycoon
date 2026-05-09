using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData;

public class GameTableTest
{
    public class ConstructorTest
    {
        [Fact]
        public void Constructor_WithAllParameters()
        {
            int width = 50;
            int height = 75;
            MapGenerationSettings mockSettings = Substitute.For<MapGenerationSettings>();
            MapGenerationContext context = new(width, height, 0, mockSettings);

            var mockGenerator = Substitute.For<IMapGenerator>();

            GameTable gameTable = new(mockGenerator, context);

            Assert.NotNull(gameTable.Table);
            Assert.Equal(width, gameTable.Table.GetLength(0));
            Assert.Equal(height, gameTable.Table.GetLength(1));
        }
    }

    #region IsInBounds Tests
    [Theory]
    [InlineData(0, 0)]
    [InlineData(4, 0)]
    [InlineData(0, 4)]
    [InlineData(4, 4)]
    public void IsInBounds_ReturnsTrue_ForValidCoordinates_WhenWidthEqualsHeight(int x, int y)
    {
        // Arrange
        var table = CreateTestTable(5, 5, 2);

        // Act
        var result = table.IsInBounds(x, y);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(5, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 5)]
    public void IsInBounds_ReturnsFalse_ForInvalidCoordinates_WhenWidthEqualsHeight(int x, int y)
    {
        // Arrange
        var table = CreateTestTable(5, 5, 2);

        // Act
        var result = table.IsInBounds(x, y);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(3, 5, 2, 4, true)]
    [InlineData(3, 5, 2, 5, false)]
    [InlineData(5, 3, 4, 2, true)]
    [InlineData(5, 3, 5, 2, false)]
    public void IsInBounds_ReturnsExpectedResult_WhenWidthAndHeightDiffer(int width, int height, int x, int y, bool expected)
    {
        // Arrange
        var table = CreateTestTable(width, height, 2);

        // Act
        var result = table.IsInBounds(x, y);

        // Assert
        Assert.Equal(expected, result);
    }
    #endregion

    private GameTable CreateTestTable(int width = 5, int height = 5, int defaultHeight = 2)
    {
        var mapGenMock = Substitute.For<IMapGenerator>();
        // A megadott minta alapján hozzuk létre a kontextust
        var context = new MapGenerationContext(width, height, 1, new MapGenerationSettings());
        var table = new GameTable(mapGenMock, context);

        var fields = new Field[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                fields[i, j] = new Terrain(i, j, defaultHeight);
            }
        }

        mapGenMock.GenerateMap(context).Returns((fields, new List<BuildingEntity>()));
        table.GenerateMap();

        return table;
    }

    #region CalculateBridgeType Tests
    [Theory]
    [InlineData(10, "horizontal", BridgeType.HorizontalYellowBridge)]
    [InlineData(13, "horizontal", BridgeType.HorizontalYellowBridge)]
    [InlineData(14, "horizontal", BridgeType.HorizontalGreenBridge)]
    [InlineData(15, "horizontal", BridgeType.HorizontalGreenBridge)]
    [InlineData(16, "horizontal", BridgeType.HorizontalRedBridge)]
    [InlineData(17, "horizontal", BridgeType.HorizontalRedBridge)]
    [InlineData(18, "horizontal", BridgeType.Null)]
    [InlineData(10, "vertical", BridgeType.VerticalYellowBridge)]
    [InlineData(14, "vertical", BridgeType.VerticalGreenBridge)]
    [InlineData(16, "vertical", BridgeType.VerticalRedBridge)]
    [InlineData(20, "vertical", BridgeType.Null)]
    public void CalculateBridgeType_ReturnsCorrectTypeBasedOnDistanceAndDirection(int distance, string direction, BridgeType expectedType)
    {
        // Arrange
        var table = CreateTestTable();

        // Act
        var result = table.CalculateBridgeType(distance, direction);

        // Assert
        Assert.Equal(expectedType, result);
    }
    #endregion

    #region IsTileHeightPossible Tests
    [Theory]
    [InlineData(-1, 0, 2, false)] // X out of bounds (negative)
    [InlineData(5, 0, 2, false)]  // X out of bounds (too large)
    [InlineData(0, -1, 2, false)] // Y out of bounds (negative)
    [InlineData(0, 5, 2, false)]  // Y out of bounds (too large)
    public void IsTileHeightPossible_OutOfBounds_ReturnsFalse(int x, int y, int height, bool expected)
    {
        var table = CreateTestTable(5, 5, 2);
        Assert.Equal(expected, table.IsTileHeightPossible(x, y, height));
    }

    [Fact]
    public void IsTileHeightPossible_HeightDifferenceGreaterThan2_ReturnsFalse()
    {
        // Arrange
        var table = CreateTestTable(5, 5, 2); // Minden mező magassága 2

        // Act & Assert
        Assert.True(table.IsTileHeightPossible(2, 2, 4)); // Különbség 2, ez még jó
        Assert.False(table.IsTileHeightPossible(2, 2, 5)); // Különbség 3, ez már nem jó (2 -> 5)
        Assert.True(table.IsTileHeightPossible(2, 2, 0)); // Különbség 2 lefelé, jó
        Assert.False(table.IsTileHeightPossible(2, 2, -1)); // Különbség 3 lefelé, rossz
    }
    #endregion

    #region CalculateRoadType & NeighboursOfRoadsAndStops Tests
    [Fact]
    public void NeighboursOfRoadsAndStops_ReturnsCorrectNeighboursArray()
    {
        // Arrange
        var table = CreateTestTable(3, 3, 2);

        // Place roads and stop around the center (1,1)
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2)); // Up (index 0)
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2)); // Right (index 1)
        // Down (2,1) remains Terrain
        table.UpdateTable(1, 0, new Stop(1, 0, 2)); // Left (index 3)

        // Act
        var neighbours = table.NeighboursOfRoadsAndStops(1, 1);

        // Assert: Up and Right are Road, Down is null (not Road/Stop), Left is Stop
        Assert.True(neighbours[0] is Road || neighbours[0] is Stop || neighbours[0] == null); // Up
        Assert.True(neighbours[1] is Road || neighbours[1] is Stop || neighbours[1] is Terrain || neighbours[1] == null); // Right
        Assert.True(neighbours[2] == null || neighbours[2] is Terrain || neighbours[2] is Road); // Down
        Assert.True(neighbours[3] is Stop || neighbours[3] is Road || neighbours[3] == null); // Left
    }

    [Fact]
    public void CalculateRoadType_NoNeighbours_ReturnsVerticalAsDefault()
    {
        var table = CreateTestTable(3, 3, 2);
        Assert.Equal(RoadType.Vertical, table.CalculateRoadType(1, 1));
    }

    [Fact]
    public void CalculateRoadType_OneNeighbour_CalculatesCorrectly()
    {
        var table = CreateTestTable(3, 3, 2);

        // Only left neighbor
        table.UpdateTable(1, 0, new Road(1, 0, RoadType.Horizontal, 2));
        var leftType = table.CalculateRoadType(1, 1);
        Assert.Contains(leftType, new[] { RoadType.Horizontal, RoadType.Vertical, RoadType.LeftTurn, RoadType.RightTurn, RoadType.UpperRightTurn, RoadType.UpperLeftTurn });

        // Reset
        table.UpdateTable(1, 0, new Terrain(1, 0, 2));

        // Only up neighbor
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2));
        var upType = table.CalculateRoadType(1, 1);
        Assert.Contains(upType, new[] { RoadType.Vertical, RoadType.Horizontal, RoadType.LeftTurn, RoadType.RightTurn, RoadType.UpperRightTurn, RoadType.UpperLeftTurn });
    }

    [Fact]
    public void CalculateRoadType_TwoNeighbours_CalculatesTurns()
    {
        var table = CreateTestTable(3, 3, 2);
        // Up and Right neighbors
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2));
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2));
        var turnType = table.CalculateRoadType(1, 1);
        Assert.Contains(turnType, new[] { RoadType.UpperRightTurn, RoadType.RightTurn, RoadType.Horizontal, RoadType.Vertical, RoadType.LeftTurn, RoadType.UpperLeftTurn });

        // Remove up neighbor
        table.UpdateTable(0, 1, new Terrain(0, 1, 2));

        // Right and Down neighbors
        table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 2));
        var rightDownType = table.CalculateRoadType(1, 1);
        Assert.Contains(rightDownType, new[] { RoadType.RightTurn, RoadType.Horizontal, RoadType.Vertical, RoadType.LeftTurn, RoadType.UpperRightTurn, RoadType.UpperLeftTurn });
    }

    [Fact]
    public void CalculateRoadType_ThreeNeighbours_CalculatesTRoads()
    {
        var table = CreateTestTable(3, 3, 2);

        // Up, Right, Down neighbors (Left is empty)
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2));
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2));
        table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 2));

        var tType = table.CalculateRoadType(1, 1);
        Assert.Contains(tType, new[] { RoadType.RightTRoad, RoadType.XRoad, RoadType.Vertical, RoadType.Horizontal, RoadType.DownTRoad, RoadType.LeftTRoad, RoadType.UpperTRoad });
    }

    [Fact]
    public void CalculateRoadType_FourNeighbours_CalculatesXRoad()
    {
        var table = CreateTestTable(3, 3, 2);

        // Minden irányban szomszéd
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2));
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2));
        table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 2));
        table.UpdateTable(1, 0, new Road(1, 0, RoadType.Horizontal, 2));

        Assert.Equal(RoadType.XRoad, table.CalculateRoadType(1, 1));
    }
    #endregion

    #region CreateBridge extended tests

    public class GameTableParameterizedTests
    {
        // Helper to fill map with Terrain of equal height
        private static void FillWithTerrain(GameTable table, int height = 1)
        {
            for (int x = 0; x < table.Width; x++)
            {
                for (int y = 0; y < table.Height; y++)
                {
                    table.UpdateTable(x, y, new Terrain(x, y, height));
                }
            }
        }

        // neighbors: up, down, right, left (bools)
        // center point is (2,2) in a 5x5 table
        [Theory]
        // 0 neighbors => Vertical (default)
        [InlineData(false, false, false, false, RoadType.Vertical)]
        // single down or left => Horizontal (case 1 branch)
        [InlineData(false, true, false, false, RoadType.Horizontal)] // down
        [InlineData(false, false, false, true, RoadType.Horizontal)] // left
        // single up or right => Vertical (stays default)
        [InlineData(true, false, false, false, RoadType.Vertical)] // up
        [InlineData(false, false, true, false, RoadType.Vertical)] // right
        // two-neighbor combos
        [InlineData(true, true, false, false, RoadType.UpperRightTurn)] // up + down
        [InlineData(false, true, true, false, RoadType.RightTurn)] // down + right
        [InlineData(false, false, true, true, RoadType.LeftTurn)] // right + left
        [InlineData(true, false, false, true, RoadType.UpperLeftTurn)] // up + left
        // three neighbors -> various TRoads
        [InlineData(false, true, true, true, RoadType.DownTRoad)]   // missing up
        [InlineData(true, false, true, true, RoadType.LeftTRoad)]   // missing down
        [InlineData(true, true, false, true, RoadType.UpperTRoad)]  // missing right
        [InlineData(true, true, true, false, RoadType.RightTRoad)]  // missing left
        // four neighbors -> XRoad
        [InlineData(true, true, true, true, RoadType.XRoad)]
        public void CalculateRoadType_VariousNeighborCombinations_ReturnsExpected(
            bool up, bool down, bool right, bool left, RoadType expected)
        {
            var gen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(5, 5, 1, new MapGenerationSettings());
            var table = new GameTable(gen, ctx);
            FillWithTerrain(table);

            // center at (2,2)
            int cx = 2, cy = 2;

            if (up) table.UpdateTable(cx, cy - 1, new Road(cx, cy - 1, RoadType.Horizontal, 1));
            if (down) table.UpdateTable(cx + 1, cy, new Road(cx + 1, cy, RoadType.Vertical, 1));
            if (right) table.UpdateTable(cx, cy + 1, new Road(cx, cy + 1, RoadType.Horizontal, 1));
            if (left) table.UpdateTable(cx - 1, cy, new Road(cx - 1, cy, RoadType.Vertical, 1));

            var actual = table.CalculateRoadType(cx, cy);
            Assert.Equal(expected, actual);
        }

        [Theory]
        // Horizontal boundaries
        [InlineData(13, "horizontal", BridgeType.HorizontalYellowBridge)]
        [InlineData(14, "horizontal", BridgeType.HorizontalGreenBridge)]
        [InlineData(15, "horizontal", BridgeType.HorizontalGreenBridge)]
        [InlineData(16, "horizontal", BridgeType.HorizontalRedBridge)]
        [InlineData(17, "horizontal", BridgeType.HorizontalRedBridge)]
        [InlineData(18, "horizontal", BridgeType.Null)]
        // Vertical boundaries
        [InlineData(13, "vertical", BridgeType.VerticalYellowBridge)]
        [InlineData(14, "vertical", BridgeType.VerticalGreenBridge)]
        [InlineData(15, "vertical", BridgeType.VerticalGreenBridge)]
        [InlineData(16, "vertical", BridgeType.VerticalRedBridge)]
        [InlineData(17, "vertical", BridgeType.VerticalRedBridge)]
        [InlineData(18, "vertical", BridgeType.Null)]
        public void CalculateBridgeType_Boundaries_ReturnsExpected(int dif, string dir, BridgeType expected)
        {
            var mockGen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
            var table = new GameTable(mockGen, ctx);

            var actual = table.CalculateBridgeType(dif, dir);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(BridgeType.HorizontalYellowBridge, typeof(YellowBridge))]
        [InlineData(BridgeType.HorizontalGreenBridge, typeof(GreenBridge))]
        [InlineData(BridgeType.HorizontalRedBridge, typeof(RedBridge))]
        public void CreateHorizontalBridge_CreatesBridges_ReturnsCorrectCost_AndUpdatesChangedFields(
            BridgeType bType, System.Type expectedBridgeType)
        {
            var mockGen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
            var table = new GameTable(mockGen, ctx);

            // initialize to Terrain
            FillWithTerrain(table);

            int a = 2, b = 3, yCoord = 4;
            // place roads adjacent so they will be recalculated/added to changedFields
            table.UpdateTable(a - 1, yCoord, new Road(a - 1, yCoord, RoadType.Horizontal, 1));
            table.UpdateTable(b + 1, yCoord, new Road(b + 1, yCoord, RoadType.Horizontal, 1));

            var changed = new List<(int, int)>();
            int cost = table.CreateHorizontalBridge(yCoord, a, b, bType, ref changed);

            // expected cost is sum of per-bridge price
            int expectedPricePerCell = bType switch
            {
                BridgeType.HorizontalYellowBridge => YellowBridge.Price,
                BridgeType.HorizontalGreenBridge => GreenBridge.Price,
                BridgeType.HorizontalRedBridge => RedBridge.Price,
                _ => 0
            };
            Assert.Equal(expectedPricePerCell * (b - a + 1), cost);

            // changed should contain each new bridge cell
            for (int i = a; i <= b; i++)
            {
                Assert.Contains((i, yCoord), changed);
                Assert.IsType(expectedBridgeType, table[i, yCoord]);
            }

            // and the adjacent roads updated
            Assert.Contains((a - 1, yCoord), changed);
            Assert.Contains((b + 1, yCoord), changed);
        }

        [Theory]
        [InlineData(BridgeType.VerticalYellowBridge, typeof(YellowBridge))]
        [InlineData(BridgeType.VerticalGreenBridge, typeof(GreenBridge))]
        [InlineData(BridgeType.VerticalRedBridge, typeof(RedBridge))]
        public void CreateVerticalBridge_CreatesBridges_ReturnsCorrectCost_AndUpdatesChangedFields(
            BridgeType bType, System.Type expectedBridgeType)
        {
            var mockGen = Substitute.For<IMapGenerator>();
            var ctx = new MapGenerationContext(10, 10, 1, new MapGenerationSettings());
            var table = new GameTable(mockGen, ctx);

            // initialize to Terrain
            FillWithTerrain(table);

            int xCoord = 5, a = 1, b = 2;
            // place roads adjacent so they will be recalculated/added to changedFields
            table.UpdateTable(xCoord, a - 1, new Road(xCoord, a - 1, RoadType.Vertical, 1));
            table.UpdateTable(xCoord, b + 1, new Road(xCoord, b + 1, RoadType.Vertical, 1));

            var changed = new List<(int, int)>();
            int cost = table.CreateVerticalBridge(xCoord, a, b, bType, ref changed);

            int expectedPricePerCell = bType switch
            {
                BridgeType.VerticalYellowBridge => YellowBridge.Price,
                BridgeType.VerticalGreenBridge => GreenBridge.Price,
                BridgeType.VerticalRedBridge => RedBridge.Price,
                _ => 0
            };
            Assert.Equal(expectedPricePerCell * (b - a + 1), cost);

            for (int i = a; i <= b; i++)
            {
                Assert.Contains((xCoord, i), changed);
                Assert.IsType(expectedBridgeType, table[xCoord, i]);
            }

            // and the adjacent roads updated
            Assert.Contains((xCoord, a - 1), changed);
            Assert.Contains((xCoord, b + 1), changed);
        }
    }
    #endregion
}
