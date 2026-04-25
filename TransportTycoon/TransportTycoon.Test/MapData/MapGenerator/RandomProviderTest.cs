using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator
{
    public class RandomProviderTest
    {
        [Fact]
        public void GetRandom_SameSeedAndPluginId_ReturnsDeterministicRandomInt()
        {
            // Arrange
            var provider = new RandomProvider();
            int seed = 12345;
            string pluginId = "TestPlugin";

            // Act
            var random1 = provider.GetRandom(seed, pluginId);
            var random2 = provider.GetRandom(seed, pluginId);

            // Assert
            Assert.NotNull(random1);
            Assert.NotNull(random2);
            // Should be different instances
            Assert.NotSame(random1, random2);

            // But they should produce the same sequence
            int testAmount = 100;
            int[] values1 = new int[testAmount];
            int[] values2 = new int[testAmount];
            for (int i = 0; i < testAmount; i++)
            {
                values1[i] = random1.Next();
                values2[i] = random2.Next();
            }
            Assert.Equal(values1, values2);
        }

        [Fact]
        public void GetRandom_SameSeedAndPluginId_ReturnsDeterministicRandomIntMax()
        {
            // Arrange
            var provider = new RandomProvider();
            int seed = 12345;
            string pluginId = "TestPlugin";

            // Act
            var random1 = provider.GetRandom(seed, pluginId);
            var random2 = provider.GetRandom(seed, pluginId);

            // Assert
            Assert.NotNull(random1);
            Assert.NotNull(random2);
            // Should be different instances
            Assert.NotSame(random1, random2);

            // But they should produce the same sequence
            int testAmount = 100;
            int maxRange = 100_000;
            int[] values1 = new int[testAmount];
            int[] values2 = new int[testAmount];
            for (int i = 0; i < testAmount; i++)
            {
                values1[i] = random1.Next(maxRange);
                values2[i] = random2.Next(maxRange);
            }
            Assert.Equal(values1, values2);
            Assert.All<int>(values1, v => Assert.InRange(v, 0, maxRange));
            Assert.All<int>(values2, v => Assert.InRange(v, 0, maxRange));
        }

        [Fact]
        public void GetRandom_SameSeedAndPluginId_ReturnsDeterministicRandomIntMinMax()
        {
            // Arrange
            var provider = new RandomProvider();
            int seed = 12345;
            string pluginId = "TestPlugin";

            // Act
            var random1 = provider.GetRandom(seed, pluginId);
            var random2 = provider.GetRandom(seed, pluginId);

            // Assert
            Assert.NotNull(random1);
            Assert.NotNull(random2);
            // Should be different instances
            Assert.NotSame(random1, random2);

            // But they should produce the same sequence
            int testAmount = 100;
            int maxRange = 100_000;
            int minRange = 50_000;
            int[] values1 = new int[testAmount];
            int[] values2 = new int[testAmount];
            for (int i = 0; i < testAmount; i++)
            {
                values1[i] = random1.Next(minRange, maxRange);
                values2[i] = random2.Next(minRange, maxRange);
            }
            Assert.Equal(values1, values2);
            Assert.All<int>(values1, v => Assert.InRange(v, minRange, maxRange));
            Assert.All<int>(values2, v => Assert.InRange(v, minRange, maxRange));
        }

        [Fact]
        public void GetRandom_SameSeedAndPluginId_ReturnsDeterministicRandomDouble()
        {
            // Arrange
            var provider = new RandomProvider();
            int seed = 12345;
            string pluginId = "TestPlugin";

            // Act
            var random1 = provider.GetRandom(seed, pluginId);
            var random2 = provider.GetRandom(seed, pluginId);

            // Assert
            Assert.NotNull(random1);
            Assert.NotNull(random2);
            // Should be different instances
            Assert.NotSame(random1, random2);

            // But they should produce the same sequence
            int testAmount = 100;
            double[] values1 = new double[testAmount];
            double[] values2 = new double[testAmount];
            for (int i = 0; i < testAmount; i++)
            {
                values1[i] = random1.NextDouble();
                values2[i] = random2.NextDouble();
            }
            Assert.Equal(values1, values2);
        }

        [Fact]
        public void GetRandom_SameSeedAndPluginId_ReturnsDeterministicRandomFloat()
        {
            // Arrange
            var provider = new RandomProvider();
            int seed = 12345;
            string pluginId = "TestPlugin";

            // Act
            var random1 = provider.GetRandom(seed, pluginId);
            var random2 = provider.GetRandom(seed, pluginId);

            // Assert
            Assert.NotNull(random1);
            Assert.NotNull(random2);
            // Should be different instances
            Assert.NotSame(random1, random2);

            // But they should produce the same sequence
            int testAmount = 100;
            float[] values1 = new float[testAmount];
            float[] values2 = new float[testAmount];
            for (int i = 0; i < testAmount; i++)
            {
                values1[i] = random1.NextSingle();
                values2[i] = random2.NextSingle();
            }
            Assert.Equal(values1, values2);
        }
    }
}
