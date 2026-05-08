using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.Test.MapData.Buildings
{
    public class IndustryEntityTest
    {
        private sealed class TestIndustryEntity : IndustryEntity
        {
            public override Load? GetConsumeLoad() => new Wood();
            public override Load GetProvideLoad() => new Paper();
            public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap) { }
        }

        [Fact]
        public void IndustryEntity_Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var industry = new TestIndustryEntity();

            // Assert
            Assert.Equal(2, industry.Width);
            Assert.Equal(2, industry.Height);
            Assert.Equal(2, industry.Scaler);
            Assert.Equal(0, industry.CurrentCapacity);
            Assert.Equal(1000, industry.MaxCapacity);
            Assert.Equal(1000, industry.MaxConsumeCapacity);
            Assert.Empty(industry.MapPoints);
        }

        [Fact]
        public void IndustryEntity_SetConsumeCapacity_UpdatesOnlyForNonNegativeValues()
        {
            // Arrange
            var industry = new TestIndustryEntity();

            // Act
            industry.SetConsumeCapacity(250);

            // Assert
            Assert.Equal(250, industry.ConsumeCapacity);

            // Act
            industry.SetConsumeCapacity(-1);

            // Assert
            Assert.Equal(250, industry.ConsumeCapacity);
        }

        [Fact]
        public void IndustryEntity_Production_IncreasesCurrentCapacityAndDecreasesConsumeCapacity()
        {
            // Arrange
            var industry = new TestIndustryEntity
            {
                Productivity = 1
            };
            industry.SetCurrentCapacity(10);
            industry.SetConsumeCapacity(100);

            // Act
            industry.Production();

            // Assert
            Assert.True(industry.CurrentCapacity > 10);
            Assert.True(industry.ConsumeCapacity < 100);
        }

        [Fact]
        public void IndustryEntity_Production_UsesConsumeCapacityWhenProductionIsHigher()
        {
            // Arrange
            var industry = new TestIndustryEntity
            {
                Productivity = 1000
            };
            industry.SetCurrentCapacity(10);
            industry.SetConsumeCapacity(5);

            // Act
            industry.Production();

            // Assert
            Assert.Equal(15, industry.CurrentCapacity);
            Assert.Equal(0, industry.ConsumeCapacity);
        }

        [Fact]
        public void IndustryEntity_Production_ClampsToMaxCapacity_WhenProductionWouldOverflow()
        {
            // Arrange
            var industry = new TestIndustryEntity
            {
                Productivity = 1000
            };
            industry.SetCurrentCapacity(999);
            industry.SetConsumeCapacity(50);

            // Act
            industry.Production();

            // Assert
            Assert.Equal(industry.MaxCapacity, industry.CurrentCapacity);
            Assert.Equal(49, industry.ConsumeCapacity);
        }

        [Fact]
        public void IndustryEntity_Production_DoesNothingWhenConsumeCapacityIsZero()
        {
            // Arrange
            var industry = new TestIndustryEntity
            {
                Productivity = 1
            };
            industry.SetCurrentCapacity(10);
            industry.SetConsumeCapacity(0);

            // Act
            industry.Production();

            // Assert
            Assert.Equal(10, industry.CurrentCapacity);
            Assert.Equal(0, industry.ConsumeCapacity);
        }

        [Fact]
        public void IndustryEntity_Production_DoesNothingWhenCurrentCapacityIsAtMaximum()
        {
            // Arrange
            var industry = new TestIndustryEntity
            {
                Productivity = 1
            };
            industry.SetCurrentCapacity(industry.MaxCapacity);
            industry.SetConsumeCapacity(100);

            // Act
            industry.Production();

            // Assert
            Assert.Equal(industry.MaxCapacity, industry.CurrentCapacity);
            Assert.Equal(100, industry.ConsumeCapacity);
        }

        [Fact]
        public void MillEntity_ConstructorAndOverrides_AreSetCorrectly()
        {
            // Arrange & Act
            var mill = new MillEntity();
            var heightMap = CreateHeightMap(10);
            mill.GenerateBuildingPoints(1, 2, heightMap);

            // Assert
            Assert.Equal(2, mill.Width);
            Assert.Equal(2, mill.Height);
            Assert.Equal(2, mill.Scaler);
            Assert.Equal(70, mill.Offset);
            Assert.IsType<Wheat>(mill.GetConsumeLoad());
            Assert.IsType<Flour>(mill.GetProvideLoad());
            Assert.Equal(4, mill.MapPoints.Count);
            AssertGeneratedPoints(mill, heightMap, 1, 2);
        }

        [Fact]
        public void PlantEntity_ConstructorAndOverrides_AreSetCorrectly()
        {
            // Arrange & Act
            var plant = new PlantEntity();
            var heightMap = CreateHeightMap(10);
            plant.GenerateBuildingPoints(1, 2, heightMap);

            // Assert
            Assert.Equal(2, plant.Width);
            Assert.Equal(2, plant.Height);
            Assert.Equal(2, plant.Scaler);
            Assert.Equal(60, plant.Offset);
            Assert.IsType<Oil>(plant.GetConsumeLoad());
            Assert.IsType<Rubber>(plant.GetProvideLoad());
            Assert.Equal(4, plant.MapPoints.Count);
            AssertGeneratedPoints(plant, heightMap, 1, 2);
        }

        [Fact]
        public void FactoryEntity_ConstructorAndOverrides_AreSetCorrectly()
        {
            // Arrange & Act
            var factory = new FactoryEntity();
            var heightMap = CreateHeightMap(10);
            factory.GenerateBuildingPoints(1, 2, heightMap);

            // Assert
            Assert.Equal(2, factory.Width);
            Assert.Equal(2, factory.Height);
            Assert.Equal(2, factory.Scaler);
            Assert.Equal(50, factory.Offset);
            Assert.IsType<Wood>(factory.GetConsumeLoad());
            Assert.IsType<Paper>(factory.GetProvideLoad());
            Assert.Equal(4, factory.MapPoints.Count);
            AssertGeneratedPoints(factory, heightMap, 1, 2);
        }

        private static int[,] CreateHeightMap(int size)
        {
            var heightMap = new int[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    heightMap[x, y] = x * 100 + y;
                }
            }

            return heightMap;
        }

        private static void AssertGeneratedPoints(BuildingEntity building, int[,] heightMap, int startX, int startY)
        {
            for (int x = 0; x < building.Width; x++)
            {
                for (int y = 0; y < building.Height; y++)
                {
                    var field = building.MapPoints[(startX + x, startY + y)];
                    var expectedHeight = heightMap[startX + x, startY + y];

                    switch (field)
                    {
                        case Mill mill:
                            Assert.Equal(startX + x, mill.X);
                            Assert.Equal(startY + y, mill.Y);
                            Assert.Equal(expectedHeight, mill.Height);
                            Assert.Same(building, mill.BuildingEntity);
                            break;
                        case Plant plant:
                            Assert.Equal(startX + x, plant.X);
                            Assert.Equal(startY + y, plant.Y);
                            Assert.Equal(expectedHeight, plant.Height);
                            Assert.Same(building, plant.BuildingEntity);
                            break;
                        case Factory factory:
                            Assert.Equal(startX + x, factory.X);
                            Assert.Equal(startY + y, factory.Y);
                            Assert.Equal(expectedHeight, factory.Height);
                            Assert.Same(building, factory.BuildingEntity);
                            break;
                    }
                }
            }
        }
    }
}
