using System.Diagnostics;
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

        #region Public properties
        public GenerationPhase Phase => GenerationPhase.Structures;
        #endregion

        #region Constructors
        public StructureGenerator(ICityGenerator cityGenerator, IRandomProvider randomProvider, MapGenerationContext context)
        {
            _cityGenerator = cityGenerator;
            _random = randomProvider.GetRandom(context.Seed, "BaseGame.Structures");
        }
        #endregion

        #region Public methods
        public bool TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY)
        {
            if (centerX >= 0 && centerY >= 0 && context.Settings.MaxCityRange > 0)
            {
                return TryPlaceNear(heightMap, waterMap, structureMap, buildingEntity, context, centerX, centerY);
            }

            int maxAttempts = 50;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int startX = _random.Next(0, context.Width - buildingEntity.Width);
                int startY = _random.Next(0, context.Height - buildingEntity.Height);

                if (IsValidPlacement(startX, startY, buildingEntity, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);

                    FillStructureMap(structureMap, startX, startY, buildingEntity);

                    if (buildingEntity is CityEntity city)
                    {
                        _cityGenerator.GenerateCity(city, context);
                    }

                    return true;
                }
            }

            return false;
        }

        public void ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY)
        {
            Debug.WriteLine($"Force placing {buildingEntity.GetType().Name} at ({centerX}, {centerY})");
            if (centerX >= 0 && centerY >= 0 && context.Settings.MaxCityRange > 0)
            {
                ForcePlaceNear(buildingEntity, heightMap, waterMap, structureMap, centerX, centerY, context);
                return;
            }


#pragma warning disable IDE0028 // Simplify collection initialization
            List<(int X, int Y)> validPoints = new(context.Width * context.Height);
#pragma warning restore IDE0028 // Simplify collection initialization

            int structureWidth = buildingEntity.Width;
            int structureHeight = buildingEntity.Height;

            for (int i = 0; i < context.Width - structureWidth; i++)
            {
                for (int j = 0; j < context.Height - structureHeight; j++)
                {
                    if (waterMap[i, j] || structureMap[i, j]) continue;
                    if (heightMap[i, j] >= 4) continue;
                    validPoints.Add((i, j));
                }
            }

            while (validPoints.Count > 0)
            {
                int randomIndex = _random.Next(0, validPoints.Count);
                (int x, int y) = validPoints[randomIndex];

                if (TryTerraformAndPlace(x, y, buildingEntity, heightMap, waterMap, structureMap, context))
                {
                    return;
                }

                // validPoints[^1] == validPoints[validPoints.Count - 1]
                validPoints[randomIndex] = validPoints[^1];
                validPoints.RemoveAt(validPoints.Count - 1);
            }
            throw new Exception("Failed to generate structure!");
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

            Debug.WriteLine($"Force placing {buildingEntity.GetType().Name} at ({startX}, {startY}) with terraforming to height {targetHeight}");

            buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);
            FillStructureMap(structureMap, startX, startY, buildingEntity);

            _cityGenerator.GenerateCity(buildingEntity, context);

            return true;
        }

        private bool GetStartPosition(int centerX, int centerY, BuildingEntity buildingEntity, MapGenerationContext context, out int startX, out int startY)
        {
            int minRange = buildingEntity switch
            {
                CityEntity => context.Settings.MinCityRange,
                _ => context.Settings.MinStructureRange,
            };
            int maxRange = buildingEntity switch
            {
                CityEntity => context.Settings.MaxCityRange,
                _ => context.Settings.MaxStructureRange,
            };

            int dx = centerX + _random.Next(-maxRange, maxRange + 1);
            int dy = centerY + _random.Next(-maxRange, maxRange + 1);
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // Circle
            if (distance < minRange || distance > maxRange)
            {
                startX = -1;
                startY = -1;
                return false;
            }

            // Clamp to map bounds
            startX = Math.Clamp(centerX + dx, 0, context.Width - buildingEntity.Width);
            startY = Math.Clamp(centerY + dy, 0, context.Height - buildingEntity.Height);
            return true;
        }

        private void ForcePlaceNear(BuildingEntity buildingEntity, int[,] heightMap, bool[,] waterMap, bool[,] structureMap, int centerX, int centerY, MapGenerationContext context)
        {
            for (int attempt = 0; attempt < 500; attempt++)
            {
                // Pick a spot within the circle
                if (!GetStartPosition(centerX, centerY, buildingEntity, context, out int startX, out int startY))
                {
                    continue;
                }

                if (TryTerraformAndPlace(startX, startY, buildingEntity, heightMap, waterMap, structureMap, context))
                {
                    return;
                }
            }

            ForcePlace(heightMap, waterMap, structureMap, buildingEntity, context, -1, -1);
        }

        private bool TryPlaceNear(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY)
        {
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Pick a spot within the circle
                if (!GetStartPosition(centerX, centerY, buildingEntity, context, out int startX, out int startY))
                {
                    continue;
                }

                if (IsValidPlacement(startX, startY, buildingEntity, heightMap, waterMap, structureMap))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);
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
