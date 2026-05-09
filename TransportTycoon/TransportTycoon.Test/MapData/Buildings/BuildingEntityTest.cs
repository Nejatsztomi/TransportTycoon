using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.Test.MapData.Buildings
{
    public class BuildingEntityTest
    {
        private sealed class DefaultBuildingEntity : BuildingEntity
        {
            public DefaultBuildingEntity() : base()
            {
                Scaler = 2;
            }

            public override Load? GetConsumeLoad() => null;
            public override Load GetProvideLoad() => new People();
            public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap) { }
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(4, 4)]
        public void CityEntity_Constructor_SetsWidthAndHeight_Correctly(int width, int height)
        {
            // Act
            var city = new CityEntity(width, height);

            // Assert
            Assert.Equal(width, city.Width);
            Assert.Equal(height, city.Height);
        }

        [Theory]
        [InlineData(0, 3)]
        [InlineData(3, 0)]
        [InlineData(-1, 3)]
        [InlineData(3, -1)]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public void CityEntity_Constructor_ThrowsForInvalidSizes(int width, int height)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new CityEntity(width, height));
        }

        [Fact]
        public void BuildingEntity_DefaultConstructor_SetsDefaultParameters()
        {
            // Arrange & Act
            var building = new DefaultBuildingEntity();

            // Assert
            Assert.Equal(2, building.Width);
            Assert.Equal(2, building.Height);
            Assert.Equal(1000, building.MaxCapacity);
            Assert.Equal(0, building.CurrentCapacity);
            Assert.Empty(building.MapPoints);
        }

        [Fact]
        public void BuildingEntity_Production_IncreasesCurrentCapacityWhenBelowMax()
        {
            // Arrange
            var building = new DefaultBuildingEntity
            {
                Productivity = 1
            };
            building.SetCurrentCapacity(10);

            // Act
            building.Production();

            // Assert
            Assert.True(building.CurrentCapacity > 10);
            Assert.True(building.CurrentCapacity <= building.MaxCapacity);
        }

        [Fact]
        public void BuildingEntity_Production_DoesNothingWhenCapacityIsAtMaximum()
        {
            // Arrange
            var building = new DefaultBuildingEntity
            {
                Productivity = 1
            };
            building.SetCurrentCapacity(building.MaxCapacity);

            // Act
            building.Production();

            // Assert
            Assert.Equal(building.MaxCapacity, building.CurrentCapacity);
        }
    }
}
