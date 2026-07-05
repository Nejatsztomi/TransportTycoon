using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class INoiseGeneratorTest
{
    public class FactoryCreateTest
    {
        [Fact]
        public void PerlinNoiseFactoryCreate_WithAllParameters()
        {
            // Arrange
            float frequency = 0.1f;

            // Act
            INoiseGenerator result = ValueNoiseGeneratorFactory.Create(frequency);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValueNoiseGenerator>(result);
        }
    }

    public class NoiseGenerationTest
    {
        public class ValueNoiseGenerationTest
        {
            private readonly MapGenerationContext _context;
            private readonly INoiseGenerator _generator;

            public ValueNoiseGenerationTest()
            {
                _context = new(10, 15, 0, new MapGenerationSettings());
                _generator = ValueNoiseGeneratorFactory.Create(0.1f);
            }
        }
    }
}
