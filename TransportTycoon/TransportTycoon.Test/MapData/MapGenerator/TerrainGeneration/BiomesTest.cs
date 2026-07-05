using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class BiomesTest
{
    [Theory]
    [InlineData("DefaultBiome", 0.3f, 0.55f, 0.75f, 0.95f, 1.0f)]
    [InlineData("FlatBiome", 0.2f, 0.80f, 0.90f, 0.95f, 1.0f)]
    [InlineData("MountainousBiome", 0.2f, 0.38f, 0.70f, 0.88f, 1.0f)]
    public void GetById_ReturnsExpectedBiomeValues(string biomeId, float waterRange, float plainRange, float hillRange, float mountainRange, float highMountainRange)
    {
        // Act
        IBiome biome = Biomes.GetById(biomeId);

        // Assert
        Assert.Equal(biomeId, biome.Id);
        Assert.Equal(waterRange, biome.WaterRange);
        Assert.Equal(plainRange, biome.PlainRange);
        Assert.Equal(hillRange, biome.HillRange);
        Assert.Equal(mountainRange, biome.MountainRange);
        Assert.Equal(highMountainRange, biome.HighMountainRange);
    }

    [Theory]
    [InlineData("")]
    [InlineData("UnknownBiome")]
    [InlineData("defaultbiome")]
    public void GetById_ReturnsDefaultBiome_WhenIdIsUnknown(string biomeId)
    {
        // Act
        IBiome biome = Biomes.GetById(biomeId);

        // Assert
        Assert.Same(Biomes.Default, biome);
    }
}
