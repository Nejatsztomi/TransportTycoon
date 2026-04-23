using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransportTycoon.MapData;

namespace TransportTycoon.Test.MapData
{
    [TestClass]
    public class TerrainTests
    {
        [TestMethod]
        public void Constructor_SetsCorrectInitialTerrainType()
        {
            // Arrange & Act
            var plainTerrain = new Terrain(0, 0, 1);
            var hillTerrain = new Terrain(0, 0, 2);
            var mountainTerrain = new Terrain(0, 0, 3);
            var highMountainTerrain = new Terrain(0, 0, 4);

            // Assert
            Assert.AreEqual(TerrainType.Plain, plainTerrain.TerrainType);
            Assert.AreEqual(TerrainType.Hill, hillTerrain.TerrainType);
            Assert.AreEqual(TerrainType.Mountain, mountainTerrain.TerrainType);
            Assert.AreEqual(TerrainType.HighMountain, highMountainTerrain.TerrainType);
        }

        [TestMethod]
        public void IncreaseHeight_NormalState_IncreasesHeightAndUpdatesType()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1); // Kezdetben Plain (1)

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.AreEqual(2, terrain.Height, "A magasságnak 2-re kellett volna nőnie.");
            Assert.AreEqual(TerrainType.Hill, terrain.TerrainType, "A típusnak Hill-re kellett volna változnia.");
        }

        [TestMethod]
        public void IncreaseHeight_AtMaxHeight_DoesNotExceedMax()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 4); // HighMountain (4) a maximum

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.AreEqual(4, terrain.Height, "A magasság nem nőhet 4 fölé.");
            Assert.AreEqual(TerrainType.HighMountain, terrain.TerrainType);
        }

        [TestMethod]
        public void DecreaseHeight_NormalState_DecreasesHeightAndUpdatesType()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 3); // Kezdetben Mountain (3)

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.AreEqual(2, terrain.Height, "A magasságnak 2-re kellett volna csökkennie.");
            Assert.AreEqual(TerrainType.Hill, terrain.TerrainType, "A típusnak Hill-re kellett volna változnia.");
        }

        [TestMethod]
        public void DecreaseHeight_AtMinHeight_DoesNotGoBelowMin()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1); // Plain (1) a minimum

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.AreEqual(1, terrain.Height, "A magasság nem csökkenhet 1 alá.");
            Assert.AreEqual(TerrainType.Plain, terrain.TerrainType);
        }
        [TestClass]
        public class TerrainForestTests
        {
            [TestMethod]
            public void Constructor_InitializesWithZeroTrees()
            {
                // Arrange & Act
                var terrain = new Terrain(0, 0, 1);

                // Assert
                Assert.AreEqual(0, terrain.GetTrees());
                Assert.IsFalse(terrain.IsFull);
            }

            [TestMethod]
            public void Grow_WhenNotFull_IncreasesTreeCountAndReturnsTrue()
            {
                // Arrange
                var terrain = new Terrain(0, 0, 1);

                // Act
                bool result = terrain.Grow();

                // Assert
                Assert.IsTrue(result, "A Grow() metódusnak true-val kellett volna visszatérnie.");
                Assert.AreEqual(1, terrain.GetTrees());
            }

            [TestMethod]
            public void Grow_WhenFull_DoesNotIncreaseTreeCountAndReturnsFalse()
            {
                // Arrange
                var terrain = new Terrain(0, 0, 1);
                terrain.Trees = 4; // Manuálisan beállítjuk maximumra

                // Act
                bool result = terrain.Grow();

                // Assert
                Assert.IsFalse(result, "A Grow() metódusnak false-al kellett volna visszatérnie, mert a mező tele van.");
                Assert.AreEqual(4, terrain.GetTrees(), "A fák száma nem nőhet 4 fölé.");
                Assert.IsTrue(terrain.IsFull);
            }

            [TestMethod]
            public void SpreadForest_SetsTreeCountToOne()
            {
                // Arrange
                var terrain = new Terrain(0, 0, 1);

                // Act
                terrain.SpreadForest();

                // Assert
                Assert.AreEqual(1, terrain.GetTrees(), "A SpreadForest() metódusnak 1-re kellett volna állítania a fák számát.");
            }

            [TestMethod]
            public void IsFull_ReturnsTrueOnlyWhenTreesAreFour()
            {
                // Arrange
                var terrain = new Terrain(0, 0, 1);

                // Act & Assert
                Assert.IsFalse(terrain.IsFull);

                terrain.Grow(); // 1
                Assert.IsFalse(terrain.IsFull);

                terrain.Grow(); // 2
                Assert.IsFalse(terrain.IsFull);

                terrain.Grow(); // 3
                Assert.IsFalse(terrain.IsFull);

                terrain.Grow(); // 4
                Assert.IsTrue(terrain.IsFull, "4 fánál az IsFull tulajdonságnak true-nak kell lennie.");
            }
        }
    }
}
