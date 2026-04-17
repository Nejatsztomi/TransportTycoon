namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public static class RiverGeneratorFactory
    {
        public static IWaterGenerator Create(IRandomProvider randomProvider, MapGenerationContext context) => new RiverGenerator(randomProvider, context);
    }

    internal class RiverGenerator : IWaterGenerator
    {
        #region Private fields
        private readonly IRandom _random;
        #endregion

        #region Public properties
        public GenerationPhase Phase => GenerationPhase.WaterLayer;
        #endregion

        #region Constructors
        public RiverGenerator(IRandomProvider randomProvider, MapGenerationContext context)
        {
            _random = randomProvider.GetRandom(context.Seed, "BaseGame.Rivers");
        }
        #endregion

        #region Public methods
        public bool[,] GenerateWaterMap(float[,] noiseMap, bool[,] waterMap, MapGenerationContext context)
        {
            for (int i = 0; i < context.Settings.RiverCount; i++)
            {
                SpawnRiver(noiseMap, waterMap, context);
            }

            return waterMap;
        }
        #endregion

        #region Private methods
        private void SpawnRiver(float[,] noiseMap, bool[,] waterMap, MapGenerationContext context)
        {

            int currentX = _random.Next(5, context.Width - 5);
            int currentY = _random.Next(5, context.Height - 5);

            if (waterMap[currentX, currentY]) return;
            bool[,] preexistingWaterMap = (bool[,])waterMap.Clone();

            // Width = sqrt(radius)
            float minRadius = (float)Math.Sqrt(context.Settings.MinRiverWidth);
            float maxRadius = (float)Math.Sqrt(context.Settings.MaxRiverWidth);
            float currentRadius = (float)Math.Sqrt(_random.Next(context.Settings.MinRiverWidth, context.Settings.MaxRiverWidth));

            HashSet<(int X, int Y)> alreadyVisited = [];

            while (true)
            {
                alreadyVisited.Add((currentX, currentY));

                PaintWaterBrush(noiseMap, waterMap, currentX, currentY, currentRadius, context);

                float widthChange = (_random.NextSingle() * 0.4f) - 0.2f;
                currentRadius = Math.Clamp(currentRadius + widthChange, minRadius, maxRadius);

                // Go downhill
                (int x, int y) nextTile = (-1, -1);
                float lowestHeight = float.MaxValue;

                List<(int dx, int dy)> directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

                while (directions.Count > 0)
                {
                    int idx = _random.Next(directions.Count);
                    (int dx, int dy) = directions[idx];
                    directions.RemoveAt(idx);

                    int nx = currentX + dx;
                    int ny = currentY + dy;

                    if ((0 <= nx && nx < context.Width) && (0 <= ny && ny < context.Height))
                    {
                        if (!alreadyVisited.Contains((nx, ny)))
                        {
                            if (noiseMap[nx, ny] < lowestHeight ||
                                (Math.Abs(noiseMap[nx, ny] - lowestHeight) <= 0.0001f && lowestHeight >= noiseMap[currentX, currentY]))
                            {
                                lowestHeight = noiseMap[nx, ny];
                                nextTile = (nx, ny);
                            }
                        }
                    }
                }

                // No next tile
                if (nextTile == (-1, -1))
                {
                    PaintWaterBrush(noiseMap, waterMap, currentX, currentY, 3.5f, context);
                    break;
                }

                if (preexistingWaterMap[nextTile.x, nextTile.y])
                {
                    break;
                }

                // Iterate
                currentX = nextTile.x;
                currentY = nextTile.y;
            }
        }

        /// <summary>
        /// Paints a circle of water around a center point based on a radius.
        /// </summary>
        private void PaintWaterBrush(float[,] noiseMap, bool[,] waterMap, int centerX, int centerY, float radius, MapGenerationContext context)
        {
            int r = (int)Math.Ceiling(radius);

            for (int i = -r; i <= r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    // Circle like radius
                    if (i * i + j * j <= radius * radius)
                    {
                        int mapX = centerX + i;
                        int mapY = centerY + j;
                        if ((0 <= mapX && mapX < context.Width) && (0 <= mapY && mapY < context.Height))
                        {
                            waterMap[mapX, mapY] = IsValidNeighbouringHeights(mapX, mapY, noiseMap, context);
                        }
                    }
                }
            }
        }

        private bool IsValidNeighbouringHeights(int x, int y, float[,] noiseMap, MapGenerationContext context)
        {
            // TODO: Replace magic number with TerrainHeight enum
            bool valid = true;
            if (x + 1 < context.Width) valid &= noiseMap[x + 1, y] <= context.Settings.Biome.HillRange;
            if (0 <= x - 1) valid &= noiseMap[x - 1, y] <= context.Settings.Biome.HillRange;
            if (y + 1 < context.Height) valid &= noiseMap[x, y + 1] <= context.Settings.Biome.HillRange;
            if (0 <= y - 1) valid &= noiseMap[x, y - 1] <= context.Settings.Biome.HillRange;
            return valid;
        }
        #endregion
    }
}
