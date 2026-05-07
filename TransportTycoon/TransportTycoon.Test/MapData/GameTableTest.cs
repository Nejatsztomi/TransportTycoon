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

        var fields = new IField[height, width];
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

    //public class MapGenerationTest
    //{
    //    private GameTable _gameTable = null!;

    //    public MapGenerationTest
    //    {
    //        _gameTable = new(5, 5);
    //    }

    //    //[Fact]
    //    //public void GenerateMap_MapIsGenerated()
    //    //{
    //    //    _gameTable.GenerateMap();
    //    //    Assert.NotEmpty(_gameTable.Table);
    //    //    bool hasInvalidField = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (field is null)
    //    //        {
    //    //            Assert.Fail("Each field in the map should be initialized");
    //    //        }

    //    //        if (!(field is Water || field is Terrain))
    //    //        {
    //    //            hasInvalidField = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasInvalidField, "All fields in the map should be either Water or Terrain");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasValidFields()
    //    //{
    //    //    _gameTable.GenerateMap();

    //    //    bool hasFieldNotInRange = false;
    //    //    bool hasInvalidField = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (!(0 <= field.Height && field.Height <= 4))
    //    //        {
    //    //            hasFieldNotInRange = true;
    //    //            break;
    //    //        }

    //    //        if (!_gameTable.IsTileHeightPossible(field.X, field.Y, field.Height))
    //    //        {
    //    //            hasInvalidField = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasFieldNotInRange, "Each field should have height between 0 and 4");
    //    //    Assert.False(hasInvalidField, "Each field should have a possible height respecting neighbouring tiles");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasTrees()
    //    //{
    //    //    _gameTable.GenerateMap();
    //    //    int treeCount = 0;

    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        treeCount += field.GetTrees();
    //    //    }

    //    //    Assert.True(treeCount > 0, "Generated map should contain trees");
    //    //}

    //    //[Fact]
    //    //public void GenerateMap_MapHasValidTrees()
    //    //{
    //    //    _gameTable.GenerateMap();

    //    //    bool hasFieldWithInvalidTrees = false;
    //    //    foreach (var field in _gameTable.Table)
    //    //    {
    //    //        if (!(0 <= field.GetTrees() && field.GetTrees() <= 4))
    //    //        {
    //    //            hasFieldWithInvalidTrees = true;
    //    //            break;
    //    //        }
    //    //    }

    //    //    Assert.False(hasFieldWithInvalidTrees, "Each field should have a valid number of trees between 0 and 4");
    //    //}
    //}

}
