using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Test.MapData.MapGenerator;

public class BiomesTest
{
    [TestClass]
    public class DefaultBiomeTest
    {
        [TestMethod]
        public void Biomes_Default_IsNotNull()
        {
            // Act
            IBiome biome = Biomes.Default;

            // Assert
            Assert.IsNotNull(biome);
        }

        [TestMethod]
        public void DefaultBiome_PlainRange_IsValid()
        {
            // Act
            float plainRange = Biomes.Default.PlainRange;

            // Assert
            Assert.IsTrue(0.0f < plainRange && plainRange <= 1.0f, "PlainRange should be between 0 (exclusive) and 1 (inclusive)");
        }

        [TestMethod]
        public void DefaultBiome_HillRange_IsValid()
        {
            // Act
            float hillRange = Biomes.Default.HillRange;

            // Assert
            Assert.IsTrue(0.0 <= hillRange && hillRange <= 1.0f, "HillRange should be between 0 and 1 (inclusive)");
        }

        [TestMethod]
        public void DefaultBiome_MountainRange_IsValid()
        {
            // Act
            float mountainRange = Biomes.Default.MountainRange;

            // Assert
            Assert.IsTrue(0.0f <= mountainRange && mountainRange <= 1.0f, "MountainRange should be between 0 and 1 (inclusive)");
        }

        [TestMethod]
        public void DefaultBiome_HighMountainRange_IsValid()
        {
            // Act
            float highMountainRange = Biomes.Default.HighMountainRange;

            // Assert
            Assert.IsTrue(0.0f <= highMountainRange && highMountainRange <= 1.0f, "HighMountainRange should be between 0 and 1 (inclusive)");
        }

        [TestMethod]
        public void DefaultBiome_RangesAreInAscendingOrder()
        {
            // Arrange
            IBiome biome = Biomes.Default;

            // Assert
            Assert.IsLessThan(biome.HillRange, biome.PlainRange, "PlainRange should be less than HillRange");
            Assert.IsLessThan(biome.MountainRange, biome.HillRange, "HillRange should be less than MountainRange");
            Assert.IsLessThan(biome.HighMountainRange, biome.MountainRange, "MountainRange should be less than or equal to HighMountainRange");
        }

        //[TestMethod]
        //public void Biomes_Default_ReturnsSameInstanceMultipleTimes()
        //{
        //    // Act
        //    IBiome biome1 = Biomes.Default;
        //    IBiome biome2 = Biomes.Default;

        //    // Assert
        //    Assert.AreSame(biome1, biome2, "Biomes.Default should return the same instance");
        //}
    }
}
