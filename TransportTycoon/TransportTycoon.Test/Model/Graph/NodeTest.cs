using TransportTycoon.MapData;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class NodeTest
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            int x = 1;
            int y = 2;
            var type = typeof(Stop);

            // Act
            var node = new GraphNS.Node(x, y, type);

            // Assert
            Assert.Equal(x, node.X);
            Assert.Equal(y, node.Y);
            Assert.Equal(type, node.Type);
        }

        [Fact]
        public void IsValidDestination_True_WhenTypeIsStop()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));

            // Act
            var result = node.IsValidDestination;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidDestination_False_WhenTypeIsNotStop()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(object));

            // Act
            var result = node.IsValidDestination;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ReturnsTrue_ForSameCoordinates()
        {
            // Arrange
            var node1 = new GraphNS.Node(1, 2, typeof(Stop));
            var node2 = new GraphNS.Node(1, 2, typeof(object));

            // Act
            var result = node1.Equals(node2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ReturnsFalse_ForDifferentCoordinates()
        {
            // Arrange
            var node1 = new GraphNS.Node(1, 2, typeof(Stop));
            var node2 = new GraphNS.Node(2, 1, typeof(Stop));

            // Act
            var result = node1.Equals(node2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_ReturnsFalse_ForDifferentObject()
        {
            // Arrange
            var node = new GraphNS.Node(1, 2, typeof(Stop));
            var other = new object();

            // Act
            var result = node.Equals(other);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHashCode_SameForSameCoordinates()
        {
            // Arrange
            var node1 = new GraphNS.Node(1, 2, typeof(Stop));
            var node2 = new GraphNS.Node(1, 2, typeof(object));

            // Act
            var hash1 = node1.GetHashCode();
            var hash2 = node2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }
    }
}
