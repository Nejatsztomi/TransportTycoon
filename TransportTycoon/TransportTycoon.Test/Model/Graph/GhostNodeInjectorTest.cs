using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class GhostNodeInjectorTest
    {
        [Fact]
        public void GetOrInjectGhostNode_ReturnsRealNode_IfExists()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var node = new GraphNS.Node(0, 0, typeof(Stop));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { node, [] } });
            var injector = new GraphNS.GhostNodeInjector(graph, new(gameTable));
            var field = new Stop(0, 0, 0);

            // Act
            var (resultNode, isGhost) = injector.GetOrInjectGhostNode(field);

            // Assert
            Assert.Equal(node, resultNode);
            Assert.False(isGhost);
        }

        [Fact]
        public void GetOrInjectGhostNode_InjectsGhostNode_WhenNoRealNodeExists_AndConnectsToJunction()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            // Set up a 2x2 map: ghost at (0,0), real node at (1,0)
            var stop10 = new Stop(1, 0, 0);
            var road00 = new Road(0, 0, RoadType.Horizontal, 0);
            gameTable.Table[0, 0] = road00;
            gameTable.Table[1, 0] = stop10;
            gameTable.Table[0, 1] = new Road(0, 1, RoadType.Horizontal, 0);
            gameTable.Table[1, 1] = new Road(1, 1, RoadType.Horizontal, 0);
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { new GraphNS.Node(1, 0, typeof(Stop)), [] } });
            var injector = new GraphNS.GhostNodeInjector(graph, new(gameTable));
            var field = road00;

            // Act
            var (resultNode, isGhost) = injector.GetOrInjectGhostNode(field);

            // Assert
            Assert.NotNull(resultNode);
            Assert.True(isGhost);
            Assert.Contains(resultNode, graph.AdjacencyList.Keys);
        }

        [Fact]
        public void GetOrInjectGhostNode_ReturnsNull_WhenNoJunctionReachable()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            // Set up a 2x2 map, all roads, but no reachable junctions (no real nodes in graph)
            gameTable.Table[0, 0] = new Road(0, 0, RoadType.Horizontal, 0);
            gameTable.Table[1, 0] = new Road(1, 0, RoadType.Horizontal, 0);
            gameTable.Table[0, 1] = new Road(0, 1, RoadType.Horizontal, 0);
            gameTable.Table[1, 1] = new Road(1, 1, RoadType.Horizontal, 0);
            var graph = new GraphNS.Graph([]);
            var injector = new GraphNS.GhostNodeInjector(graph, new(gameTable));
            var field = new Road(0, 0, RoadType.Horizontal, 0);

            // Act
            var (resultNode, isGhost) = injector.GetOrInjectGhostNode(field);

            // Assert
            Assert.Null(resultNode);
            Assert.False(isGhost);
        }

        [Fact]
        public void RemoveGhostNode_RemovesNodeFromGraph()
        {
            // Arrange
            var gameTable = Substitute.For<GameTable>(Substitute.For<IMapGenerator>(), new MapGenerationContext(2, 2, 0, new MapGenerationSettings()));
            var ghostNode = new GraphNS.Node(0, 0, typeof(Road));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { ghostNode, [] } });
            var injector = new GraphNS.GhostNodeInjector(graph, new(gameTable));

            // Act
            injector.RemoveGhostNode(ghostNode);

            // Assert
            Assert.DoesNotContain(ghostNode, graph.AdjacencyList.Keys);
        }
    }
}
