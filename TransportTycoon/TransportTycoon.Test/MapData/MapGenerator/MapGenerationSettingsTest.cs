using NSubstitute;
using TransportTycoon.MapData.MapGenerator;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGenerationSettingsTest
{
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

    public class RiverWidthTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void MinRiverWidth_CanBePositive(int width)
        {
            // Act
            MapGenerationSettings settings = new() { MinRiverWidth = width };

            // Assert
            Assert.Equal(width, settings.MinRiverWidth);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void MinRiverWidth_MustBePositive(int width)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinRiverWidth = width
                };
            });
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 5)]
        public void MaxRiverWidth_CanBeGreaterThanOrEqualMinRiverWidth(int minWidth, int maxWidth)
        {
            // Act
            MapGenerationSettings settings = new() { MinRiverWidth = minWidth, MaxRiverWidth = maxWidth };

            // Assert
            Assert.Equal(minWidth, settings.MinRiverWidth);
            Assert.Equal(maxWidth, settings.MaxRiverWidth);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        public void MaxRiverWidth_MustBeGreaterThanOrEqualMinRiverWidth(int minWidth, int maxWidth)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinRiverWidth = minWidth,
                    MaxRiverWidth = maxWidth
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

        public class StructureSizeTest
        {
            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(5)]
            public void StructureWidth_CanBeAtLeastTwo(int width)
            {
                // Act
                MapGenerationSettings settings = new() { StructureWidth = width };

                // Assert
                Assert.Equal(width, settings.StructureWidth);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            public void StructureWidth_MustBeAtLeastTwo(int width)
            {
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new MapGenerationSettings()
                    {
                        StructureWidth = width
                    };
                });
            }

            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(5)]
            public void StructureHeight_CanBeAtLeastTwo(int height)
            {
                // Act
                MapGenerationSettings settings = new() { StructureHeight = height };

                // Assert
                Assert.Equal(height, settings.StructureHeight);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(0)]
            [InlineData(-1)]
            public void StructureHeight_MustBeAtLeastTwo(int height)
            {
                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new MapGenerationSettings()
                    {
                        StructureHeight = height
                    };
                });
            }
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

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void MinCityRange_CanBeNonNegative(int range)
        {
            // Act
            MapGenerationSettings settings = new() { MinCityRange = range };

            // Assert
            Assert.Equal(range, settings.MinCityRange);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        public void MinCityRange_MustBeNonNegative(int range)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinCityRange = range
                };
            });
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 5)]
        public void MaxCityRange_CanBeGreaterThanOrEqualMinCityRange(int minRange, int maxRange)
        {
            // Act
            MapGenerationSettings settings = new() { MinCityRange = minRange, MaxCityRange = maxRange };

            // Assert
            Assert.Equal(minRange, settings.MinCityRange);
            Assert.Equal(maxRange, settings.MaxCityRange);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(2, 1)]
        public void MaxCityRange_MustBeGreaterThanOrEqualMinCityRange(int minRange, int maxRange)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    MinCityRange = minRange,
                    MaxCityRange = maxRange
                };
            });
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void CityWidth_CanBeAtLeastThree(int width)
        {
            // Act
            MapGenerationSettings settings = new() { CityWidth = width };

            // Assert
            Assert.Equal(width, settings.CityWidth);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        [InlineData(-1)]
        public void CityWidth_MustBeAtLeastThree(int width)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    CityWidth = width
                };
            });
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void CityHeight_CanBeGreaterThanThree(int height)
        {
            // Act
            MapGenerationSettings settings = new() { CityHeight = height };

            // Assert
            Assert.Equal(height, settings.CityHeight);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(0)]
        [InlineData(-1)]
        public void CityHeight_MustBeGreaterThanThree(int height)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    CityHeight = height
                };
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void RoadLength_CanBePositive(int length)
        {
            // Act
            MapGenerationSettings settings = new() { RoadLength = length };

            // Assert
            Assert.Equal(length, settings.RoadLength);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RoadLength_MustBePositive(int length)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    RoadLength = length
                };
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void BranchCount_CanBePositive(int count)
        {
            // Act
            MapGenerationSettings settings = new() { BranchCount = count };

            // Assert
            Assert.Equal(count, settings.BranchCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BranchCount_MustBePositive(int count)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new MapGenerationSettings()
                {
                    BranchCount = count
                };
            });
        }
    }
}
