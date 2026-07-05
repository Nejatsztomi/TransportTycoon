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
    }

    public class MapGenerationContextDataConstructorTest
    {
        [Theory]
        [InlineData(100, 50, 42)]
        [InlineData(1, 1, 0)]
        public void MapGenerationContextData_CanBeCreatedWithParameters(int width, int height, int seed)
        {
            // Act
            MapGenerationContextData contextData = new(width, height, seed, new MapGenerationSettings());

            // Assert
            Assert.Equal(width, contextData.Width);
            Assert.Equal(height, contextData.Height);
            Assert.Equal(seed, contextData.Seed);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void MapGenerationContextData_ThrowsWhenWidthIsInvalid(int width)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGenerationContextData(width, 100, 0, new MapGenerationSettings()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void MapGenerationContextData_ThrowsWhenHeightIsInvalid(int height)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGenerationContextData(100, height, 0, new MapGenerationSettings()));
        }

        [Theory]
        [InlineData(0, 100, 0)]
        [InlineData(-1, 100, 0)]
        public void MapGenerationContextData_ThrowsWhenWidthHasWrongValue(int width, int height, int seed)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGenerationContextData(width, height, seed, new MapGenerationSettings()));
        }

        [Theory]
        [InlineData(100, 0, 0)]
        [InlineData(100, -1, 0)]
        public void MapGenerationContextData_ThrowsWhenHeightHasWrongValue(int width, int height, int seed)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGenerationContextData(width, height, seed, new MapGenerationSettings()));
        }

        [Theory]
        [InlineData(100, 50, 42)]
        [InlineData(1, 1, 0)]
        public void MapGenerationContextData_CanBeCreatedFromMapGenerationContext(int width, int height, int seed)
        {
            // Arrange
            MapGenerationContext context = new(width, height, seed, new MapGenerationSettings());

            // Act
            MapGenerationContextData contextData = new(context);

            // Assert
            Assert.Equal(context.Width, contextData.Width);
            Assert.Equal(context.Height, contextData.Height);
            Assert.Equal(context.Seed, contextData.Seed);
            Assert.Same(context.Settings, contextData.Settings);
        }
    }
}
