using TransportTycoon.MapData;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class GraphTest
    {
        [Fact]
        public void Constructor_WithAdjacencyList_SetsAdjacencyList()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var edge = new GraphNS.Edge(node, node, [], 1.0);
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { node, [edge] }
            };

            // Act
            var graph = new GraphNS.Graph(adjacencyList);

            // Assert
            Assert.Equal(adjacencyList, graph.AdjacencyList);
        }

        [Fact]
        public void Constructor_WithNodesAndEdges_CreatesAdjacencyList()
        {
            // Arrange
            var node1 = new GraphNS.Node(1, 2, typeof(Stop));
            var node2 = new GraphNS.Node(3, 4, typeof(Stop));
            var edge = new GraphNS.Edge(node1, node2, [], 2.0);
            var nodes = new List<GraphNS.Node> { node1, node2 };
            var edges = new List<GraphNS.Edge> { edge };

            // Act
            var graph = new GraphNS.Graph(nodes, edges);

            // Assert
            Assert.Contains(node1, graph.AdjacencyList.Keys);
            Assert.Contains(node2, graph.AdjacencyList.Keys);
            Assert.Contains(edge, graph.AdjacencyList[node1]);
            Assert.Contains(edge, graph.AdjacencyList[node2]);
        }

        [Fact]
        public void GetNodeAt_ReturnsCorrectNode()
        {
            // Arrange
            var node = new GraphNS.Node(5, 6, typeof(Stop));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { node, [] } });

            // Act
            var result = graph.GetNodeAt(5, 6);

            // Assert
            Assert.Equal(node, result);
        }

        [Fact]
        public void GetNodeAt_ReturnsNullIfNotFound()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { node, [] } });

            // Act
            var result = graph.GetNodeAt(9, 9);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddNode_AddsNodeIfNotExists()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var graph = new GraphNS.Graph([]);

            // Act
            graph.AddNode(node);

            // Assert
            Assert.Contains(node, graph.AdjacencyList.Keys);
        }

        [Fact]
        public void AddNode_DoesNotAddIfExists()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { node, [] } });

            // Act
            graph.AddNode(node);

            // Assert
            Assert.Single(graph.AdjacencyList.Keys);
        }

        [Fact]
        public void AddEdge_AddsEdgeIfNodeExists()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var graph = new GraphNS.Graph(new Dictionary<GraphNS.Node, List<GraphNS.Edge>> { { node, [] } });
            var edge = new GraphNS.Edge(node, node, [], 1.0);

            // Act
            graph.AddEdge(node, edge);

            // Assert
            Assert.Contains(edge, graph.AdjacencyList[node]);
        }

        [Fact]
        public void AddEdge_DoesNothingIfNodeNotExists()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var graph = new GraphNS.Graph([]);
            var edge = new GraphNS.Edge(node, node, [], 1.0);

            // Act
            graph.AddEdge(node, edge);

            // Assert
            Assert.False(graph.AdjacencyList.ContainsKey(node));
        }
    }
}
