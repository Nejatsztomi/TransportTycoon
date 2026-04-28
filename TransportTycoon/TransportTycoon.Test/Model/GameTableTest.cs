using NSubstitute;
using System.Reflection;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.Model
{
    public class GameTableAdvancedTests
    {
        // Segédmetódus a tesztpálya gyors felépítéséhez
        private GameTable CreateTestTable(int width = 5, int height = 5, int defaultHeight = 2)
        {
            var mapGenMock = Substitute.For<IMapGenerator>();
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

        #region Hidak építése (Create Bridges)
        [Fact]
        public void CreateHorizontalBridge_PlacesBridgeAndUpdatesAdjacentRoads()
        {
            // Arrange
            var table = CreateTestTable(5, 5, 1);
            table.UpdateTable(1, 0, new Road(1, 0, RoadType.Horizontal, 1)); // Bal út
            table.UpdateTable(1, 4, new Road(1, 4, RoadType.Horizontal, 1)); // Jobb út
            var changedFields = new List<(int, int)>();

            // Act - Híd építése (1,1)-től (1,3)-ig
            int cost = table.CreateHorizontalBridge(1, 1, 3, BridgeType.HorizontalGreenBridge, ref changedFields);

            // Assert
            Assert.IsType<GreenBridge>(table[1, 1]);
            Assert.IsType<GreenBridge>(table[1, 2]);
            Assert.IsType<GreenBridge>(table[1, 3]);
            Assert.True(cost > 0);
            Assert.Contains((1, 1), changedFields);
            Assert.Contains((1, 0), changedFields); // Bal út frissült
            Assert.Contains((1, 4), changedFields); // Jobb út frissült
        }

        [Fact]
        public void CreateVerticalBridge_PlacesBridgeAndUpdatesAdjacentRoads()
        {
            // Arrange
            var table = CreateTestTable(5, 5, 1);
            table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 1)); // Felső út
            table.UpdateTable(4, 1, new Road(4, 1, RoadType.Vertical, 1)); // Alsó út
            var changedFields = new List<(int, int)>();

            // Act - Híd építése (1,1)-től (3,1)-ig
            int cost = table.CreateVerticalBridge(1, 1, 3, BridgeType.VerticalRedBridge, ref changedFields);

            // Assert
            Assert.IsType<RedBridge>(table[1, 1]);
            Assert.IsType<RedBridge>(table[2, 1]);
            Assert.IsType<RedBridge>(table[3, 1]);
            Assert.True(cost > 0);
            Assert.Contains((0, 1), changedFields); // Felső út frissült
            Assert.Contains((4, 1), changedFields); // Alsó út frissült
        }

        [Fact]
        public void CreateShortBridge_WithAdjacentPlains_CreatesHorizontalBridge()
        {
            // Arrange
            var table = CreateTestTable(5, 5, 1); // Minden Plain Terrain magasság 1-gyel
            var changedFields = new List<(int, int)>();

            // Act
            int cost = table.CreateShortBridge(1, 1, ref changedFields);

            // Assert
            Assert.IsType<YellowBridge>(table[1, 1]);
            Assert.Equal(BridgeType.HorizontalYellowBridge, ((YellowBridge)table[1, 1]).BridgeType);
            Assert.True(cost > 0);
        }

        [Fact]
        public void CreateShortBridge_OutOfBounds_ReturnsZeroCost()
        {
            // Arrange
            var table = CreateTestTable(5, 5, 1);
            var changedFields = new List<(int, int)>();

            // Act
            int cost = table.CreateShortBridge(0, 0, ref changedFields); // Szélen van

            // Assert
            Assert.Equal(0, cost);
        }
        #endregion

        #region Hidak rombolása (Destroy Bridge)
        [Fact]
        public void DestroyBridge_Horizontal_DestroysConnectedBridgesAndUpdatesRoads()
        {
            // Arrange
            var table = CreateTestTable(5, 5, 1);
            var changedFields = new List<(int, int)>();
            table.CreateHorizontalBridge(2, 1, 3, BridgeType.HorizontalGreenBridge, ref changedFields);
            table.UpdateTable(2, 0, new Road(2, 0, RoadType.Horizontal, 1));
            changedFields.Clear();

            // Act - Rombolás a híd közepén
            table.DestroyBridge(2, 2, ref changedFields);

            // Assert
            Assert.IsType<Water>(table[2, 1]);
            Assert.IsType<Water>(table[2, 2]);
            Assert.IsType<Water>(table[2, 3]);
            Assert.Contains((2, 0), changedFields); // Az utat is érintette a frissítés
        }
        #endregion

        #region Térkép és környezet ellenőrzések
        [Fact]
        public void CheckNeighboringTrees_ReturnsOnlyTerrainsWithZeroTrees()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2);
            table.UpdateTable(0, 1, new Terrain(0, 1, 2) { Trees = 0 }); // Érvényes
            table.UpdateTable(2, 1, new Terrain(2, 1, 2) { Trees = 2 }); // Érvénytelen (van fa)
            table.UpdateTable(1, 0, new Water(1, 0)); // Érvénytelen (nem Terrain)
            table.UpdateTable(1, 2, new Terrain(1, 2, 2) { Trees = 0 }); // Érvényes

            // Act
            var neighbours = table.CheckNeighboringTrees(1, 1);

            // Assert
            Assert.Equal(2, neighbours.Count);
            Assert.Contains(table[0, 1], neighbours);
            Assert.Contains(table[1, 2], neighbours);
        }

        [Fact]
        public void IsMapAccurate_PrivateMethod_ReturnsTrueForValidMap()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2); // Minden szomszéd szintkülönbsége 0 (éppen jó)
            MethodInfo? isMapAccurateMethod = typeof(GameTable).GetMethod("IsMapAccurate", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            bool isAccurate = (bool)isMapAccurateMethod!.Invoke(table, null)!;

            // Assert
            Assert.True(isAccurate);
        }

        [Fact]
        public void IsMapAccurate_PrivateMethod_ReturnsFalseForInvalidMap()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2);
            table.UpdateTable(1, 1, new Terrain(1, 1, 6)); // Túl nagy ugrás a magasságban (2->6)
            MethodInfo? isMapAccurateMethod = typeof(GameTable).GetMethod("IsMapAccurate", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            bool isAccurate = (bool)isMapAccurateMethod!.Invoke(table, null)!;

            // Assert
            Assert.False(isAccurate);
        }
        #endregion

        #region StopEnvironment (Megálló környezet)
        [Fact]
        public void StopEnvironment_WithRoadNeighbour_CreatesStop()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2);
            table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 2)); // Út felette

            // Act
            bool result = table.StopEnvironment(1, 1);

            // Assert
            Assert.True(result);
            Assert.IsType<Stop>(table[1, 1]);
        }

        [Fact]
        public void StopEnvironment_WithUpwardBuildingBlock_AddsConnection()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2);
            var mockBuilding = Substitute.For<IBuildingBlocks>();
            mockBuilding.Height.Returns(2); // Azonos magasság a HeightCheck miatt
            table.UpdateTable(0, 1, mockBuilding); // Épület felette (x-1, y)

            // Act
            bool result = table.StopEnvironment(1, 1);

            // Assert
            Assert.True(result);
            Assert.IsType<Stop>(table[1, 1]);
            var stop = (Stop)table[1, 1];
            Assert.Single(stop.Connenctions!);
            Assert.Contains(mockBuilding, stop.Connenctions!);
        }

        [Fact]
        public void StopEnvironment_NoAdjacentRoadOrBuilding_ReturnsFalse()
        {
            // Arrange
            var table = CreateTestTable(3, 3, 2); // Csak füves Terrain mezők

            // Act
            bool result = table.StopEnvironment(1, 1);

            // Assert
            Assert.False(result);
            Assert.IsNotType<Stop>(table[1, 1]); // Nem épült meg
        }
        #endregion
    }
    public class GameTableBridgeAndStopTests
    {
        // Segédmetódus a tesztpálya gyors felépítéséhez
        private GameTable CreateTestTable(int width = 5, int height = 5, int defaultHeight = 2)
        {
            var mapGenMock = Substitute.For<IMapGenerator>();
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

        #region CreateShortBridge Tesztek
        [Fact]
        public void CreateShortBridge_OutOfBounds_ReturnsZero()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();

            // Szélén lévő mező, kilépne a tömbből
            int cost = table.CreateShortBridge(0, 0, ref changedFields);
            Assert.Equal(0, cost);
        }

        [Fact]
        public void CreateShortBridge_BetweenHorizontalInfrastructures_CreatesHorizontalBridge()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();
            table.UpdateTable(1, 0, new Road(1, 0, RoadType.Horizontal, 1));
            table.UpdateTable(1, 2, new Road(1, 2, RoadType.Horizontal, 1));

            table.CreateShortBridge(1, 1, ref changedFields);

            Assert.IsType<YellowBridge>(table[1, 1]);
            Assert.Equal(BridgeType.HorizontalYellowBridge, ((YellowBridge)table[1, 1]).BridgeType);
        }

        [Fact]
        public void CreateShortBridge_BetweenVerticalInfrastructures_CreatesVerticalBridge()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();
            table.UpdateTable(0, 1, new Road(0, 1, RoadType.Vertical, 1));
            table.UpdateTable(2, 1, new Road(2, 1, RoadType.Vertical, 1));

            table.CreateShortBridge(1, 1, ref changedFields);

            Assert.IsType<YellowBridge>(table[1, 1]);
            Assert.Equal(BridgeType.VerticalYellowBridge, ((YellowBridge)table[1, 1]).BridgeType);
        }

        [Fact]
        public void CreateShortBridge_BetweenHorizontalPlains_CreatesHorizontalBridge()
        {
            var table = CreateTestTable(5, 5, 1); // Height 1 = Plain Terrain
            var changedFields = new List<(int, int)>();

            table.CreateShortBridge(1, 1, ref changedFields);

            Assert.IsType<YellowBridge>(table[1, 1]);
            Assert.Equal(BridgeType.HorizontalYellowBridge, ((YellowBridge)table[1, 1]).BridgeType);
        }

        [Fact]
        public void CreateShortBridge_BetweenVerticalPlains_CreatesVerticalBridge()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();
            table.UpdateTable(0, 1, new Terrain(0, 1, 1)); // Plain
            table.UpdateTable(2, 1, new Terrain(2, 1, 1)); // Plain
            // A szélsőket direkt nem tesszük Plain-re, hogy a Vertical ágra fusson

            table.CreateShortBridge(1, 1, ref changedFields);

            Assert.IsType<YellowBridge>(table[1, 1]);
            Assert.Equal(BridgeType.VerticalYellowBridge, ((YellowBridge)table[1, 1]).BridgeType);
        }
        #endregion

        #region DestroyBridge Tesztek
        [Fact]
        public void DestroyBridge_NotABridge_DoesNothing()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();

            table.DestroyBridge(1, 1, ref changedFields); // Terrain van ott alapból

            Assert.Empty(changedFields);
            Assert.IsType<Terrain>(table[1, 1]);
        }

        [Fact]
        public void DestroyBridge_HorizontalBridge_DestroysAndUpdatesRoads()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();

            // Építünk egy 3 hosszú hidat
            table.UpdateTable(2, 1, new YellowBridge(2, 1, BridgeType.HorizontalYellowBridge, 1));
            table.UpdateTable(2, 2, new YellowBridge(2, 2, BridgeType.HorizontalYellowBridge, 1));
            table.UpdateTable(2, 3, new YellowBridge(2, 3, BridgeType.HorizontalYellowBridge, 1));
            table.UpdateTable(2, 0, new Road(2, 0, RoadType.Horizontal, 1));
            table.UpdateTable(2, 4, new Road(2, 4, RoadType.Horizontal, 1));

            // Romboljuk le a közepén
            table.DestroyBridge(2, 2, ref changedFields);

            // A híd elemeinek vízzé kellett változniuk
            Assert.IsType<Water>(table[2, 1]);
            Assert.IsType<Water>(table[2, 2]);
            Assert.IsType<Water>(table[2, 3]);

            // Az utakat is frissítenie kellett a széleken
            Assert.Contains((2, 0), changedFields);
            Assert.Contains((2, 4), changedFields);
        }

        [Fact]
        public void DestroyBridge_VerticalBridge_DestroysAndUpdatesRoads()
        {
            var table = CreateTestTable();
            var changedFields = new List<(int, int)>();

            // Építünk egy vertikális hidat
            table.UpdateTable(1, 2, new GreenBridge(1, 2, BridgeType.VerticalGreenBridge, 1));
            table.UpdateTable(2, 2, new GreenBridge(2, 2, BridgeType.VerticalGreenBridge, 1));
            table.UpdateTable(3, 2, new GreenBridge(3, 2, BridgeType.VerticalGreenBridge, 1));
            table.UpdateTable(0, 2, new Road(0, 2, RoadType.Vertical, 1));
            table.UpdateTable(4, 2, new Road(4, 2, RoadType.Vertical, 1));

            // Romboljuk le a közepén
            table.DestroyBridge(2, 2, ref changedFields);

            // A híd elemeinek vízzé kellett változniuk
            Assert.IsType<Water>(table[1, 2]);
            Assert.IsType<Water>(table[2, 2]);
            Assert.IsType<Water>(table[3, 2]);

            // Az utakat is frissítenie kellett a széleken
            Assert.Contains((0, 2), changedFields);
            Assert.Contains((4, 2), changedFields);
        }
        #endregion

        #region StopEnvironment Tesztek
        [Fact]
        public void StopEnvironment_NeighbourIsRoad_CreatesStopAndReturnsTrue()
        {
            var table = CreateTestTable();
            table.UpdateTable(0, 1, new Road(0, 1, RoadType.Horizontal, 2));

            bool result = table.StopEnvironment(1, 1);

            Assert.True(result);
            Assert.IsType<Stop>(table[1, 1]);
        }

        [Fact]
        public void StopEnvironment_BuildingAbove_CreatesStopAndConnects()
        {
            var table = CreateTestTable();
            var mockBlock = Substitute.For<IBuildingBlocks>();
            mockBlock.Height.Returns(2);
            table.UpdateTable(0, 1, mockBlock); // Felül (x-1)

            bool result = table.StopEnvironment(1, 1);

            Assert.True(result);
            Assert.Contains(mockBlock, ((Stop)table[1, 1]).Connenctions!);
        }

        [Fact]
        public void StopEnvironment_BuildingRight_CreatesStopAndConnects()
        {
            var table = CreateTestTable();
            var mockBlock = Substitute.For<IBuildingBlocks>();
            mockBlock.Height.Returns(2);
            table.UpdateTable(1, 2, mockBlock); // Jobbra (y+1)

            bool result = table.StopEnvironment(1, 1);

            Assert.True(result);
            Assert.Contains(mockBlock, ((Stop)table[1, 1]).Connenctions!);
        }

        [Fact]
        public void StopEnvironment_BuildingBelow_CreatesStopAndConnects()
        {
            var table = CreateTestTable();
            var mockBlock = Substitute.For<IBuildingBlocks>();
            mockBlock.Height.Returns(2);
            table.UpdateTable(2, 1, mockBlock); // Alul (x+1)

            bool result = table.StopEnvironment(1, 1);

            Assert.True(result);
            Assert.Contains(mockBlock, ((Stop)table[1, 1]).Connenctions!);
        }

        [Fact]
        public void StopEnvironment_BuildingLeft_CreatesStopAndConnects()
        {
            var table = CreateTestTable();
            var mockBlock = Substitute.For<IBuildingBlocks>();
            mockBlock.Height.Returns(2);
            table.UpdateTable(1, 0, mockBlock); // Balra (y-1)

            bool result = table.StopEnvironment(1, 1);

            Assert.True(result);
            Assert.Contains(mockBlock, ((Stop)table[1, 1]).Connenctions!);
        }

        [Fact]
        public void StopEnvironment_NoValidNeighbours_ReturnsFalse()
        {
            var table = CreateTestTable(); // Minden szomszéd üres Terrain

            bool result = table.StopEnvironment(1, 1);

            Assert.False(result);
            Assert.IsNotType<Stop>(table[1, 1]); // Nem épül meg
        }
        #endregion
    }
}
