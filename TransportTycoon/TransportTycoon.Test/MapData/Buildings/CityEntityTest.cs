using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.Test.MapData.Buildings
{
    public class CityEntityTest
    {
        [Theory]
        [InlineData(3, 5)]
        [InlineData(4, 4)]
        public void Constructor_SetsWidthAndHeight_Correctly(int width, int height)
        {
            // Act
            var city = new CityEntity(width, height);

            // Assert
            Assert.Equal(width, city.Width);
            Assert.Equal(height, city.Height);
        }

        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var city = new CityEntity(2, 3);

            // Assert
            Assert.Equal(10, city.Offset);
            Assert.Equal(1, city.Scaler);
            Assert.Empty(city.MapPoints);
        }

        [Fact]
        public void OverriddenFunctions_ReturnExpectedLoads()
        {
            // Arrange
            var city = new CityEntity(2, 2);

            // Act
            var consumeLoad = city.GetConsumeLoad();
            var provideLoad = city.GetProvideLoad();

            // Assert
            Assert.Null(consumeLoad);
            Assert.IsType<People>(provideLoad);
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(4, 4)]
        public void GenerateBuildingPoints_CreatesHouseTiles_ForRectangleAndSquare(int width, int height)
        {
            // Arrange
            var city = new CityEntity(width, height);
            var heightMap = new int[10, 10];
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    heightMap[x, y] = x * 100 + y;
                }
            }

            int startX = 1;
            int startY = 2;

            // Act
            city.GenerateBuildingPoints(startX, startY, heightMap);

            // Assert
            Assert.Equal(width * height, city.MapPoints.Count);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var field = city.MapPoints[(startX + x, startY + y)];

                    Assert.IsType<House>(field);

                    var house = (House)field;
                    Assert.Equal(startX + x, house.X);
                    Assert.Equal(startY + y, house.Y);
                    Assert.Equal(heightMap[startX + x, startY + y], house.Height);
                    Assert.Same(city, house.BuildingEntity);
                }
            }
        }
    }
}
