using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator.TerrainGeneration;

public class WaterBiomesTest
{
    [Theory]
    [InlineData("Wet", 0.75f)]
    [InlineData("Normal", 0.5f)]
    [InlineData("Dry", 0.25f)]
    public void GetById_ReturnsExpectedWaterBiomeValues(string biomeId, float waterLevel)
    {
        // Act
        IWaterBiome biome = WaterBiomes.GetById(biomeId);

        // Assert
        Assert.Equal(biomeId, biome.Id);
        Assert.Equal(waterLevel, biome.WaterLevel);
    }

    [Theory]
    [InlineData("")]
    [InlineData("UnknownBiome")]
    [InlineData("normal")]
    public void GetById_ReturnsNormalBiome_WhenIdIsUnknown(string biomeId)
    {
        // Act
        IWaterBiome biome = WaterBiomes.GetById(biomeId);

        // Assert
        Assert.Same(WaterBiomes.Normal, biome);
    }
}
