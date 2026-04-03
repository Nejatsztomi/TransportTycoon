using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class CityGeneratorFactory
    {
        public static ICityGenerator Create(IRandomProvider randomProvider, MapGenerationContext context) => new CityGenerator(randomProvider, context);
    }

    internal class CityGenerator : ICityGenerator
    {
        #region Private fields
        private readonly IRandom _random;
        #endregion

        #region Constructor
        public CityGenerator(IRandomProvider randomProvider, MapGenerationContext context)
        {
            _random = randomProvider.GetRandom(context.Seed, GenerationDomain.Cities);
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Implementaion of the Drunken Builder algorithm.
        /// It spawns a number of random walkers (branchCount) that carve roads in the city for a certain number of steps (maxRoadCount).
        /// </summary>
        /// <param name="city"></param>
        /// <param name="context"></param>
        public void GenerateCity(BuildingEntity city, MapGenerationContext context)
        {
            if (city is not CityEntity) return;
            (int topLeftX, int topLeftY) = city.MapPoints.First().Key;

            int centerX = topLeftX + city.Width / 2;
            int centerY = topLeftY + city.Height / 2;
            city.MapPoints[(centerX, centerY)] = new Road(centerX, centerY, RoadType.XRoad, city.MapPoints[(centerX, centerY)].Height);

            CarveExit(city, centerX, centerY);

            // 3. Spawn Random Walkers to build the internal streets
            for (int i = 0; i < context.Settings.BranchCount; i++)
            {
                CarveRoad(city, centerX, centerY, context.Settings.RoadLength);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gurantes and exit from the city to the main road network.
        /// </summary>
        /// <param name="city"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        private void CarveExit(BuildingEntity city, int startX, int startY)
        {
            (int topLeftX, int topLeftY) = city.MapPoints.First().Key;

            int x = startX;
            int y = startY;

            // North, East, South, West
            (int dx, int dy)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) };
            (int dx, int dy) = directions[_random.Next(4)];

            while (x <= topLeftX && x < topLeftX + city.Width &&
                    topLeftY <= y && y < topLeftY + city.Height)
            {
                city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height);

                // Random movement
                if (_random.NextDouble() > 0.8)
                {
                    int sideStep = _random.NextDouble() > 0.5 ? 1 : -1;
                    // Tend to move in the other axis
                    if (dx == 0)
                    {
                        x = Math.Clamp(x + sideStep, 0, city.Width - 1);
                    }
                    else
                    {
                        y = Math.Clamp(y + sideStep, 0, city.Height - 1);
                    }
                    city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height);
                }

                // Step the iteration
                x += dx;
                y += dy;
            }
        }

        /// <summary>
        /// Carves road into the city.
        /// Uses the same algorithm <see cref="CarveExit(BuildingEntity, int, int, Random)"/> but don't loop until the edge of the city, but only for a certain number of steps (maxRoadCount).
        /// </summary>
        /// <param name="city"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="maxRoadCount"></param>
        private void CarveRoad(BuildingEntity city, int startX, int startY, int maxRoadCount)
        {
            (int topLeftX, int topLeftY) = city.MapPoints.First().Key;

            int x = startX;
            int y = startY;

            // North, East, South, West
            (int dx, int dy)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) };
            (int dx, int dy) currentDir = directions[_random.Next(4)];

            for (int step = 0; step < maxRoadCount; step++)
            {
                // Place road
                city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height);

                if (_random.NextDouble() < 0.3)
                {
                    currentDir = directions[_random.Next(4)];
                }

                int nextX = x + currentDir.dx;
                int nextY = y + currentDir.dy;

                // If the road hits the edge of the city bounds, stop this branch
                if (!(topLeftX <= nextX && nextX < topLeftX + city.Width) || !(topLeftY <= nextY && nextY < topLeftY + city.Height))
                {
                    break;
                }

                // Step the iteraion
                x = nextX;
                y = nextY;
            }
        }
        #endregion
    }
}
