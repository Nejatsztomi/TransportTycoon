using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    /// <summary>
    /// A factory class for creating instances of <see cref="IStructureGenerator"/> that specifically generate city structures.
    /// </summary>
    public static class CityGeneratorFactory
    {
        /// <summary>
        /// Creates a new instance of an implementation of the IStructureGenerator interface using the specified random
        /// provider and map generation context.
        /// </summary>
        /// <param name="randomProvider">The random number provider to use for procedural generation. Cannot be null.</param>
        /// <param name="context">The context containing configuration and state information for map generation. Cannot be null.</param>
        /// <returns>An IStructureGenerator instance configured with the specified random provider and context.</returns>
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

        #region Constructors
        internal CityGenerator(IRandomProvider randomProvider, MapGenerationContext context) : base(randomProvider, context) { }
        #endregion

        #region Public methods
        public override List<BuildingEntity> GenerateStructures(MapGenerationContext context)
        {
            var random = _random.GetRandom(context.Seed, PluginId);
            var structures = new List<BuildingEntity>(context.Settings.MaxStructure);

            var validPoints = GetValidPointsForPlacement(context, context.Settings.CityWidth, context.Settings.CityHeight);

            validPoints.RemoveAll(point =>
                context.WaterMap[point.X, point.Y] ||
                context.StructureMap[point.X, point.Y] ||
                context.HeightMap[point.X, point.Y] >= 4);

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
        /// <param name="city">The city entity to generate roads for.</param>
        /// <param name="context">The map generation context.</param>
        /// <param name="random">The random number generator.</param>
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

        /// <summary>
        /// Force carves a main exit road from the city center to the edge of the city bounds.
        /// The exit direction is random, and the path is not straight but has a chance to side step, creating a more natural look.
        /// </summary>
        /// <remarks>
        /// The code coverage tools may not properly recognize the tests for this method, even though it is tested in CityGeneratorTest.cs.
        /// A further class extraction layer might be needed to improve testability and code coverage reporting for this method.
        /// </remarks>
        /// <param name="city">The city entity to generate the exit road for.</param>
        /// <param name="startX">The starting X coordinate of the exit road.</param>
        /// <param name="startY">The starting Y coordinate of the exit road.</param>
        /// <param name="random">The random number generator.</param>
        private void CarveExit(CityEntity city, int startX, int startY, IRandom random)
        {
            (int topLeftX, int topLeftY) = city.TopLeftPoints;

            int x = startX;
            int y = startY;

            // North, East, South, West
            (int dx, int dy) = _directions[random.Next(4)];

            while (IsInsideTheCity(x, y, topLeftX, topLeftY, city.Width, city.Height))
            {
                city.MapPoints[(x, y)] = new Road(x, y, RoadType.XRoad, city.MapPoints[(x, y)].Height);

                // Random movement
                if (random.NextDouble() > 0.8)
                {
                    int sideStep = random.NextDouble() > 0.5 ? 1 : -1;
                    // Tend to move in the other axis
                    if (dx == 0)
                    {
                        x = Math.Clamp(x + sideStep, topLeftX, topLeftX + city.Width - 1);
                    }
                    else
                    {
                        y = Math.Clamp(y + sideStep, topLeftY, topLeftY + city.Height - 1);
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
                if (!IsInsideTheCity(nextX, nextY, topLeftX, topLeftY, city.Width, city.Height)) break;

                // Step the iteraion
                x = nextX;
                y = nextY;
            }
        }

        private bool IsInsideTheCity(int x, int y, int topLeftX, int topLeftY, int width, int height)
        {
            return topLeftX <= x && x < topLeftX + width && topLeftY <= y && y < topLeftY + height;
        }
        #endregion
    }
}
