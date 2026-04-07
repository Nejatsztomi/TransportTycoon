using TransportTycoon.MapData;

namespace TransportTycoon.Test;

[TestClass]
public class TerrainTest
{
    [TestClass]
    public class TerrainTests
    {
        [TestMethod]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var terrain = new Terrain(5, 10, 2);

            // Assert
            Assert.AreEqual(5, terrain.X);
            Assert.AreEqual(10, terrain.Y);
            Assert.AreEqual(2, terrain.Height);
            Assert.AreEqual(0, terrain.Trees);
            Assert.IsTrue(terrain.Modifiable);
        }

        [TestMethod]
        [DataRow(1, FieldType.Plain)]
        [DataRow(2, FieldType.Hill)]
        [DataRow(3, FieldType.Mountain)]
        [DataRow(4, FieldType.HighMountain)]
        [DataRow(5, FieldType.HighMountain)] // 4 felett is HighMountain a logika alapján
        public void Constructor_SetsCorrectFieldType_BasedOnHeight(int height, FieldType expectedType)
        {
            // Act
            var terrain = new Terrain(0, 0, height);

            // Assert
            Assert.AreEqual(expectedType, terrain.FieldType);
        }

        public void IncreaseHeight_From1To2_HeightIncreasesAndTypeIsHill()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.AreEqual(2, terrain.Height);
            Assert.AreEqual(FieldType.Hill, terrain.FieldType);
        }

        [TestMethod]
        public void IncreaseHeight_From3To4_HeightIncreasesAndTypeIsHighMountain()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 3);

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.AreEqual(4, terrain.Height);
            Assert.AreEqual(FieldType.HighMountain, terrain.FieldType);
        }

        [TestMethod]
        public void IncreaseHeight_WhenHeightIsAlready4_HeightDoesNotIncrease()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 4);

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.AreEqual(4, terrain.Height, "Height should remain 4 since it has reached the maximum limit.");
            Assert.AreEqual(FieldType.HighMountain, terrain.FieldType);
        }


        [TestMethod]
        public void DecreaseHeight_From3To2_HeightDecreasesAndTypeIsHill()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 3);

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.AreEqual(2, terrain.Height);
            Assert.AreEqual(FieldType.Hill, terrain.FieldType);
        }

        [TestMethod]
        public void DecreaseHeight_From2To1_HeightDecreasesAndTypeIsPlain()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 2);

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.AreEqual(1, terrain.Height);
            Assert.AreEqual(FieldType.Plain, terrain.FieldType);
        }

        [TestMethod]
        public void DecreaseHeight_WhenHeightIsAlready1_HeightDoesNotDecrease()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.AreEqual(1, terrain.Height, "Height should remain 1 since it has reached the minimum limit.");
            Assert.AreEqual(FieldType.Plain, terrain.FieldType);
        }

        [TestMethod]
        public void Grow_IncrementsTrees_WhenNotFull()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);

            // Act
            bool result = terrain.Grow();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, terrain.Trees);
            Assert.IsFalse(terrain.IsFull);
        }

        [TestMethod]
        public void Grow_ReturnsFalse_WhenFull()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);
            terrain.Trees = 4; // Manuálisan telepakoljuk

            // Act
            bool result = terrain.Grow();

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(4, terrain.Trees);
            Assert.IsTrue(terrain.IsFull);
        }

        [TestMethod]
        public void SpreadForest_SetsTreesToOne()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);

            // Act
            terrain.SpreadForest();

            // Assert
            Assert.AreEqual(1, terrain.Trees);
        }
    }
}

