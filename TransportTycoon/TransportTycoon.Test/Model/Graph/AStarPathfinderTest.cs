using TransportTycoon.MapData;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class AStarPathfinderTest
    {
        [Fact]
        public void FindPath_ReturnsNull_WhenNoPathExists()
        {
            // Arrange
            var nodeA = new GraphNS.Node(0, 0, typeof(Stop));
            var nodeB = new GraphNS.Node(1, 1, typeof(Stop));
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { nodeA, [] },
                { nodeB, [] }
            };
            var graph = new GraphNS.Graph(adjacencyList);
            var pathfinder = new GraphNS.AStarPathfinder(graph);

            // Act
            var path = pathfinder.FindPath(nodeA, nodeB);

            // Assert
            Assert.Null(path);
        }

        [Fact]
        public void FindPath_ReturnsNull_WhenStartNodeIsNotInAdjacencyList()
        {
            // Arrange
            var nodeA = new GraphNS.Node(0, 0, typeof(Stop));
            var nodeB = new GraphNS.Node(1, 0, typeof(Stop));
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { nodeB, [] }
            };
            var graph = new GraphNS.Graph(adjacencyList);
            var pathfinder = new GraphNS.AStarPathfinder(graph);

            // Act
            var path = pathfinder.FindPath(nodeA, nodeB);

            // Assert
            Assert.Null(path);
        }

        [Fact]
        public void FindPath_ReturnsSingleEdgePath_WhenDirectEdgeExists()
        {
            // Arrange
            var nodeA = new GraphNS.Node(0, 0, typeof(Stop));
            var nodeB = new GraphNS.Node(1, 0, typeof(Stop));
            var edge = new GraphNS.Edge(nodeA, nodeB, [], 1.0);
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { nodeA, [edge] },
                { nodeB, [] }
            };
            var graph = new GraphNS.Graph(adjacencyList);
            var pathfinder = new GraphNS.AStarPathfinder(graph);

            // Act
            var path = pathfinder.FindPath(nodeA, nodeB);

            // Assert
            Assert.NotNull(path);
            Assert.Single(path);
            Assert.Equal(edge, path[0]);
        }

        [Fact]
        public void FindPath_ReturnsShortestPath_WhenMultiplePathsExist()
        {
            // Arrange
            var nodeA = new GraphNS.Node(0, 0, typeof(Stop));
            var nodeB = new GraphNS.Node(1, 0, typeof(Stop));
            var nodeC = new GraphNS.Node(2, 0, typeof(Stop));
            var edgeAB = new GraphNS.Edge(nodeA, nodeB, [], 1.0);
            var edgeBC = new GraphNS.Edge(nodeB, nodeC, [], 1.0);
            var edgeAC = new GraphNS.Edge(nodeA, nodeC, [], 5.0);
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { nodeA, [edgeAB, edgeAC] },
                { nodeB, [edgeBC] },
                { nodeC, [] }
            };
            var graph = new GraphNS.Graph(adjacencyList);
            var pathfinder = new GraphNS.AStarPathfinder(graph);

            // Act
            var path = pathfinder.FindPath(nodeA, nodeC);

            // Assert
            Assert.NotNull(path);
            Assert.Equal(2, path.Count);
            Assert.Equal(edgeAB, path[0]);
            Assert.Equal(edgeBC, path[1]);
        }

        [Fact]
        public void FindPath_UsesCache_ForRepeatedQueries()
        {
            // Arrange
            var nodeA = new GraphNS.Node(0, 0, typeof(Stop));
            var nodeB = new GraphNS.Node(1, 0, typeof(Stop));
            var edge = new GraphNS.Edge(nodeA, nodeB, [], 1.0);
            var adjacencyList = new Dictionary<GraphNS.Node, List<GraphNS.Edge>>
            {
                { nodeA, [edge] },
                { nodeB, [] }
            };
            var graph = new GraphNS.Graph(adjacencyList);
            var pathfinder = new GraphNS.AStarPathfinder(graph);

            // Act
            var path1 = pathfinder.FindPath(nodeA, nodeB);
            var path2 = pathfinder.FindPath(nodeA, nodeB);

            // Assert
            Assert.NotNull(path1);
            Assert.NotNull(path2);
            Assert.Same(path1, path2); // Should be the same instance from cache
        }
    }
}
