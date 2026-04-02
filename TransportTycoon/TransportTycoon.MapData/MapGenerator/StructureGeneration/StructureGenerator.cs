using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class StructureGeneratorFactory
    {
        public static IStructureGenerator Create(ICityGenerator cityGenerator) => new StructureGenerator(cityGenerator);
    }

    internal class StructureGenerator : IStructureGenerator
    {
        #region Properties
        private ICityGenerator CityGenerator { get; }
        #endregion

        #region Constructors
        public StructureGenerator(ICityGenerator cityGenerator)
        {
            CityGenerator = cityGenerator;
        }
        #endregion

        #region Public methods
        public bool TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int x = -1, int y = -1, int radius = -1)
        {
            if (x >= 0 && y >= 0 && radius > 0)
            {
                return TryPlaceNear(heightMap, waterMap, structureMap, buildingEntity, context, x, y, radius);
            }

            Random rng = new(context.Seed);

            int maxAttempts = 50;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int startX = rng.Next(0, buildingEntity.Width - context.Width);
                int startY = rng.Next(0, buildingEntity.Height - context.Height);

                if (IsValidPlacement(startX, startY, buildingEntity.Width, buildingEntity.Height, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY);

                    FillStructureMap(structureMap, startX, startY, buildingEntity);

                    return true;
                }
            }

            return false;
        }

        public void ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, int x, int y, int radius, MapGenerationContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private methods
        private void FillStructureMap(bool[,] structureMap, int startX, int startY, BuildingEntity buildingEntity)
        {
            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    structureMap[startX + i, startY + j] = true;
                }
            }
        }

        private bool TryPlaceNear(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, int radius)
        {
            Random rng = new(context.Seed + 1);
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Circle
                int startX = centerX + rng.Next(-radius, radius);
                int startY = centerY + rng.Next(-radius, radius);

                // Clamp to map boundaries
                startX = Math.Clamp(startX, 0, context.Width - buildingEntity.Width);
                startY = Math.Clamp(startY, 0, context.Height - buildingEntity.Height);

                if (IsValidPlacement(startX, startY, buildingEntity.Width, buildingEntity.Height, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY);
                    FillStructureMap(structureMap, startX, startY, buildingEntity);
                    return true;
                }
            }

            return false;
        }

        private bool IsValidPlacement(int startX, int startY, int width, int height, int[,] heightMap, bool[,] waterMap, bool[,] occupiedMap)
        {
            // Top-left anchor
            int targetHeight = heightMap[startX, startY];

            // TODO: remove magic number, use TerrainHeight enum
            // Higher than mountain tiles shouldn't have structures
            if (targetHeight >= 4) return false;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int currentX = startX + i;
                    int currentY = startY + j;

                    // Must not be water
                    if (waterMap[currentX, currentY]) return false;

                    // Must not be already occupied
                    if (occupiedMap[currentX, currentY]) return false;

                    // The tiles must have the same height as the top-left anchor tile
                    if (heightMap[currentX, currentY] != targetHeight) return false;
                }
            }

            return true;
        }
        #endregion
    }
}
