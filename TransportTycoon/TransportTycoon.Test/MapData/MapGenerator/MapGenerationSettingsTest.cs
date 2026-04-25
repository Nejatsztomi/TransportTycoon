using NSubstitute;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGenerationSettingsTest
{
    public class DefaultValuesTest
    {
        [Fact]
        public void DefaultSettings_HasCorrectDefaultValues()
        {
            // Act
            MapGenerationSettings settings = new();

            // Assert
            Assert.Equal(MapGenerationSettingsDefaults.RiverCount, settings.RiverCount);
            Assert.Equal(MapGenerationSettingsDefaults.ForestPercentage, settings.ForestPercentage);
            Assert.Equal(MapGenerationSettingsDefaults.MinStructureRange, settings.MinStructureRange);
            Assert.Equal(MapGenerationSettingsDefaults.MaxStructureRange, settings.MaxStructureRange);
            Assert.Equal(MapGenerationSettingsDefaults.MinCities, settings.MinCities);
            Assert.Equal(MapGenerationSettingsDefaults.MaxCities, settings.MaxCities);
            Assert.Equal(MapGenerationSettingsDefaults.MinStructure, settings.MinStructure);
            Assert.Equal(MapGenerationSettingsDefaults.MaxStructure, settings.MaxStructure);
            Assert.Equal(MapGenerationSettingsDefaults.TerrainNoiseScale, settings.TerrainNoiseScale);
            Assert.Equal(MapGenerationSettingsDefaults.ForestNoiseScale, settings.ForestNoiseScale);
            Assert.Equal(MapGenerationSettingsDefaults.WaterNoiseScale, settings.WaterNoiseScale);
            Assert.Equal(MapGenerationSettingsDefaults.Biome, settings.Biome);
        }
    }

    public class RiverCountTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void RiverCount_CanBeNonNegative(int riverCount)
        {
            // Act
            MapGenerationSettings settings = new() { RiverCount = riverCount };

            // Assert
            Assert.Equal(riverCount, settings.RiverCount);
        }

        [Fact]
        public void RiverCount_CannotBeNegative()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    RiverCount = -1
                };
            });
        }
    }

    public class ForestPercentageTest
    {
        [Theory]
        [InlineData(0.0f)]
        [InlineData(0.5f)]
        [InlineData(1.0f)]
        public void ForestPercentage_CanBetweenZeroAndOne(float forestPercentage)
        {
            // Act
            MapGenerationSettings settings = new()
            {
                ForestPercentage = forestPercentage
            };

            // Assert
            Assert.Equal(forestPercentage, settings.ForestPercentage);
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void ForestPercentage_MustBeBetweenZeroAndOne(float forestPercentage)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    ForestPercentage = forestPercentage
                };
            });
        }
    }

    public class NoiseScalesTest
    {
        [Theory]
        [InlineData(0.0f)]
        [InlineData(0.1f)]
        [InlineData(1.0f)]
        public void TerrainNoiseScale_CanBetweenZeroAndOne(float scale)
        {
            MapGenerationSettings settings = new() { TerrainNoiseScale = scale, ForestNoiseScale = scale, WaterNoiseScale = scale };
            Assert.Equal(scale, settings.TerrainNoiseScale);
            Assert.Equal(scale, settings.ForestNoiseScale);
            Assert.Equal(scale, settings.WaterNoiseScale);
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void TerrainNoiseScale_MustBetweenZeroAndOne(float scale)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    TerrainNoiseScale = scale
                };
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    ForestNoiseScale = scale
                };
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    WaterNoiseScale = scale
                };
            });
        }
    }

    public class BiomeTest
    {
        [Fact]
        public void Biome_CanBeChanged()
        {
            // Act
            IBiome customBiome = Substitute.For<IBiome>();
            MapGenerationSettings settings = new() { Biome = customBiome };

            // Assert
            Assert.Equal(customBiome, settings.Biome);
        }
    }

    public class StructureGenerationTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void MinStructureRange_CanBeNonNegative(int range)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructureRange = range };

            // Assert
            Assert.Equal(range, settings.MinStructureRange);
        }

        [Fact]
        public void MinStructureRange_MustBeNonNegative()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinStructureRange = -1
                };
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void MaxStructureRange_CanBeGreaterThanOrEqualMinStructureRange(int range)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructureRange = 0, MaxStructureRange = range };

            // Assert
            Assert.Equal(range, settings.MaxStructureRange);
        }

        [Fact]
        public void MaxStructureRange_MustBeGreaterThanOrEqualMinStructureRange()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinStructureRange = 1,
                    MaxStructureRange = 0
                };
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void MinStructure_CanBePositive(int min)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructure = min };

            // Assert
            Assert.Equal(min, settings.MinStructure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void MinStructure_MustBePositive(int min)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinStructure = min
                };
            });
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 5)]
        public void MaxStructure_CanBeGreaterThanOrEqualMinStructure(int min, int max)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructure = min, MaxStructure = max };

            // Assert
            Assert.Equal(max, settings.MaxStructure);
            Assert.Equal(min, settings.MinStructure);
        }

        [Fact]
        public void MaxStructure_MustBeGreaterThanOrEqualMinStructure()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinStructure = 1,
                    MaxStructure = 0
                };
            });
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void MinCitites_CanBeAtLeastTwo(int min)
        {
            // Act
            MapGenerationSettings settings = new() { MinCities = min };

            // Assert
            Assert.Equal(min, settings.MinCities);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void MinCities_MustBeAtLeastTwo(int min)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinCities = min
                };
            });
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 5)]
        public void MaxCities_CanBeGreaterThanOrEqualMinCities(int min, int max)
        {
            // Act
            MapGenerationSettings settings = new() { MinCities = min, MaxCities = max };

            // Assert
            Assert.Equal(max, settings.MaxCities);
            Assert.Equal(min, settings.MinCities);
        }

        [Fact]
        public void MaxCities_MustBeGreaterThanOrEqualMinCities()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinCities = 2,
                    MaxCities = 1
                };
            });
        }
    }
}
