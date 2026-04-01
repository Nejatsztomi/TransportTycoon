using TransportTycoon.MapData.MapGenerator.NoiseGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class INoiseGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void PerlinNoiseFactoryCreate_WithAllParameters()
        {
            INoiseGenerator result = PerlinNoiseGeneratorFactory.Create(10, 10, 42);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<PerlinNoiseGenerator>(result);
        }
    }

    public class NoiseGenerationTest
    {
        [TestClass]
        public class PerlinNoiseGeneratorTest
        {
            private static readonly int _width = 10;
            private static readonly int _height = 15;
            private static INoiseGenerator _generator = null!;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                _generator = PerlinNoiseGeneratorFactory.Create(_width, _height, 42);
            }

            [TestMethod]
            public void PerlinNoiseGenerate_IsCorrectSize()
            {
                float[,] noise = _generator.GenerateNoise(0.1f);

                Assert.AreEqual(_width, noise.GetLength(0), "Generated noise width should be what is in the constructor");
                Assert.AreEqual(_height, noise.GetLength(1), "Generated noise height should be what is in the constructor");
            }

            [TestMethod]
            [DataRow(0.1f)]
            [DataRow(0.5f)]
            [DataRow(0.9f)]
            public void PerlinNoiseGenerate_ValuesAreBetweenZeroAndOne(float noiseScale)
            {
                float[,] noise = _generator.GenerateNoise(noiseScale);

                bool hasValuesOutsideRange = false;
                foreach (var value in noise)
                {
                    if (value < 0.0f || value > 1.0f)
                    {
                        hasValuesOutsideRange = true;
                        break;
                    }
                }

                Assert.IsFalse(hasValuesOutsideRange);
            }
        }
    }
}
