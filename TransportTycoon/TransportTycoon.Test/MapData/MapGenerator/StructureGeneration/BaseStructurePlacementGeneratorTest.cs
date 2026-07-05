using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.StructureGeneration;

public class BaseStructurePlacementGeneratorTest
{
    [Theory]
    [InlineData(-1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(-1, 1, 10)]
    [InlineData(1, -1, 10)]
    public void TryPlace_WithEmptyMap_AndNoNearPlacement_StillPlacesStructure(int centerX, int centerY, int cityRange)
    {
        // Arrange
        MapGenerationContext context = CreateContext(cityRange);
        ScriptedRandomProvider randomProvider = new([0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        bool result = generator.TryPlacePublic(city, context, centerX, centerY, randomProvider.Random, validPoints);

        // Assert
        Assert.True(result);
        Assert.Equal(16, city.MapPoints.Count);
        AssertStructurePlaced(context, city);
        Assert.DoesNotContain(randomProvider.Random.RangeCalls, call => call.Min < 0);
    }

    [Fact]
    public void TryPlaceNear_WithWaterFilledMap_ReturnsFalseAfterMultipleAttempts()
    {
        // Arrange
        MapGenerationContext context = CreateContext(1, 0, 20, 20);
        FillWaterMap(context);
        ScriptedRandomProvider randomProvider = new([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        bool result = generator.TryPlacePublic(city, context, 10, 10, randomProvider.Random, validPoints);

        // Assert
        Assert.False(result);
        Assert.Empty(city.MapPoints);
        Assert.True(randomProvider.Random.RangeCalls.Count >= 200);
    }

    [Fact]
    public void ForcePlaceNear_WithWaterFilledMap_ThrowsAfterMultipleAttempts()
    {
        // Arrange
        MapGenerationContext context = CreateContext(1, 0, 20, 20);
        FillWaterMap(context);
        ScriptedRandomProvider randomProvider = new([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act & Assert
        Assert.Throws<Exception>(() => generator.ForcePlacePublic(city, context, 10, 10, randomProvider.Random, validPoints));
        Assert.Empty(city.MapPoints);
        Assert.True(randomProvider.Random.RangeCalls.Count >= 1000);
    }

    [Theory]
    [InlineData(-1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(-1, 1, 10)]
    [InlineData(1, -1, 10)]
    public void ForcePlace_WithEmptyMap_AndNoNearPlacement_StillPlacesStructure(int centerX, int centerY, int cityRange)
    {
        // Arrange
        MapGenerationContext context = CreateContext(cityRange);
        ScriptedRandomProvider randomProvider = new([0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        generator.ForcePlacePublic(city, context, centerX, centerY, randomProvider.Random, validPoints);

        // Assert
        Assert.Equal(16, city.MapPoints.Count);
        AssertStructurePlaced(context, city);
        Assert.DoesNotContain(randomProvider.Random.RangeCalls, call => call.Min < 0);
    }

    [Theory]
    [InlineData(10, 10, 10)]
    [InlineData(8, 8, 10)]
    public void TryPlace_WhenNearPlacementIsEnabled_PlacesStructureNearCenter(int centerX, int centerY, int cityRange)
    {
        // Arrange
        MapGenerationContext context = CreateContext(cityRange, 0, 20, 20);
        ScriptedRandomProvider randomProvider = new([0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        bool result = generator.TryPlacePublic(city, context, centerX, centerY, randomProvider.Random, validPoints);

        // Assert
        Assert.True(result);
        Assert.Equal((centerX, centerY), city.TopLeftPoints);
        AssertStructurePlaced(context, city);
        Assert.Contains(randomProvider.Random.RangeCalls, call => call.Min == -cityRange && call.Max == cityRange + 1);
    }

    [Theory]
    [InlineData(10, 10, 10)]
    [InlineData(8, 8, 10)]
    public void ForcePlace_WhenNearPlacementIsEnabled_PlacesStructureNearCenter(int centerX, int centerY, int cityRange)
    {
        // Arrange
        MapGenerationContext context = CreateContext(cityRange, 0, 20, 20);
        ScriptedRandomProvider randomProvider = new([0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        generator.ForcePlacePublic(city, context, centerX, centerY, randomProvider.Random, validPoints);

        // Assert
        Assert.Equal((centerX, centerY), city.TopLeftPoints);
        AssertStructurePlaced(context, city);
        Assert.Contains(randomProvider.Random.RangeCalls, call => call.Min == -cityRange && call.Max == cityRange + 1);
    }

    [Fact]
    public void ForcePlace_WhenNoValidTileExists_ThrowsException()
    {
        // Arrange
        MapGenerationContext context = CreateContext(0, 0, 20, 20);
        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                context.WaterMap[x, y] = true;
            }
        }

        ScriptedRandomProvider randomProvider = new([0, 0, 0, 0, 0, 0, 0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act & Assert
        Assert.Throws<Exception>(() => generator.ForcePlacePublic(city, context, -1, -1, randomProvider.Random, validPoints));
    }

    [Fact]
    public void TryPlace_OnHeightFourOnlyMap_ReturnsFalse()
    {
        // Arrange
        MapGenerationContext context = CreateContext(0);
        FillHeightMap(context, 4);
        ScriptedRandomProvider randomProvider = new([0, 0, 0, 0, 0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        bool result = generator.TryPlacePublic(city, context, -1, -1, randomProvider.Random, validPoints);

        // Assert
        Assert.False(result);
        Assert.Empty(city.MapPoints);
    }

    [Fact]
    public void TryPlace_OnVaryingHeightMap_ReturnsFalse()
    {
        // Arrange
        MapGenerationContext context = CreateContext(0);
        FillVaryingHeightMap(context);
        ScriptedRandomProvider randomProvider = new([0, 0, 0, 0, 0, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        bool result = generator.TryPlacePublic(city, context, -1, -1, randomProvider.Random, validPoints);

        // Assert
        Assert.False(result);
        Assert.Empty(city.MapPoints);
    }

    [Fact]
    public void ForcePlace_WhenTerraformingFailsDueToSteepHeightPositive_ThrowsException()
    {
        // Arrange
        // A 4x4 map means there is exactly ONE valid coordinate to start: (0,0)
        MapGenerationContext context = CreateContext(0, 0, 4, 4);

        // Fill the map with 0s, but place a '4' in the corner.
        // The average height will be 4/16 = 0.
        // When TryTerraformAndPlace checks the '4' against the target height '0', 
        // it exceeds the allowed +1 difference, forcing IsNeighbouringHeightValid to return false.
        FillHeightMap(context, 0);
        context.HeightMap[0, 0] = 4;

        ScriptedRandomProvider randomProvider = new([0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act & Assert
        // Because the only coordinate (0,0) fails terraforming, validPoints empties out and it throws.
        Assert.Throws<Exception>(() => generator.ForcePlacePublic(city, context, -1, -1, randomProvider.Random, validPoints));
    }

    [Fact]
    public void ForcePlace_WhenTerraformingFailsDueToSteepHeightNegative_ThrowsException()
    {
        // Arrange
        // A 4x4 map means there is exactly ONE valid coordinate to start: (0,0)
        MapGenerationContext context = CreateContext(0, 0, 4, 4);

        // Fill the map with 0s, but place a '4' in the corner.
        // The average height will be 4/16 = 0.
        // When TryTerraformAndPlace checks the '4' against the target height '0', 
        // it exceeds the allowed +1 difference, forcing IsNeighbouringHeightValid to return false.
        FillHeightMap(context, 4);
        context.HeightMap[0, 0] = 0;

        ScriptedRandomProvider randomProvider = new([0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act & Assert
        // Because the only coordinate (0,0) fails terraforming, validPoints empties out and it throws.
        Assert.Throws<Exception>(() => generator.ForcePlacePublic(city, context, -1, -1, randomProvider.Random, validPoints));
    }

    [Fact]
    public void ForcePlaceNear_WhenNearAreaIsBlocked_FallsBackToForcePlaceAndSucceeds()
    {
        // Arrange
        MapGenerationContext context = CreateContext(1, 0, 20, 20); // CityRange = 1

        // Block ONLY the "near" area around center (10,10) with water
        for (int x = 9; x <= 11; x++)
        {
            for (int y = 9; y <= 11; y++)
            {
                context.WaterMap[x, y] = true;
            }
        }

        ScriptedRandomProvider randomProvider = new([0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        // This will try near 10,10 for 500 attempts, fail due to the water block,
        // and successfully fall back to ForcePlace(-1, -1) to place it elsewhere.
        generator.ForcePlacePublic(city, context, 10, 10, randomProvider.Random, validPoints);

        // Assert
        Assert.Equal(16, city.MapPoints.Count); // Ensure it was placed
        (int topLeftX, int topLeftY) = city.TopLeftPoints;
        Assert.False(context.WaterMap[topLeftX, topLeftY]); // Ensure it placed outside the water area
    }

    [Fact]
    public void GetStartPosition_WhenDistanceIsLessThanMinRange_SkipsAttempt()
    {
        // Arrange
        // We set MinCityRange to 5, so a distance of 0 (dx=0, dy=0) will trigger 'distance < minRange'
        MapGenerationContext context = CreateContext(maxCityRange: 10, minCityRange: 5, width: 20, height: 20);

        // First attempt: dx=0, dy=0 (Distance 0 - fails minRange check)
        // Second attempt: dx=10, dy=0 (Distance 10 - succeeds and places)
        ScriptedRandomProvider randomProvider = new([0, 0, 10, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        generator.ForcePlacePublic(city, context, 10, 10, randomProvider.Random, validPoints);

        // Assert
        // The first attempt was skipped, so it should have placed at the second attempt coordinates
        Assert.Equal((16, 10), city.TopLeftPoints);
    }

    [Fact]
    public void GetStartPosition_WhenDistanceIsGreaterThanMaxRange_SkipsAttempt()
    {
        // Arrange
        MapGenerationContext context = CreateContext(maxCityRange: 10, width: 30, height: 30);

        // First attempt: dx=10, dy=10 (Distance ~14.14 - fails maxRange check)
        // Second attempt: dx=10, dy=0 (Distance 10 - succeeds and places)
        ScriptedRandomProvider randomProvider = new([10, 10, 10, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);
        CityEntity city = new(4, 4);
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, city.Width, city.Height);

        // Act
        generator.ForcePlacePublic(city, context, 10, 10, randomProvider.Random, validPoints);

        // Assert
        // The first attempt was skipped, so it should have placed at the second attempt coordinates
        Assert.Equal((20, 10), city.TopLeftPoints);
    }

    [Theory]
    [InlineData(false)] // Tests TryPlaceNear
    [InlineData(true)]  // Tests ForcePlaceNear
    public void PlaceNear_WithNonCityEntity_UsesStructureRange(bool useForcePlace)
    {
        // Arrange
        // We set CityRange > 0 to pass the outer TryPlace/ForcePlace 'if' condition.
        MapGenerationSettings settings = new()
        {
            MinCityRange = 0,
            MaxCityRange = 10,
            MinStructureRange = 5,
            MaxStructureRange = 10,
            CityWidth = 4,
            CityHeight = 4,
        };
        MapGenerationContext context = new(20, 20, 42, settings);

        // dx=10, dy=0 -> Distance is 10. This perfectly hits the boundary of MaxStructureRange.
        ScriptedRandomProvider randomProvider = new([10, 0]);
        TestStructureGenerator generator = CreateGenerator(context, randomProvider);

        // Use a Dummy Structure to trigger the '_' default switch arm
        DummyBuilding dummyBuilding = new();
        List<(int X, int Y)> validPoints = generator.GetValidPointsForPlacementPublic(context, 2, 2);

        // Act
        if (useForcePlace)
        {
            generator.ForcePlacePublic(dummyBuilding, context, 10, 10, randomProvider.Random, validPoints);
            // If it throws an exception, the test fails, which covers our "always pass" requirement.
        }
        else
        {
            bool result = generator.TryPlacePublic(dummyBuilding, context, 10, 10, randomProvider.Random, validPoints);
            Assert.True(result);
        }

        // Assert
        Assert.Equal((18, 10), dummyBuilding.TopLeft);
    }

    private sealed class DummyBuilding : BuildingEntity
    {
        // Adjust these properties if your base class requires them in a constructor
        public new int Width => 2;
        public new int Height => 2;

        // A simple property to track where it was placed during the test
        public (int X, int Y) TopLeft { get; private set; }

        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            TopLeft = (startX, startY);
        }

        public override Load? GetConsumeLoad() => null;

        public override Load GetProvideLoad() => new Wheat();
    }

    private static MapGenerationContext CreateContext(int maxCityRange, int minCityRange = 0, int width = 7, int height = 7)
    {
        MapGenerationSettings settings = new()
        {
            MinCityRange = minCityRange,
            MaxCityRange = maxCityRange,
            MinStructureRange = 0,
            MaxStructureRange = 0,
            CityWidth = 4,
            CityHeight = 4,
        };

        return new MapGenerationContext(width, height, 42, settings);
    }

    private static void FillWaterMap(MapGenerationContext context)
    {
        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                context.WaterMap[x, y] = true;
            }
        }
    }

    private static void FillHeightMap(MapGenerationContext context, int height)
    {
        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                context.HeightMap[x, y] = height;
            }
        }
    }

    private static void FillVaryingHeightMap(MapGenerationContext context)
    {
        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                context.HeightMap[x, y] = x + y;
            }
        }
    }

    private static TestStructureGenerator CreateGenerator(MapGenerationContext context, ScriptedRandomProvider randomProvider)
        => new(randomProvider, context);

    private static void AssertStructurePlaced(MapGenerationContext context, CityEntity city)
    {
        (int topLeftX, int topLeftY) = city.TopLeftPoints;

        for (int x = 0; x < city.Width; x++)
        {
            for (int y = 0; y < city.Height; y++)
            {
                int mapX = topLeftX + x;
                int mapY = topLeftY + y;

                Assert.True(context.StructureMap[mapX, mapY]);
                Assert.True(city.MapPoints.ContainsKey((mapX, mapY)));
                Assert.IsType<House>(city.MapPoints[(mapX, mapY)]);
            }
        }
    }

    private sealed class ScriptedRandomProvider : IRandomProvider
    {
        public ScriptedRandom Random { get; }

        public ScriptedRandomProvider(IEnumerable<int> rangeValues)
        {
            Random = new ScriptedRandom(rangeValues);
        }

        public IRandom GetRandom(int seed, string pluginId) => Random;
    }

    private sealed class ScriptedRandom : IRandom
    {
        private readonly Queue<int> _rangeValues = new();
        public List<(int Min, int Max)> RangeCalls { get; } = [];

        public ScriptedRandom(IEnumerable<int> rangeValues)
        {
            foreach (int value in rangeValues)
            {
                _rangeValues.Enqueue(value);
            }
        }

        public int Next() => 0;

        public int Next(int maxValue) => 0;

        public int Next(int minValue, int maxValue)
        {
            RangeCalls.Add((minValue, maxValue));

            if (_rangeValues.Count > 0)
            {
                return _rangeValues.Dequeue();
            }

            return minValue;
        }

        public float NextSingle() => 0f;

        public double NextDouble() => 0d;
    }

    private sealed class TestStructureGenerator : BaseStructurePlacementGenerator
    {
        public TestStructureGenerator(IRandomProvider randomProvider, MapGenerationContext context) : base(randomProvider, context)
        {
        }

        public override GenerationPhase Phase => GenerationPhase.Structures;

        public override List<BuildingEntity> GenerateStructures(MapGenerationContext context) => [];

        public bool TryPlacePublic(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
            => TryPlace(buildingEntity, context, centerX, centerY, random, validPoints);

        public void ForcePlacePublic(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
            => ForcePlace(buildingEntity, context, centerX, centerY, random, validPoints);

        public List<(int X, int Y)> GetValidPointsForPlacementPublic(MapGenerationContext context, int buildingWidth, int buildingHeight)
            => GetValidPointsForPlacement(context, buildingWidth, buildingHeight);
    }
}
