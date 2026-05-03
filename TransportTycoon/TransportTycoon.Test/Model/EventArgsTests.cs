using TransportTycoon.Model;

namespace TransportTycoon.Test.Model
{
    public class EventArgsTests
    {
        [Fact]
        public void TransportTycoonEventArgs_PropertiesAreSetCorrectly()
        {
            // Arrange & Act
            var args = new TransportTycoonEventArgs(100, 5, 250);

            // Assert
            Assert.Equal(100ul, args.GameTime);
            Assert.Equal(5, args.NumberOfVehicles);
            Assert.Equal(250, args.Maintenance);
        }

        [Fact]
        public void TransportTycoonFieldEventArgs_PropertiesAreSetCorrectly()
        {
            // Arrange & Act
            var args = new TransportTycoonFieldEventArgs(10, 20);

            // Assert
            Assert.Equal(10, args.X);
            Assert.Equal(20, args.Y);
        }
    }
}
