using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGenerationContextTest
{
    [TestClass]
    public class ConstructorTest
    {
        [TestMethod]
        [DataRow(100, 50, 42, DisplayName = "Normal constructor")]
        [DataRow(0, 0, 0, DisplayName = "All zero constructor")]
        [DataRow(100, 100, -1, DisplayName = "Negative seed constructor")]
        public void MapGenerationContext_CanBeCreatedWithParameters(int width, int height, int seed)
        {
            // Act
            MapGenerationContext context = new(width, height, seed, new MapGenerationSettings());

            // Assert
            Assert.AreEqual(width, context.Width, "Width should be set");
            Assert.AreEqual(height, context.Height, "Height should be set");
            Assert.AreEqual(seed, context.Seed, "Seed should be set");
        }

        // TODO: Add tests for invalid parameters (e.g., negative width/height) if the constructor is expected to throw exceptions

    }
}
