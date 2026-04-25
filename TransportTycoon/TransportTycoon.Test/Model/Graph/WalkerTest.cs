using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class WalkerTest
    {
        [Fact]
        public void Walk_TerminatesAtStop_CreatesEdgesAndReturnsEndNode()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            // Set up a 2x2 map: start at (0,0) Road, stop at (1,0)
            var road00 = new Road(0, 0, RoadType.Horizontal, 0);
            var stop10 = new Stop(1, 0, 0);
            gameTable.Table[0, 0] = road00;
            gameTable.Table[1, 0] = stop10;
            gameTable.Table[0, 1] = new Road(0, 1, RoadType.Horizontal, 0);
            gameTable.Table[1, 1] = new Road(1, 1, RoadType.Horizontal, 0);
            var visitedFields = new HashSet<(int, int)>();
            var visitedJunctions = new HashSet<(int, int)>();
            var startNode = new GraphNS.Node(0, 0, typeof(Road));
            var walker = new GraphNS.Walker(startNode, road00, gameTable, visitedFields, visitedJunctions);

            // Act
            var result = walker.Walk();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.EndNode);
            Assert.NotEmpty(result.CreatedEdges);
            Assert.All(result.CreatedEdges, e => Assert.True(e.StartNode.Equals(startNode) || e.EndNode.Equals(startNode)));
        }

        [Fact]
        public void Walk_TerminatesImmediately_IfStartIsStop()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(1, 1, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var stop00 = new Stop(0, 0, 0);
            gameTable.Table[0, 0] = stop00;
            var visitedFields = new HashSet<(int, int)>();
            var visitedJunctions = new HashSet<(int, int)>();
            var startNode = new GraphNS.Node(0, 0, typeof(Stop));
            var walker = new GraphNS.Walker(startNode, stop00, gameTable, visitedFields, visitedJunctions);

            // Act
            var result = walker.Walk();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.EndNode);
            Assert.Equal(2, result.CreatedEdges.Count);
            Assert.All(result.CreatedEdges, e =>
            {
                Assert.Equal(startNode, e.StartNode);
                Assert.Equal(startNode, e.EndNode);
            });
        }

        [Fact]
        public void Walk_StopsIfNoValidField()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(1, 1, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var road00 = new Road(0, 0, RoadType.Horizontal, 0);
            gameTable.Table[0, 0] = road00;
            var visitedFields = new HashSet<(int, int)>();
            var visitedJunctions = new HashSet<(int, int)>();
            var startNode = new GraphNS.Node(0, 0, typeof(Road));
            var walker = new GraphNS.Walker(startNode, road00, gameTable, visitedFields, visitedJunctions);

            // Act
            var result = walker.Walk();

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.EndNode);
            Assert.Empty(result.CreatedEdges);
        }
    }
}
