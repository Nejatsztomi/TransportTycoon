using Xunit;
using TransportTycoon.MapData;

namespace TransportTycoon.Test.MapData
{
    public class TerrainTest
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange & Act
            var terrain = new Terrain(5, 10, 2);

            // Assert
            Assert.Equal(5, terrain.X);
            Assert.Equal(10, terrain.Y);
            Assert.Equal(2, terrain.Height);
            Assert.Equal(0, terrain.Trees);
            Assert.Equal(TerrainType.Hill, terrain.TerrainType);
            Assert.True(terrain.Modifiable);
            Assert.False(terrain.IsFull);
        }

        [Theory]
        [InlineData(1, TerrainType.Plain)]
        [InlineData(2, TerrainType.Hill)]
        [InlineData(3, TerrainType.Mountain)]
        [InlineData(4, TerrainType.HighMountain)]
        public void Constructor_SetsCorrectTerrainTypeBasedOnHeight(int height, TerrainType expectedType)
        {
            // Arrange & Act
            var terrain = new Terrain(0, 0, height);

            // Assert
            Assert.Equal(expectedType, terrain.TerrainType);
        }

        [Fact]
        public void IncreaseHeight_IncreasesHeightAndUpdatesTerrainType()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 2); // Kezdetben Hill

            // Act
            terrain.IncreaseHeight();

            // Assert
            Assert.Equal(3, terrain.Height);
            Assert.Equal(TerrainType.Mountain, terrain.TerrainType);
        }

        [Fact]
        public void IncreaseHeight_DoesNotExceedMaxHeight()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 4); // HighMountain

            // Act
            terrain.IncreaseHeight(); // Nem szabadna 5-re nőnie

            // Assert
            Assert.Equal(4, terrain.Height);
            Assert.Equal(TerrainType.HighMountain, terrain.TerrainType);
        }

        [Fact]
        public void DecreaseHeight_DecreasesHeightAndUpdatesTerrainType()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 3); // Kezdetben Mountain

            // Act
            terrain.DecreaseHeight();

            // Assert
            Assert.Equal(2, terrain.Height);
            Assert.Equal(TerrainType.Hill, terrain.TerrainType);
        }

        [Fact]
        public void DecreaseHeight_DoesNotGoBelowMinHeight()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1); // Plain

            // Act
            terrain.DecreaseHeight(); // Nem szabadna 0-ra csökkennie

            // Assert
            Assert.Equal(1, terrain.Height);
            Assert.Equal(TerrainType.Plain, terrain.TerrainType);
        }

        [Fact]
        public void Grow_IncreasesTreeCountAndReturnsTrueIfNotFull()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1);

            // Act
            bool result = terrain.Grow();

            // Assert
            Assert.True(result);
            Assert.Equal(1, terrain.Trees);
            Assert.Equal(1, terrain.GetTrees());
        }

        [Fact]
        public void Grow_DoesNotIncreaseTreeCountAndReturnsFalseIfFull()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1) { Trees = 4 }; // Maximum fával indítunk

            // Act
            bool result = terrain.Grow();

            // Assert
            Assert.False(result); // Mivel már tele van (IsFull == true)
            Assert.Equal(4, terrain.Trees); // Nem nőhet 5-re
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        public void IsFull_ReturnsCorrectStatusBasedOnTreeCount(int trees, bool expectedIsFull)
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1) { Trees = trees };

            // Act & Assert
            Assert.Equal(expectedIsFull, terrain.IsFull);
        }

        [Fact]
        public void SpreadForest_SetsTreesToOne()
        {
            // Arrange
            var terrain = new Terrain(0, 0, 1); // 0 fával indul

            // Act
            terrain.SpreadForest();

            // Assert
            Assert.Equal(1, terrain.Trees);
        }
    }
}
