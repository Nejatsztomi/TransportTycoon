using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class RouteKeyTest
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var start = new GraphNS.Node(1, 2, typeof(object));
            var end = new GraphNS.Node(3, 4, typeof(object));

            // Act
            var key = new GraphNS.RouteKey(start, end);

            // Assert
            Assert.Equal(start, key.Start);
            Assert.Equal(end, key.End);
        }
    }
}
