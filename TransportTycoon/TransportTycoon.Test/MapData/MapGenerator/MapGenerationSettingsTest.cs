using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class MapGenerationSettingsTest
{
    [TestClass]
    public class DefaultValuesTest
    {
        [TestMethod]
        public void DefaultSettings_HasCorrectDefaultValues()
        {
            // Act
            MapGenerationSettings settings = new();

            // Assert
            Assert.AreEqual(MapGenerationSettingsDefaults.RiverCount, settings.RiverCount);
            Assert.AreEqual(MapGenerationSettingsDefaults.ForestPercentage, settings.ForestPercentage);
            Assert.AreEqual(MapGenerationSettingsDefaults.MinStructureRange, settings.MinStructureRange);
            Assert.AreEqual(MapGenerationSettingsDefaults.MaxStructureRange, settings.MaxStructureRange);
            Assert.AreEqual(MapGenerationSettingsDefaults.MinCities, settings.MinCities);
            Assert.AreEqual(MapGenerationSettingsDefaults.MaxCities, settings.MaxCities);
            Assert.AreEqual(MapGenerationSettingsDefaults.MinStructure, settings.MinStructure);
            Assert.AreEqual(MapGenerationSettingsDefaults.MaxStructure, settings.MaxStructure);
            Assert.AreEqual(MapGenerationSettingsDefaults.TerrainNoiseScale, settings.TerrainNoiseScale);
            Assert.AreEqual(MapGenerationSettingsDefaults.ForestNoiseScale, settings.ForestNoiseScale);
            Assert.AreEqual(MapGenerationSettingsDefaults.WaterNoiseScale, settings.WaterNoiseScale);
            Assert.AreEqual(MapGenerationSettingsDefaults.Biome, settings.Biome);
        }
    }

    [TestClass]
    public class RiverCountTest
    {
        [TestMethod]
        [DataRow(0, DisplayName = "Zero rivers")]
        [DataRow(1, DisplayName = "Single river")]
        [DataRow(5, DisplayName = "Multiple rivers")]
        public void RiverCount_CanBeNonNegative(int riverCount)
        {
            // Act
            MapGenerationSettings settings = new() { RiverCount = riverCount };

            // Assert
            Assert.AreEqual(riverCount, settings.RiverCount);
        }

        [TestMethod]
        public void RiverCount_CannotBeNegative()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { RiverCount = -1 }; }, "ArgumentOutOfRangeException should be thrown for negative RiverCount");
        }
    }

    [TestClass]
    public class ForestPercentageTest
    {
        [TestMethod]
        [DataRow(0.0f, DisplayName = "Zero forest percentage")]
        [DataRow(0.5f, DisplayName = "Half forest percentage")]
        [DataRow(1.0f, DisplayName = "Full forest percentage")]
        public void ForestPercentage_CanBetweenZeroAndOne(float forestPercentage)
        {
            // Act
            MapGenerationSettings settings = new() { ForestPercentage = forestPercentage };

            // Assert
            Assert.AreEqual(forestPercentage, settings.ForestPercentage);
        }

        [TestMethod]
        [DataRow(-0.1f, DisplayName = "Negative forest percentage")]
        [DataRow(1.1f, DisplayName = "Forest percentage greater than one")]
        public void ForestPercentage_MustBeBetweenZeroAndOne(float forestPercentage)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { ForestPercentage = forestPercentage }; }, "ArgumentOutOfRangeException should be thrown for values outside 0 and 1");
        }
    }

    [TestClass]
    public class NoiseScalesTest
    {
        [TestMethod]
        [DataRow(0.0f, DisplayName = "Zero noise scales")]
        [DataRow(0.1f, DisplayName = "Valid noise scales")]
        [DataRow(1.0f, DisplayName = "Full noise scales")]
        public void TerrainNoiseScale_CanBetweenZeroAndOne(float scale)
        {
            MapGenerationSettings settings = new() { TerrainNoiseScale = scale, ForestNoiseScale = scale, WaterNoiseScale = scale };
            Assert.AreEqual(scale, settings.TerrainNoiseScale);
            Assert.AreEqual(scale, settings.ForestNoiseScale);
            Assert.AreEqual(scale, settings.WaterNoiseScale);
        }

        [TestMethod]
        [DataRow(-0.1f, DisplayName = "Negative noise scales")]
        [DataRow(1.1f, DisplayName = "Noise scales greater than one")]
        public void TerrainNoiseScale_MustBetweenZeroAndOne(float scale)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { TerrainNoiseScale = scale }; }, "ArgumentOutOfRangeException should be thrown for values outside 0 and 1");
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { ForestNoiseScale = scale }; }, "ArgumentOutOfRangeException should be thrown for values outside 0 and 1");
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { WaterNoiseScale = scale }; }, "ArgumentOutOfRangeException should be thrown for values outside 0 and 1");
        }
    }

    [TestClass]
    public class BiomeTest
    {
        [TestMethod]
        public void Biome_CanBeChanged()
        {
            // Act
            IBiome customBiome = Substitute.For<IBiome>();
            MapGenerationSettings settings = new() { Biome = customBiome };

            // Assert
            Assert.AreEqual(customBiome, settings.Biome);
        }
    }

    [TestClass]
    public class StructureGenerationTest
    {
        [TestMethod]
        [DataRow(0, DisplayName = "Zero minimum structure range")]
        [DataRow(5, DisplayName = "Positive minimum structure range")]
        public void MinStructureRange_CanBeNonNegative(int range)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructureRange = range };

            // Assert
            Assert.AreEqual(range, settings.MinStructureRange);
        }

        [TestMethod]
        public void MinStructureRange_MustBeNonNegative()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinStructureRange = -1 }; }, "ArgumentOutOfRangeException should be thrown for negative values");
        }

        [TestMethod]
        [DataRow(0, DisplayName = "Zero maximum structure range")]
        [DataRow(5, DisplayName = "Positive maximum structure range")]
        public void MaxStructureRange_CanBeGreaterThanOrEqualMinStructureRange(int range)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructureRange = 0, MaxStructureRange = range };

            // Assert
            Assert.AreEqual(range, settings.MaxStructureRange);
        }

        [TestMethod]
        public void MaxStructureRange_MustBeGreaterThanOrEqualMinStructureRange()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinStructureRange = 1, MaxStructureRange = 0 }; }, "ArgumentOutOfRangeException should be thrown if MaxStructureRange is smaller then MinStructureRange");
        }

        [TestMethod]
        [DataRow(1, DisplayName = "Minimum structures")]
        [DataRow(5, DisplayName = "More than minimum structures")]
        public void MinStructure_CanBePositive(int min)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructure = min };

            // Assert
            Assert.AreEqual(min, settings.MinStructure);
        }

        [TestMethod]
        [DataRow(0, DisplayName = "Zero structure")]
        [DataRow(-1, DisplayName = "Negative structures")]
        public void MinStructure_MustBePositive(int min)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinStructure = min }; }, "ArgumentOutOfRangeException should be thrown for non-positve values");
        }

        [TestMethod]
        [DataRow(1, 1, DisplayName = "Max structures equal to min structures")]
        [DataRow(1, 5, DisplayName = "Max structures greater than min structures")]
        public void MaxStructure_CanBeGreaterThanOrEqualMinStructure(int min, int max)
        {
            // Act
            MapGenerationSettings settings = new() { MinStructure = min, MaxStructure = max };

            // Assert
            Assert.AreEqual(max, settings.MaxStructure);
            Assert.AreEqual(min, settings.MinStructure);
        }

        [TestMethod]
        public void MaxStructure_MustBeGreaterThanOrEqualMinStructure()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinStructure = 1, MaxStructure = 0 }; }, "ArgumentOutOfRangeException should be thrown if MaxStrucure is smaller then MinStructure");
        }

        [TestMethod]
        [DataRow(2, DisplayName = "Minimum cities")]
        [DataRow(5, DisplayName = "More than minimum cities")]
        public void MinCitites_CanBeAtLeastTwo(int min)
        {
            // Act
            MapGenerationSettings settings = new() { MinCities = min };

            // Assert
            Assert.AreEqual(min, settings.MinCities);
        }

        [TestMethod]
        [DataRow(1, DisplayName = "Less than minimum cities")]
        [DataRow(0, DisplayName = "Zero cities")]
        [DataRow(-1, DisplayName = "Negative cities")]
        public void MinCities_MustBeAtLeastTwo(int min)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinCities = min }; }, "ArgumentOutOfRangeException should be thrown for values below 2");
        }

        [TestMethod]
        [DataRow(2, 2, DisplayName = "Max cities equal to min cities")]
        [DataRow(2, 5, DisplayName = "Max cities greater than min cities")]
        public void MaxCities_CanBeGreaterThanOrEqualMinCities(int min, int max)
        {
            // Act
            MapGenerationSettings settings = new() { MinCities = min, MaxCities = max };

            // Assert
            Assert.AreEqual(max, settings.MaxCities);
            Assert.AreEqual(min, settings.MinCities);
        }

        [TestMethod]
        public void MaxCities_MustBeGreaterThanOrEqualMinCities()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { new MapGenerationSettings() { MinCities = 2, MaxCities = 1 }; }, "ArgumentOutOfRangeException should be thrown if MaxCities is smaller then MinCities");
        }
    }
}
