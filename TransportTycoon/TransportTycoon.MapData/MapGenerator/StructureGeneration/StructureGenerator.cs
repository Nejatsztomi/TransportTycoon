using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class StructureGeneratorFactory
    {
        public static IStructureGenerator Create(ICityGenerator cityGenerator, IRandomProvider randomProvider, MapGenerationContext context) => new StructureGenerator(cityGenerator, randomProvider, context);
    }

    internal class StructureGenerator : IStructureGenerator
    {
        #region Private fields
        private readonly ICityGenerator _cityGenerator;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public StructureGenerator(ICityGenerator cityGenerator, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _cityGenerator = cityGenerator;
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Structures);
        }
        #endregion

        #region Public methods
        public bool TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, int radius)
        {
            if (centerX >= 0 && centerY >= 0 && radius > 0)
            {
                return TryPlaceNear(heightMap, waterMap, structureMap, buildingEntity, context, centerX, centerY, radius);
            }

            int maxAttempts = 50;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int startX = _random.Next(0, context.Width - buildingEntity.Width);
                int startY = _random.Next(0, context.Height - buildingEntity.Height);

                if (IsValidPlacement(startX, startY, buildingEntity, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY);

                    FillStructureMap(structureMap, startX, startY, buildingEntity);

                    if (buildingEntity is CityEntity city)
                    {
                        _cityGenerator.GenerateCity(3, 10, city, context);
                    }

                    return true;
                }
            }

            return false;
        }

        public void ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, int radius)
        {
            if (centerX >= 0 && centerY >= 0 && radius > 0)
            {
                ForcePlaceNear(buildingEntity, heightMap, waterMap, structureMap, centerX, centerY, radius, context);
                return;
            }

            while (true)
            {
                int startX = _random.Next(0, context.Width - buildingEntity.Width);
                int startY = _random.Next(0, context.Height - buildingEntity.Height);

                if (TryTerraformAndPlace(startX, startY, buildingEntity, heightMap, waterMap, structureMap, context))
                {
                    return;
                }
            }
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

        private bool TryTerraformAndPlace(int startX, int startY, BuildingEntity buildingEntity, int[,] heightMap, bool[,] waterMap, bool[,] structureMap, MapGenerationContext context)
        {
            // Valid tile check (no water, no structures)
            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    if (waterMap[startX + i, startY + j] || structureMap[startX + i, startY + j])
                        return false;
                }
            }

            // TERRAFORM: Flatten to map to the avarage height
            int sumHeight = 0;
            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    sumHeight = heightMap[startX + i, startY + j];
                }
            }

            // TODO: check for invalid terrain and fix it
            int targetHeight = (int)Math.Round((double)sumHeight / (buildingEntity.Width * buildingEntity.Height));
            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    heightMap[startX + i, startY + j] = targetHeight;
                }
            }
            buildingEntity.GenerateBuildingPoints(startX, startY);
            FillStructureMap(structureMap, startX, startY, buildingEntity);

            _cityGenerator.GenerateCity(3, 10, buildingEntity, context);

            return true;
        }

        private void ForcePlaceNear(BuildingEntity buildingEntity, int[,] heightMap, bool[,] waterMap, bool[,] structureMap, int centerX, int centerY, int radius, MapGenerationContext context)
        {
            for (int attempt = 0; attempt < 500; attempt++)
            {
                // Pick a spot within the radius
                int startX = centerX + _random.Next(-radius, radius);
                int startY = centerY + _random.Next(-radius, radius);

                // Clamp to map bounds
                startX = Math.Clamp(startX, 0, context.Width - buildingEntity.Width);
                startY = Math.Clamp(startY, 0, context.Height - buildingEntity.Height);

                if (TryTerraformAndPlace(startX, startY, buildingEntity, heightMap, waterMap, structureMap, context))
                {
                    return;
                }
            }

            ForcePlace(heightMap, waterMap, structureMap, buildingEntity, context, -1, -1, -1);
        }

        private bool TryPlaceNear(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, int radius)
        {
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Circle
                int startX = centerX + _random.Next(-radius, radius);
                int startY = centerY + _random.Next(-radius, radius);

                // Clamp to map boundaries
                startX = Math.Clamp(startX, 0, context.Width - buildingEntity.Width);
                startY = Math.Clamp(startY, 0, context.Height - buildingEntity.Height);

                if (IsValidPlacement(startX, startY, buildingEntity, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY);
                    FillStructureMap(structureMap, startX, startY, buildingEntity);
                    return true;
                }
            }

            return false;
        }

        private bool IsValidPlacement(int startX, int startY, BuildingEntity buildingEntity, int[,] heightMap, bool[,] waterMap, bool[,] structureMap)
        {
            // Top-left anchor
            int targetHeight = heightMap[startX, startY];

            // TODO: remove magic number, use TerrainHeight enum
            // Higher than mountain tiles shouldn't have structures
            if (targetHeight >= 4) return false;

            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    int currentX = startX + i;
                    int currentY = startY + j;

                    // Must not be water
                    if (waterMap[currentX, currentY]) return false;

                    // Must not be already occupied
                    if (structureMap[currentX, currentY]) return false;

                    // The tiles must have the same height as the top-left anchor tile
                    if (heightMap[currentX, currentY] != targetHeight) return false;
                }
            }

            return true;
        }
        #endregion
    }
}
