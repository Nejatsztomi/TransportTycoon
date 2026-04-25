
using System.Text.Json;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class BiomeJsonConverterTest
    {
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new BiomeJsonConverter() }
        };

        [Theory]
        [InlineData("DefaultBiome")]
        [InlineData("FlatBiome")]
        [InlineData("MountainousBiome")]
        public void Write_And_Read_RoundTrip_Works(string biomeId)
        {
            // Arrange
            var biome = Biomes.GetById(biomeId);

            // Act
            var json = JsonSerializer.Serialize(biome, _options);
            var deserialized = JsonSerializer.Deserialize<IBiome>(json, _options);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(biome.Id, deserialized.Id);
        }

        [Fact]
        public void Read_ThrowsJsonException_OnNull()
        {
            // Arrange
            string json = "null";

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<IBiome>(json, _options));
        }

        [Fact]
        public void Read_ReturnDefaultBiome_OnInvalidId()
        {
            // Arrange
            string json = "\"NonExistentBiome\"";

            // Act & Assert
            Assert.Equal(Biomes.Default, JsonSerializer.Deserialize<IBiome>(json, _options));
        }
    }

    public class WaterBiomeJsonConverterTest
    {
        private readonly JsonSerializerOptions _options = new()
        {
            Converters = { new WaterBiomeJsonConverter() },
        };

        [Theory]
        [InlineData("Wet")]
        [InlineData("Normal")]
        [InlineData("Dry")]
        public void Write_And_Read_RoundTrip_Works(string biomeId)
        {
            // Arrange
            var biome = WaterBiomes.GetById(biomeId);

            // Act
            var json = JsonSerializer.Serialize(biome, _options);
            var deserialized = JsonSerializer.Deserialize<IWaterBiome>(json, _options);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(biome.Id, deserialized!.Id);
        }

        [Fact]
        public void Read_ThrowsJsonException_OnNull()
        {
            // Arrange
            var json = "null";

            // Act & Assert
            var e = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<IWaterBiome>(json, _options));
        }

        [Fact]
        public void Read_ReturnNormalWaterBiome_OnInvalidId()
        {
            // Arrange
            string json = "\"NonExistentBiome\"";

            // Act & Assert
            Assert.Equal(WaterBiomes.Normal, JsonSerializer.Deserialize<IWaterBiome>(json, _options));
        }
    }
}
