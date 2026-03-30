using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class INoiseGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void PerlinNoiseFactoryCreate_WithAllParameters() { }
    }

    public class NoiseGenerationTest
    {
        [TestClass]
        public class PerlinNoiseGeneratorTest
        {
            private static INoiseGenerator _generator = null!;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                _generator = PerlinNoiseGeneratorFactory.Create(10, 10, 42);
            }

            [TestMethod]
            public void PerlinNoiseGenerate_IsCorrectSize() { }

            [TestMethod]
            public void PerlinNoiseGenerate_ValuesAreBetweenZeroAndOne() { }
        }
    }
}
