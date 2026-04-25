using TransportTycoon.MapData.MapGenerator.CoordinateHasher;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator.CoordinateHasher
{
    public class BasicCoordinateHasherTest
    {
        [Fact]
        public void Hash2D_SameInputSameSeed_ProducesSameValue()
        {
            // Arrange
            var hasher = new BasicCoordinateHasher();
            int x = 10, y = 20, seed = 42;

            // Act
            float hash1 = hasher.Hash2D(x, y, seed);
            float hash2 = hasher.Hash2D(x, y, seed);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Hash2D_SameInputDifferentSeed_ProducesDifferentValue()
        {
            // Arrange
            var hasher = new BasicCoordinateHasher();
            int x = 10, y = 20;
            int seed1 = 42, seed2 = 99;

            // Act
            float hash1 = hasher.Hash2D(x, y, seed1);
            float hash2 = hasher.Hash2D(x, y, seed2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void Hash2DRange_SameInputSameSeed_ProducesSameValue()
        {
            // Arrange
            var hasher = new BasicCoordinateHasher();
            int x = 5, y = 7, seed = 123, min = 0, max = 100;

            // Act
            int val1 = hasher.Hash2DRange(x, y, seed, min, max);
            int val2 = hasher.Hash2DRange(x, y, seed, min, max);

            // Assert
            Assert.Equal(val1, val2);
        }

        [Fact]
        public void Hash2DRange_SameInputDifferentSeed_ProducesDifferentValue()
        {
            // Arrange
            var hasher = new BasicCoordinateHasher();
            int x = 5, y = 7, min = 0, max = 100;
            int seed1 = 123, seed2 = 456;

            // Act
            int val1 = hasher.Hash2DRange(x, y, seed1, min, max);
            int val2 = hasher.Hash2DRange(x, y, seed2, min, max);

            // Assert
            Assert.NotEqual(val1, val2);
        }
    }

    public class ValueNoiseGeneratorTest
    {
        [Fact]
        public void GenerateNoise_SameInputSameSeed_ProducesSameValue()
        {
            // Arrange
            var generator = new ValueNoiseGenerator(0.05f);
            float x = 1.5f, y = 2.5f;
            int seed = 42;

            // Act
            float val1 = generator.GenerateNoise(x, y, seed);
            float val2 = generator.GenerateNoise(x, y, seed);

            // Assert
            Assert.Equal(val1, val2);
        }

        [Fact]
        public void GenerateNoise_SameInputDifferentSeed_ProducesDifferentValue()
        {
            // Arrange
            var generator = new ValueNoiseGenerator(0.05f);
            float x = 1.5f, y = 2.5f;
            int seed1 = 42, seed2 = 99;

            // Act
            float val1 = generator.GenerateNoise(x, y, seed1);
            float val2 = generator.GenerateNoise(x, y, seed2);

            // Assert
            Assert.NotEqual(val1, val2);
        }
    }
}
