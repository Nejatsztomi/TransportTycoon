using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class GraphBuilderTest
    {
        [Fact]
        public void BuildGraph_BasicMap_CreatesExpectedGraph()
        {
            // Arrange
            // 2x2 map: Stop at (0,0), Road at (1,0), Stop at (1,1), Road at (0,1)
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var map = new GameTable(mapGen, context);
            var stop00 = new Stop(0, 0, 0);
            var stop11 = new Stop(1, 1, 0);
            var road10 = new Road(1, 0, RoadType.Horizontal, 0);
            var road01 = new Road(0, 1, RoadType.Vertical, 0);
            map.Table[0, 0] = stop00;
            map.Table[1, 0] = road10;
            map.Table[1, 1] = stop11;
            map.Table[0, 1] = road01;

            // Act
            var graph = GraphNS.GraphBuilder.BuildGraph(map);

            // Assert
            Assert.NotNull(graph);
            Assert.NotEmpty(graph.AdjacencyList);
            // Should contain at least the two stops as nodes
            Assert.Contains(graph.AdjacencyList.Keys, n => n.X == 0 && n.Y == 0);
            Assert.Contains(graph.AdjacencyList.Keys, n => n.X == 1 && n.Y == 1);
        }

        [Fact]
        public void BuildGraph_AddsDestinationNode_WhenGraphDoesNotContainIt()
        {
            // Arrange
            // 2x1 map: Stop at (0,0) connected to an XRoad at (1,0)
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 1, 0, new MapGenerationSettings());
            var map = new GameTable(mapGen, context);
            var stop00 = new Stop(0, 0, 0);
            var junction10 = new Road(1, 0, RoadType.XRoad, 0);
            map.Table[0, 0] = stop00;
            map.Table[1, 0] = junction10;

            // Act
            var graph = GraphNS.GraphBuilder.BuildGraph(map);

            // Assert
            var destinationNode = graph.AdjacencyList.Keys.SingleOrDefault(n => n.X == 1 && n.Y == 0);
            Assert.NotNull(destinationNode);

            var startNode = graph.AdjacencyList.Keys.Single(n => n.X == 0 && n.Y == 0);
            Assert.Single(graph.AdjacencyList[startNode]);
            Assert.Equal(destinationNode, graph.AdjacencyList[startNode][0].EndNode);

            Assert.Single(graph.AdjacencyList[destinationNode]);
            Assert.Equal(startNode, graph.AdjacencyList[destinationNode][0].EndNode);
        }
    }
}
