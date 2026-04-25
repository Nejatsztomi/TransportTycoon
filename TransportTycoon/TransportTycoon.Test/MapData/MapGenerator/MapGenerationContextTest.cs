using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGenerationContextTest
{
    public class ConstructorTest
    {
        [Theory]
        [InlineData(100, 50, 42)]
        [InlineData(0, 0, 0)]
        [InlineData(100, 100, -1)]
        public void MapGenerationContext_CanBeCreatedWithParameters(int width, int height, int seed)
        {
            // Act
            MapGenerationContext context = new(width, height, seed, new MapGenerationSettings());

            // Assert
            Assert.Equal(width, context.Width);
            Assert.Equal(height, context.Height);
            Assert.Equal(seed, context.Seed);
        }
        // TODO: Add tests for invalid parameters (e.g., negative width/height) if the constructor is expected to throw exceptions
    }
}
