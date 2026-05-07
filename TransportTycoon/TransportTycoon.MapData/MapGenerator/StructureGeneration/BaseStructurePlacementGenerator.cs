using System.Diagnostics;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    internal abstract class BaseStructurePlacementGenerator : IStructureGenerator
    {
        #region Private fields
        private const int MAX_TRY_PLACE_ATTEMPTS = 50;
        private const int MAX_TRY_PLACE_NEAR_ATTEMPTS = 100;
        private const int MAX_FORCE_PLACE_NEAR_ATTEMPTS = 500;
        #endregion

        #region Protected fields
        protected readonly IRandomProvider _random;
        protected readonly MapGenerationContext _context;
        #endregion

        #region Public properties
        public abstract GenerationPhase Phase { get; }
        #endregion

        #region Protected constructors
        protected BaseStructurePlacementGenerator(IRandomProvider randomProvider, MapGenerationContext context)
        {
            _random = randomProvider;
            _context = context;
        }
        #endregion

        #region Public methods
        public abstract List<BuildingEntity> GenerateStructures(MapGenerationContext context);
        #endregion

        #region Protected methods
        protected bool TryPlace(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
        {
            var heightMap = context.HeightMap;
            var structureMap = context.StructureMap;

            if (centerX >= 0 && centerY >= 0 && context.Settings.MaxCityRange > 0)
            {
                return TryPlaceNear(buildingEntity, context, centerX, centerY, random, validPoints);
            }

            for (int attempt = 0; attempt < MAX_TRY_PLACE_ATTEMPTS; attempt++)
            {
                int startX = random.Next(0, context.Width - buildingEntity.Width);
                int startY = random.Next(0, context.Height - buildingEntity.Height);

                if (IsValidPlacement(startX, startY, buildingEntity, context))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);

                    FillStructureMap(structureMap, startX, startY, buildingEntity, validPoints);

                    return true;
                }
            }
            return false;
        }

        protected void ForcePlace(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
        {
            Debug.WriteLine($"Force placing {buildingEntity.GetType().Name} at ({centerX}, {centerY})");
            if (centerX >= 0 && centerY >= 0 && context.Settings.MaxCityRange > 0)
            {
                ForcePlaceNear(buildingEntity, context, centerX, centerY, random, validPoints);
                return;
            }

            while (validPoints.Count > 0)
            {
                int randomIndex = random.Next(0, validPoints.Count);
                (int x, int y) = validPoints[randomIndex];

                if (TryTerraformAndPlace(x, y, buildingEntity, context, validPoints)) return;

                // validPoints[^1] == validPoints[validPoints.Count - 1]
                validPoints[randomIndex] = validPoints[^1];
                validPoints.RemoveAt(validPoints.Count - 1);
            }
            throw new Exception("Failed to generate structure!");
        }
        #endregion

        #region Private methods
        private void FillStructureMap(bool[,] structureMap, int startX, int startY, BuildingEntity buildingEntity, List<(int X, int Y)> validPoints)
        {
            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    structureMap[startX + i, startY + j] = true;
                    validPoints.Remove((startX + i, startY + j));
                }
            }
        }

        private bool TryTerraformAndPlace(int startX, int startY, BuildingEntity buildingEntity, MapGenerationContext context, List<(int X, int Y)> validPoints)
        {
            var heightMap = context.HeightMap;
            var waterMap = context.WaterMap;
            var structureMap = context.StructureMap;

            if (startX < 0 || startY < 0 || startX + buildingEntity.Width > context.Width || startY + buildingEntity.Height > context.Height) return false;

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
                    sumHeight += heightMap[startX + i, startY + j];
                }
            }

            int targetHeight = (int)Math.Round((double)sumHeight / (buildingEntity.Width * buildingEntity.Height));
            if (!IsNeighbouringHeightValid(startX, startY, buildingEntity, heightMap, targetHeight)) return false;

            for (int i = 0; i < buildingEntity.Width; i++)
            {
                for (int j = 0; j < buildingEntity.Height; j++)
                {
                    heightMap[startX + i, startY + j] = targetHeight;
                }
            }

            Debug.WriteLine($"Force placing {buildingEntity.GetType().Name} at ({startX}, {startY}) with terraforming to height {targetHeight}");

            buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);
            FillStructureMap(structureMap, startX, startY, buildingEntity, validPoints);

            return true;
        }

        private bool IsNeighbouringHeightValid(int startX, int startY, BuildingEntity buildingEntity, int[,] heightMap, int targetHeight)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            // TODO (optimization): skip inside
            for (int i = -1; i < buildingEntity.Width + 1; i++)
            {
                for (int j = -1; j < buildingEntity.Height + 1; j++)
                {
                    int currentX = startX + i;
                    int currentY = startY + j;
                    if (!(0 <= currentX && currentX < width && 0 <= currentY && currentY < height)) continue;
                    if (heightMap[currentX, currentY] > targetHeight + 1 || heightMap[currentX, currentY] < targetHeight - 1) return false;
                }
            }
            return true;
        }

        protected List<(int X, int Y)> GetValidPointsForPlacement(MapGenerationContext context, int buildingWidth, int buildingHeight)
        {
            var validPoints = new List<(int X, int Y)>(context.Width * context.Height);

            for (int i = 0; i <= context.Width - buildingWidth; i++)
            {
                for (int j = 0; j <= context.Height - buildingHeight; j++)
                {
                    validPoints.Add((i, j));
                }
            }

            return validPoints;
        }

        private bool IsValidPlacement(int startX, int startY, BuildingEntity buildingEntity, MapGenerationContext context)
        {
            var heightMap = context.HeightMap;
            var waterMap = context.WaterMap;
            var structureMap = context.StructureMap;

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

        private bool TryPlaceNear(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
        {
            var heightMap = context.HeightMap;
            var structureMap = context.StructureMap;

            for (int attempt = 0; attempt < MAX_TRY_PLACE_NEAR_ATTEMPTS; attempt++)
            {
                // Pick a spot within the circle
                if (!GetStartPosition(centerX, centerY, buildingEntity, context, out int startX, out int startY, random)) continue;

                if (IsValidPlacement(startX, startY, buildingEntity, context))
                {
                    buildingEntity.GenerateBuildingPoints(startX, startY, heightMap);
                    FillStructureMap(structureMap, startX, startY, buildingEntity, validPoints);
                    return true;
                }
            }
            return false;
        }

        private void ForcePlaceNear(BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY, IRandom random, List<(int X, int Y)> validPoints)
        {
            for (int attempt = 0; attempt < MAX_FORCE_PLACE_NEAR_ATTEMPTS; attempt++)
            {
                // Pick a spot within the circle
                if (!GetStartPosition(centerX, centerY, buildingEntity, context, out int startX, out int startY, random)) continue;

                if (TryTerraformAndPlace(startX, startY, buildingEntity, context, validPoints)) return;
            }
            ForcePlace(buildingEntity, context, -1, -1, random, validPoints);
        }

        private bool GetStartPosition(int centerX, int centerY, BuildingEntity buildingEntity, MapGenerationContext context, out int startX, out int startY, IRandom random)
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

            int dx = random.Next(-maxRange, maxRange + 1);
            int dy = random.Next(-maxRange, maxRange + 1);
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
        #endregion
    }
}
