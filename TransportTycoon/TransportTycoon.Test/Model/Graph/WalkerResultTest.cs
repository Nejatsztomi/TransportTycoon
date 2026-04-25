using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class WalkerResultTest
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var edge = new GraphNS.Edge(new GraphNS.Node(1, 2, typeof(object)), new GraphNS.Node(3, 4, typeof(object)), [], 1.0);
            // Walker has no public constructor, so use a minimal valid list (empty) for newWalkers
            var node = new GraphNS.Node(5, 6, typeof(object));
            var createdEdges = new List<GraphNS.Edge> { edge };
            var newWalkers = new List<GraphNS.Walker>();
            GraphNS.Node? endNode = node;

            // Act
            var result = new GraphNS.WalkerResult(createdEdges, newWalkers, endNode);

            // Assert
            Assert.Equal(createdEdges, result.CreatedEdges);
            Assert.Equal(newWalkers, result.NewWalkers);
            Assert.Equal(endNode, result.EndNode);
        }
    }
}
