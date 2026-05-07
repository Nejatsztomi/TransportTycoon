using NSubstitute;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class WaterGeneratorTest
{
    public class FactoryCreateTest
    {
        [Fact]
        public void RiverGeneratorFactory_Create_WithValidParameters()
        {
            // Arrange
            IRandomProvider randomProvider = Substitute.For<IRandomProvider>();
            MapGenerationContext context = CreateContext();

            // Act
            IWaterGenerator result = RiverGeneratorFactory.Create(randomProvider, context);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RiverGenerator>(result);
        }

        [Fact]
        public void RiverGeneratorFactory_Create_SetsWaterLayerPhase()
        {
            // Arrange
            IRandomProvider randomProvider = Substitute.For<IRandomProvider>();
            MapGenerationContext context = CreateContext();

            // Act
            IWaterGenerator result = RiverGeneratorFactory.Create(randomProvider, context);

            // Assert
            Assert.Equal(GenerationPhase.WaterLayer, result.Phase);
        }
    }

    public class GenerateWaterMapTest
    {
        private readonly IWaterGenerator _waterGenerator;
        private readonly MapGenerationContext _context;
        private readonly float[,] _noiseMap;
        private readonly bool[,] _waterMap;
        private readonly ScriptedRandom _random;

        public GenerateWaterMapTest()
        {
            _context = CreateContext();
            _noiseMap = CreateNoiseMap(_context.Width, _context.Height, 0.25f);
            _waterMap = new bool[_context.Width, _context.Height];
            _random = new ScriptedRandom(
                rangeValues: [5, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                singleValues: [0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f],
                directionValues: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
            );

            IRandomProvider randomProvider = Substitute.For<IRandomProvider>();
            randomProvider.GetRandom(Arg.Any<int>(), Arg.Any<string>()).Returns(_random);
            _waterGenerator = RiverGeneratorFactory.Create(randomProvider, _context);
        }

        [Fact]
        public void GenerateWaterMap_ReturnsCorrectDimensions()
        {
            // Act
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(_noiseMap, _waterMap, _context);

            // Assert
            Assert.Equal(_context.Width, waterMap.GetLength(0));
            Assert.Equal(_context.Height, waterMap.GetLength(1));
        }

        [Fact]
        public void GenerateWaterMap_AlreadyFullOfWater_StaysUnchanged()
        {
            // Arrange
            bool[,] waterMap = new bool[_context.Width, _context.Height];
            waterMap[5, 5] = true;
            bool[,] originalWaterMap = (bool[,])waterMap.Clone();

            // Act
            bool[,] result = _waterGenerator.GenerateWaterMap(_noiseMap, waterMap, _context);

            // Assert
            Assert.Equal(originalWaterMap, result);
        }

        [Fact]
        public void GenerateWaterMap_OnMountains_NoWaterAppears()
        {
            // Arrange
            float[,] mountainNoiseMap = CreateNoiseMap(_context.Width, _context.Height, 1f);
            bool[,] waterMap = new bool[_context.Width, _context.Height];

            // Act
            bool[,] result = _waterGenerator.GenerateWaterMap(mountainNoiseMap, waterMap, _context);

            // Assert
            for (int x = 0; x < _context.Width; x++)
            {
                for (int y = 0; y < _context.Height; y++)
                {
                    Assert.False(result[x, y], $"Water should not appear on mountains at ({x}, {y}).");
                }
            }
        }

        [Fact]
        public void GenerateWaterMap_OnLowTerrain_ProducesWater()
        {
            // Arrange
            float[,] lowNoiseMap = CreateNoiseMap(_context.Width, _context.Height, 0.1f);
            bool[,] waterMap = new bool[_context.Width, _context.Height];

            // Act
            bool[,] result = _waterGenerator.GenerateWaterMap(lowNoiseMap, waterMap, _context);

            // Assert
            bool hasWater = false;
            for (int x = 0; x < _context.Width && !hasWater; x++)
            {
                for (int y = 0; y < _context.Height && !hasWater; y++)
                {
                    hasWater = result[x, y];
                }
            }

            Assert.True(hasWater, "River generation should produce water on low terrain.");
        }
    }

    private static MapGenerationContext CreateContext()
    {
        MapGenerationSettings settings = new()
        {
            RiverCount = 1,
            MinRiverWidth = 1,
            MaxRiverWidth = 1,
        };

        return new MapGenerationContext(20, 20, 42, settings);
    }

    private static float[,] CreateNoiseMap(int width, int height, float value)
    {
        float[,] noiseMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = value;
            }
        }

        return noiseMap;
    }

    private sealed class ScriptedRandom : IRandom
    {
        private readonly Queue<int> _rangeValues = new();
        private readonly Queue<float> _singleValues = new();
        private readonly Queue<int> _directionValues = new();

        public ScriptedRandom(IEnumerable<int> rangeValues, IEnumerable<float> singleValues, IEnumerable<int> directionValues)
        {
            foreach (int value in rangeValues)
            {
                _rangeValues.Enqueue(value);
            }

            foreach (float value in singleValues)
            {
                _singleValues.Enqueue(value);
            }

            foreach (int value in directionValues)
            {
                _directionValues.Enqueue(value);
            }
        }

        public int Next() => 0;

        public int Next(int maxValue)
        {
            if (maxValue == 4 && _directionValues.Count > 0)
            {
                return _directionValues.Dequeue();
            }

            if (_directionValues.Count > 0)
            {
                return _directionValues.Dequeue();
            }

            return 0;
        }

        public int Next(int minValue, int maxValue)
        {
            if (_rangeValues.Count > 0)
            {
                return _rangeValues.Dequeue();
            }

            return minValue;
        }

        public float NextSingle() => _singleValues.Count > 0 ? _singleValues.Dequeue() : 0f;

        public double NextDouble() => 0d;
    }
}
