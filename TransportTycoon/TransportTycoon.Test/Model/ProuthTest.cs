using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model
{
    public class ProuthTest
    {
        [Fact]
        public void AddStop_AllowsAddingSameStopTwice()
        {
            // Arrange
            var prouth = new Prouth();
            var node = new GraphNS.Node(4, 4, typeof(Stop));

            // Act
            prouth.AddStop(node);
            prouth.AddStop(node);

            // Assert
            Assert.Equal(2, prouth.Stops.Count(n => n.Equals(node)));
        }

        [Fact]
        public void RemoveStop_DoesNothingIfStopNotPresent()
        {
            // Arrange
            var node = new GraphNS.Node(5, 5, typeof(Stop));
            var prouth = new Prouth();

            // Act
            prouth.RemoveStop(node); // Should not throw or change anything

            // Assert
            Assert.Empty(prouth.Stops);
        }

        [Fact]
        public void Constructor_WithStops_SetsStops()
        {
            // Arrange
            var node1 = new GraphNS.Node(0, 0, typeof(Stop));
            var node2 = new GraphNS.Node(1, 1, typeof(Stop));
            var stops = new List<GraphNS.Node> { node1, node2 };

            // Act
            var prouth = new Prouth(stops);

            // Assert
            Assert.Equal(stops, prouth.Stops);
        }

        [Fact]
        public void Constructor_Empty_InitializesEmptyStops()
        {
            // Act
            var prouth = new Prouth();

            // Assert
            Assert.Empty(prouth.Stops);
        }

        [Fact]
        public void AddStop_AddsNodeToStops()
        {
            // Arrange
            var prouth = new Prouth();
            var node = new GraphNS.Node(2, 2, typeof(Stop));

            // Act
            prouth.AddStop(node);

            // Assert
            Assert.Contains(node, prouth.Stops);
        }

        [Fact]
        public void RemoveStop_RemovesNodeFromStops()
        {
            // Arrange
            var node = new GraphNS.Node(3, 3, typeof(Stop));
            var prouth = new Prouth([node]);

            // Act
            prouth.RemoveStop(node);

            // Assert
            Assert.DoesNotContain(node, prouth.Stops);
        }

        [Fact]
        public void ConvertStopTilesToNodes_ReturnsNodesForStops()
        {
            // Arrange
            var stop1 = new Stop(0, 0, 0);
            var stop2 = new Stop(1, 1, 0);
            var node1 = new GraphNS.Node(0, 0, typeof(Stop));
            var node2 = new GraphNS.Node(1, 1, typeof(Stop));
            var graph = new GraphNS.Graph([]);
            graph.AddNode(node1);
            graph.AddNode(node2);
            var stops = new List<Stop> { stop1, stop2 };

            // Act
            var nodes = ProuthUtil.ConvertStopTilesToNodes(stops, graph);

            // Assert
            Assert.Contains(node1, nodes);
            Assert.Contains(node2, nodes);
            Assert.Equal(2, nodes.Count);
        }

        [Fact]
        public void ConvertNodestoStopTiles_ReturnsStopsForNodes()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var stop1 = new Stop(0, 0, 0);
            var stop2 = new Stop(1, 1, 0);
            gameTable.Table[0, 0] = stop1;
            gameTable.Table[1, 1] = stop2;
            var node1 = new GraphNS.Node(0, 0, typeof(Stop));
            var node2 = new GraphNS.Node(1, 1, typeof(Stop));
            var nodes = new List<GraphNS.Node> { node1, node2 };

            // Act
            var stops = ProuthUtil.ConvertNodestoStopTiles(nodes, gameTable);

            // Assert
            Assert.Contains(stop1, stops);
            Assert.Contains(stop2, stops);
            Assert.Equal(2, stops.Count);
        }

        [Fact]
        public void ConvertNodestoStopTiles_IgnoresNullFields()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(1, 1, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var node = new GraphNS.Node(0, 0, typeof(Stop));
            var nodes = new List<GraphNS.Node> { node };

            // Act
            var stops = ProuthUtil.ConvertNodestoStopTiles(nodes, gameTable);

            // Assert
            Assert.Empty(stops);
        }

        [Fact]
        public void ConvertNodestoStopTiles_IgnoresNonStopFields()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 1, 0, new MapGenerationSettings());
            var gameTable = new GameTable(mapGen, context);
            var road = new Road(0, 0, RoadType.Horizontal, 0);
            gameTable.Table[0, 0] = road;

            var stop = new Stop(1, 0, 0);
            gameTable.Table[1, 0] = stop;

            var roadNode = new GraphNS.Node(0, 0, typeof(Road));
            var stopNode = new GraphNS.Node(1, 0, typeof(Stop));
            var nodes = new List<GraphNS.Node> { roadNode, stopNode };

            // Act
            var stops = ProuthUtil.ConvertNodestoStopTiles(nodes, gameTable);

            // Assert
            Assert.Single(stops);
            Assert.Contains(stop, stops);
            Assert.DoesNotContain(stops, x => x.X == road.X && x.Y == road.Y);
        }
    }
}
