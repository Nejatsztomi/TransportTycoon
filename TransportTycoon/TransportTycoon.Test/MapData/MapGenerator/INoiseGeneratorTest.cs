using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class INoiseGeneratorTest
{
    [TestClass]
    public class FactoryCreateTest
    {
        [TestMethod]
        public void PerlinNoiseFactoryCreate_WithAllParameters()
        {
            MapGenerationContext context = new(10, 15, 0, new MapGenerationSettings());
            INoiseGenerator result = PerlinNoiseGeneratorFactory.Create(new RandomProvider(), context);
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType<PerlinNoiseGenerator>(result);
        }
    }

    public class NoiseGenerationTest
    {
        public class PerlinNoiseGeneratorTest
        {
            private static readonly MapGenerationContext _context = new(10, 15, 0, new MapGenerationSettings());
            private static INoiseGenerator _generator = null!;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                _generator = PerlinNoiseGeneratorFactory.Create(new RandomProvider(), _context);
            }
        }
    }
}
