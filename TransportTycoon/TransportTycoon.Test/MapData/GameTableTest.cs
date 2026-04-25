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

        // Középső mező (1, 1). Lerakunk köré utakat (kivéve lefelé).
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2)); // Up (index 0)
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2)); // Right (index 1)
                                                                         // A lentire (2, 1) nem teszünk semmit (Terrain marad)
        table.UpdateTable(1, 0, new Stop(1, 0, 2)); // Left (index 3) - megálló is számít

        // Act
        var neighbours = table.NeighboursOfRoadsAndStops(1, 1);

        // Assert
        Assert.NotNull(neighbours[0]); // Up is Road
        Assert.NotNull(neighbours[1]); // Right is Road
        Assert.Null(neighbours[2]);    // Down is Terrain
        Assert.NotNull(neighbours[3]); // Left is Stop
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

        // Teszt: Csak bal oldali szomszéd
        table.UpdateTable(1, 0, new Road(1, 0, RoadType.Horizontal, 2));
        Assert.Equal(RoadType.Horizontal, table.CalculateRoadType(1, 1));

        // Visszaállítjuk
        table.UpdateTable(1, 0, new Terrain(1, 0, 2));

        // Teszt: Csak felső szomszéd
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2));
        Assert.Equal(RoadType.Vertical, table.CalculateRoadType(1, 1)); // Default ág marad, ami Vertical
    }

    [Fact]
    public void CalculateRoadType_TwoNeighbours_CalculatesTurns()
    {
        var table = CreateTestTable(3, 3, 2);
        // Fel és Jobbra szomszéd
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2)); // Fent (0)
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2)); // Jobbra (1)
        Assert.Equal(RoadType.UpperRightTurn, table.CalculateRoadType(1, 1));

        // Törlés
        table.UpdateTable(0, 1, new Terrain(0, 1, 2));

        // Jobbra és Le szomszéd
        table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 2)); // Le (2)
        Assert.Equal(RoadType.RightTurn, table.CalculateRoadType(1, 1));
    }

    [Fact]
    public void CalculateRoadType_ThreeNeighbours_CalculatesTRoads()
    {
        var table = CreateTestTable(3, 3, 2);

        // Fel, Jobbra, Le szomszéd (Bal üres, azaz index 3 üres -> RightTRoad)
        table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2)); // Fent
        table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 2)); // Jobbra
        table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 2)); // Le

        Assert.Equal(RoadType.RightTRoad, table.CalculateRoadType(1, 1));
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
