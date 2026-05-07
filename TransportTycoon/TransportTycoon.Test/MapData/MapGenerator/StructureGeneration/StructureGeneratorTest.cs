using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.StructureGeneration;

public class StructureGeneratorTest
{
    [Fact]
    public void StructureGeneratorFactory_Create_SetsStructurePhase()
    {
        // Arrange
        IRandomProvider randomProvider = new RandomProvider();
        MapGenerationContext context = CreateContext(20, 20, 42, minStructure: 1, maxStructure: 1);

        // Act
        IStructureGenerator result = StructureGeneratorFactory.Create(randomProvider, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GenerationPhase.Structures, result.Phase);
    }

    [Fact]
    public void GenerateStructures_WhenMinStructureEqualsMaxStructure_PlacesAllStructures()
    {
        // Arrange
        MapGenerationContext context = CreateContext(20, 20, 42, minStructure: 4, maxStructure: 4);
        IStructureGenerator generator = StructureGeneratorFactory.Create(new RandomProvider(), context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(4, structures.Count);
        AssertAllStructuresPlaced(context, structures);
    }

    [Fact]
    public void GenerateStructures_WhenMinStructureIsLessThanMaxStructure_PlacesAllStructures()
    {
        // Arrange
        MapGenerationContext context = CreateContext(20, 20, 42, minStructure: 2, maxStructure: 5);
        IStructureGenerator generator = StructureGeneratorFactory.Create(new RandomProvider(), context);

        // Act
        List<BuildingEntity> structures = generator.GenerateStructures(context);

        // Assert
        Assert.Equal(5, structures.Count);
        AssertAllStructuresPlaced(context, structures);
    }

    private static MapGenerationContext CreateContext(int width, int height, int seed, int minStructure, int maxStructure)
    {
        MapGenerationSettings settings = new()
        {
            MinStructure = minStructure,
            MaxStructure = maxStructure,
        };

        return new MapGenerationContext(width, height, seed, settings);
    }

    private static void AssertAllStructuresPlaced(MapGenerationContext context, List<BuildingEntity> structures)
    {
        int occupiedCells = 0;

        foreach (BuildingEntity structure in structures)
        {
            Assert.Equal(structure.Width * structure.Height, structure.MapPoints.Count);

            foreach ((int x, int y) in structure.MapPoints.Keys)
            {
                Assert.True(context.StructureMap[x, y], $"Expected structure placement at ({x}, {y}).");
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
}
