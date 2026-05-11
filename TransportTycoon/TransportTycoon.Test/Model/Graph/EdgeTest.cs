using NSubstitute;
using TransportTycoon.MapData;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class EdgeTest
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var start = new GraphNS.Node(1, 2, typeof(Stop));
            var end = new GraphNS.Node(3, 4, typeof(Stop));
            var roads = new List<Field> { Substitute.For<Field>() };
            double cost = 5.5;

            // Act
            var edge = new GraphNS.Edge(start, end, roads, cost);

            // Assert
            Assert.Equal(start, edge.StartNode);
            Assert.Equal(end, edge.EndNode);
            Assert.Equal(roads, edge.Roads);
            Assert.Equal(cost, edge.Cost);
        }

        [Fact]
        public void Roads_CanBeEmpty()
        {
            // Arrange
            var start = new GraphNS.Node(1, 2, typeof(Stop));
            var end = new GraphNS.Node(3, 4, typeof(Stop));
            var roads = new List<Field>();
            double cost = 1.0;

            // Act
            var edge = new GraphNS.Edge(start, end, roads, cost);

            // Assert
            Assert.Empty(edge.Roads);
        }
    }
}
