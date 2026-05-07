using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class CityGeneratorFactory
    {
        public static IStructureGenerator Create(IRandomProvider randomProvider, MapGenerationContext context) => new CityGenerator(randomProvider, context);
    }

    internal class CityGenerator : BaseStructurePlacementGenerator
    {
        #region Private fields
        private static readonly (int dx, int dy)[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        private const string PluginId = "BaseGame.Cities";
        #endregion

        #region Public properties
        public override GenerationPhase Phase => GenerationPhase.Structures;
        #endregion

        #region Constructor
        internal CityGenerator(IRandomProvider randomProvider, MapGenerationContext context) : base(randomProvider, context) { }
        #endregion

        #region Public methods
        public override List<BuildingEntity> GenerateStructures(MapGenerationContext context)
        {
            var random = _random.GetRandom(context.Seed, PluginId);
            var structures = new List<BuildingEntity>(context.Settings.MaxStructure);

            var validPoints = new List<(int X, int Y)>(context.Width * context.Height);

            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    if (context.WaterMap[i, j] || context.StructureMap[i, j]) continue;
                    if (context.HeightMap[i, j] >= 4) continue;
                    validPoints.Add((i, j));
                }
            }

            for (int i = 0; i < context.Settings.MinCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                ForcePlace(city, context, -1, -1, random, validPoints);

                GenerateCity(city, context, random);
                structures.Add(city);
            }

            // Try to generate rest
            for (int i = context.Settings.MinCities; i < context.Settings.MaxCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                if (!TryPlace(city, context, -1, -1, random, validPoints)) continue;

                GenerateCity(city, context, random);
                structures.Add(city);
            }

            return structures;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Implementaion of the Drunken Builder algorithm.
        /// It spawns a number of random walkers (branchCount) that carve roads in the city for a certain number of steps (maxRoadCount).
        /// </summary>
        /// <param name="city"></param>
        /// <param name="context"></param>
        private void GenerateCity(CityEntity city, MapGenerationContext context, IRandom random)
        {
            (int topLeftX, int topLeftY) = city.TopLeftPoints;

            int centerX = topLeftX + city.Width / 2;
            int centerY = topLeftY + city.Height / 2;
            city.MapPoints[(centerX, centerY)] = new Road(centerX, centerY, RoadType.XRoad, city.MapPoints[(centerX, centerY)].Height, city);

            CarveExit(city, centerX, centerY, random);

            // 3. Spawn Random Walkers to build the internal streets
            for (int i = 0; i < context.Settings.BranchCount; i++)
            {
                CarveRoad(city, centerX, centerY, context.Settings.RoadLength, random);
            }
        }

        private void CarveExit(CityEntity city, int startX, int startY, IRandom random)
        {
            (int topLeftX, int topLeftY) = city.TopLeftPoints;

            int x = startX;
            int y = startY;

            // North, East, South, West
            (int dx, int dy) = _directions[random.Next(4)];

            while (x <= topLeftX && x < topLeftX + city.Width &&
                    topLeftY <= y && y < topLeftY + city.Height)
            {
                city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height);

                // Random movement
                if (random.NextDouble() > 0.8)
                {
                    int sideStep = random.NextDouble() > 0.5 ? 1 : -1;
                    // Tend to move in the other axis
                    if (dx == 0)
                    {
                        x = Math.Clamp(x + sideStep, 0, city.Width - 1);
                    }
                    else
                    {
                        y = Math.Clamp(y + sideStep, 0, city.Height - 1);
                    }
                    city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height, city);
                }

                // Step the iteration
                x += dx;
                y += dy;
            }
        }

        private void CarveRoad(CityEntity city, int startX, int startY, int maxRoadCount, IRandom random)
        {
            (int topLeftX, int topLeftY) = city.TopLeftPoints;

            int x = startX;
            int y = startY;

            // North, East, South, West
            (int dx, int dy) currentDir = _directions[random.Next(4)];

            for (int step = 0; step < maxRoadCount; step++)
            {
                // Place road
                city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height, city);

                if (random.NextDouble() < 0.3) currentDir = _directions[random.Next(4)];

                int nextX = x + currentDir.dx;
                int nextY = y + currentDir.dy;

                // If the road hits the edge of the city bounds, stop this branch
                if (!(topLeftX <= nextX && nextX < topLeftX + city.Width) || !(topLeftY <= nextY && nextY < topLeftY + city.Height)) break;

                // Step the iteraion
                x = nextX;
                y = nextY;
            }
        }
        #endregion
    }
}
