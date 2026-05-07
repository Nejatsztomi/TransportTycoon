using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.Test.MapData.Buildings
{
    public class SiteEntityTest
    {
        private sealed class TestSiteEntity : SiteEntity
        {
            public override Load? GetConsumeLoad() => null;
            public override Load GetProvideLoad() => new Wood();
            public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap) { }
        }

        [Fact]
        public void SiteEntity_Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var site = new TestSiteEntity();

            // Assert
            Assert.Equal(2, site.Width);
            Assert.Equal(2, site.Height);
            Assert.Equal(1, site.Scaler);
            Assert.Empty(site.MapPoints);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(30)]
        [InlineData(40)]
        public void DerivedSiteEntities_SetsOffsetAndDefaultDimensions_Correctly(int expectedOffset)
        {
            // Arrange
            SiteEntity site = expectedOffset switch
            {
                20 => new LumberCampEntity(),
                30 => new MineEntity(),
                _ => new FarmEntity()
            };

            // Assert
            Assert.Equal(2, site.Width);
            Assert.Equal(2, site.Height);
            Assert.Equal(1, site.Scaler);
            Assert.Equal(expectedOffset, site.Offset);
            Assert.Empty(site.MapPoints);
        }

        [Theory]
        [InlineData(typeof(LumberCampEntity), typeof(Wood), 20)]
        [InlineData(typeof(MineEntity), typeof(Oil), 30)]
        [InlineData(typeof(FarmEntity), typeof(Wheat), 40)]
        public void DerivedSiteEntities_OverrideLoadMethods_Correctly(Type entityType, Type expectedProvideType, int expectedOffset)
        {
            // Arrange
            var site = (BuildingEntity)Activator.CreateInstance(entityType)!;

            // Act
            var consumeLoad = site.GetConsumeLoad();
            var provideLoad = site.GetProvideLoad();

            // Assert
            Assert.Null(consumeLoad);
            Assert.IsType(expectedProvideType, provideLoad);
            Assert.Equal(expectedOffset, site.Offset);
        }

        [Theory]
        [InlineData(typeof(LumberCampEntity))]
        [InlineData(typeof(MineEntity))]
        [InlineData(typeof(FarmEntity))]
        public void DerivedSiteEntities_GenerateBuildingPoints_CreatesCorrectSiteTiles(Type entityType)
        {
            // Arrange
            var site = (BuildingEntity)Activator.CreateInstance(entityType)!;
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
            site.GenerateBuildingPoints(startX, startY, heightMap);

            // Assert
            Assert.Equal(4, site.MapPoints.Count);

            for (int x = 0; x < site.Width; x++)
            {
                for (int y = 0; y < site.Height; y++)
                {
                    var field = site.MapPoints[(startX + x, startY + y)];
                    var expectedHeight = heightMap[startX + x, startY + y];

                    switch (site)
                    {
                        case LumberCampEntity:
                            Assert.IsType<LumberCamp>(field);
                            var lumberCamp = (LumberCamp)field;
                            Assert.Equal(startX + x, lumberCamp.X);
                            Assert.Equal(startY + y, lumberCamp.Y);
                            Assert.Equal(expectedHeight, lumberCamp.Height);
                            Assert.Same(site, lumberCamp.BuildingEntity);
                            break;
                        case MineEntity:
                            Assert.IsType<Mine>(field);
                            var mine = (Mine)field;
                            Assert.Equal(startX + x, mine.X);
                            Assert.Equal(startY + y, mine.Y);
                            Assert.Equal(expectedHeight, mine.Height);
                            Assert.Same(site, mine.BuildingEntity);
                            break;
                        case FarmEntity:
                            Assert.IsType<Farm>(field);
                            var farm = (Farm)field;
                            Assert.Equal(startX + x, farm.X);
                            Assert.Equal(startY + y, farm.Y);
                            Assert.Equal(expectedHeight, farm.Height);
                            Assert.Same(site, farm.BuildingEntity);
                            break;
                    }
                }
            }
        }
    }
}
