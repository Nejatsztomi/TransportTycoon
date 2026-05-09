using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.StructureGeneration;

public class CityGeneratorTest
{
    [Fact]
    public void CityGeneratorFactory_Create_SetsStructurePhase()
    {
        // Arrange
        IRandomProvider randomProvider = new RandomProvider();
        MapGenerationContext context = CreateContext(30, 30, 42, minCities: 2, maxCities: 2);

        // Act
        IStructureGenerator result = CityGeneratorFactory.Create(randomProvider, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GenerationPhase.Structures, result.Phase);
    }

    [Fact]
    public void GenerateStructures_WhenMinCitiesEqualsMaxCities_PlacesAllCities()
    {
        // Arrange
        MapGenerationContext context = CreateContext(30, 30, 42, minCities: 2, maxCities: 2);
        IStructureGenerator generator = CityGeneratorFactory.Create(new RandomProvider(), context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(2, structures.Count);
        AssertAllStructuresPlaced(context, structures);
    }

    [Fact]
    public void GenerateStructures_WhenMinCitiesIsLessThanMaxCities_PlacesAllCities()
    {
        // Arrange
        MapGenerationContext context = CreateContext(30, 30, 42, minCities: 2, maxCities: 5);
        IStructureGenerator generator = CityGeneratorFactory.Create(new RandomProvider(), context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(5, structures.Count);
        AssertAllStructuresPlaced(context, structures);
    }

    [Fact]
    public void GenerateStructures_WhenMoreCitiesCouldBeGenerated_ButNoSpaceRemains_StopsAtPlacedCities()
    {
        // Arrange
        MapGenerationContext context = CreateContext(10, 5, 42, minCities: 2, maxCities: 100);
        IStructureGenerator generator = CityGeneratorFactory.Create(new AlwaysMinimumRandomProvider(), context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(2, structures.Count);
        AssertAllStructuresPlaced(context, structures);
    }

    [Fact]
    public void GenerateStructures_CarveExit_WhenDirectionIsUp_PlacesRoadsInVerticalLine()
    {
        // Arrange
        ScriptedRandomProvider randomProvider = new([0, 0], [0.0, 0.0, 0.0, 0.0, 0.0]);
        MapGenerationContext context = CreateContext(10, 5, 42, minCities: 2, maxCities: 2, branchCount: 1, roadLength: 1);
        IStructureGenerator generator = CityGeneratorFactory.Create(randomProvider, context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(2, structures.Count);
        CityEntity city = Assert.IsType<CityEntity>(structures[0]);
        (int topLeftX, int topLeftY) = city.TopLeftPoints;
        int centerX = topLeftX + city.Width / 2;
        int centerY = topLeftY + city.Height / 2;

        AssertRoadPath(city, [(centerX, centerY), (centerX, centerY - 1), (centerX, centerY - 2)]);
    }

    [Fact]
    public void GenerateStructures_CarveExit_WhenDirectionIsRight_PlacesRoadsInHorizontalLine()
    {
        // Arrange
        ScriptedRandomProvider randomProvider = new([1, 0], [0.0, 0.0, 0.0, 0.0, 0.0]);
        MapGenerationContext context = CreateContext(10, 5, 42, minCities: 2, maxCities: 2, branchCount: 1, roadLength: 1);
        IStructureGenerator generator = CityGeneratorFactory.Create(randomProvider, context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(2, structures.Count);
        CityEntity city = Assert.IsType<CityEntity>(structures[0]);
        (int topLeftX, int topLeftY) = city.TopLeftPoints;
        int centerX = topLeftX + city.Width / 2;
        int centerY = topLeftY + city.Height / 2;

        AssertRoadPath(city, [(centerX, centerY), (centerX + 1, centerY), (centerX + 2, centerY)]);
    }

    [Fact]
    public void GenerateStructures_CarveExit_WhenSideStepOccurs_PlacesRoadsInBothDirections()
    {
        // Arrange
        ScriptedRandomProvider randomProvider = new([0, 0], [0.9, 0.6, 0.0, 0.0, 0.0, 0.0]);
        MapGenerationContext context = CreateContext(10, 5, 42, minCities: 2, maxCities: 2, branchCount: 1, roadLength: 1);
        IStructureGenerator generator = CityGeneratorFactory.Create(randomProvider, context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(2, structures.Count);
        CityEntity city = Assert.IsType<CityEntity>(structures[0]);
        (int topLeftX, int topLeftY) = city.TopLeftPoints;
        int centerX = topLeftX + city.Width / 2;
        int centerY = topLeftY + city.Height / 2;

        AssertRoadPath(city, [(centerX, centerY), (centerX + 1, centerY), (centerX + 1, centerY - 1), (centerX + 1, centerY - 2)]);
    }

    private static MapGenerationContext CreateContext(int width, int height, int seed, int minCities, int maxCities, int branchCount = 3, int roadLength = 10)
    {
        MapGenerationSettings settings = new()
        {
            MinCities = minCities,
            MaxCities = maxCities,
            CityWidth = 5,
            CityHeight = 5,
            MinCityRange = 0,
            MaxCityRange = 0,
            BranchCount = branchCount,
            RoadLength = roadLength,
        };

        return new MapGenerationContext(width, height, seed, settings);
    }

    private static void AssertAllStructuresPlaced(MapGenerationContext context, List<BuildingEntity> structures)
    {
        int occupiedCells = 0;

        foreach (BuildingEntity structure in structures)
        {
            Assert.IsType<CityEntity>(structure);
            Assert.Equal(context.Settings.CityWidth * context.Settings.CityHeight, structure.MapPoints.Count);

            foreach ((int x, int y) in structure.MapPoints.Keys)
            {
                Assert.True(context.StructureMap[x, y], $"Expected city placement at ({x}, {y}).");
                occupiedCells++;
            }
        }

        int structureMapCells = 0;
        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                if (context.StructureMap[x, y])
                {
                    structureMapCells++;
                }
            }
        }

        Assert.Equal(occupiedCells, structureMapCells);
    }

    private static void AssertRoadPath(CityEntity city, params (int X, int Y)[] expectedRoads)
    {
        HashSet<(int X, int Y)> expectedRoadSet = [.. expectedRoads];
        int roadCount = 0;

        for (int x = 0; x < city.Width; x++)
        {
            for (int y = 0; y < city.Height; y++)
            {
                (int currentX, int currentY) = (city.TopLeftPoints.X + x, city.TopLeftPoints.Y + y);
                IField field = city.MapPoints[(currentX, currentY)];

                if (expectedRoadSet.Contains((currentX, currentY)))
                {
                    Assert.IsType<Road>(field);
                    roadCount++;
                }
                else
                {
                    Assert.IsType<House>(field);
                }
            }
        }

        Assert.Equal(expectedRoads.Length, roadCount);
    }

    private sealed class AlwaysMinimumRandomProvider : IRandomProvider
    {
        public IRandom GetRandom(int seed, string pluginId) => new AlwaysMinimumRandom();
    }

    private sealed class AlwaysMinimumRandom : IRandom
    {
        public int Next() => 0;
        public int Next(int maxValue) => 0;
        public int Next(int minValue, int maxValue) => minValue;
        public float NextSingle() => 0f;
        public double NextDouble() => 0d;
    }

    private sealed class ScriptedRandomProvider : IRandomProvider
    {
        private readonly ScriptedRandom _random;

        public ScriptedRandomProvider(int[] directions, double[] rolls)
        {
            _random = new ScriptedRandom(directions, rolls);
        }

        public IRandom GetRandom(int seed, string pluginId) => _random;
    }

    private sealed class ScriptedRandom : IRandom
    {
        private readonly Queue<int> _directions = new();
        private readonly Queue<double> _doubleRolls = new();

        public ScriptedRandom(int[] directions, double[] rolls)
        {
            foreach (int direction in directions)
            {
                _directions.Enqueue(direction);
            }

            foreach (double roll in rolls)
            {
                _doubleRolls.Enqueue(roll);
            }
        }

        public int Next() => 0;

        public int Next(int maxValue)
        {
            if (maxValue == 4 && _directions.Count > 0)
            {
                return _directions.Dequeue();
            }

            return 0;
        }

        public int Next(int minValue, int maxValue) => minValue;

        public float NextSingle() => 0f;

        public double NextDouble() => _doubleRolls.Count > 0 ? _doubleRolls.Dequeue() : 0d;
    }
}
