using System.Diagnostics;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    public static class MapGeneratorFactory
    {
        public static IMapGenerator CreateMapGenerator(MapGenerationContext context)
        {
            IRandomProvider randomProvider = new RandomProvider();

            INoiseGenerator noiseGenerator = ValueNoiseGeneratorFactory.Create(0.05f);
            ICityGenerator cityGenerator = CityGeneratorFactory.Create(randomProvider, context);

            ITerrainGenerator terrainGenerator = TerraingGeneratorFactory.Create();
            IForestGenerator forestGenerator = ForestGeneratorFactory.Create(noiseGenerator);
            IWaterGenerator riverGenerator = RiverGeneratorFactory.Create(randomProvider, context);
            IStructureGenerator structureGenerator = StructureGeneratorFactory.Create(cityGenerator, randomProvider, context);
            List<IMapPluginGenerator> generators = [noiseGenerator, terrainGenerator, riverGenerator, structureGenerator, forestGenerator];
            return new MapGenerator(generators, randomProvider);
        }
    }

    internal sealed class MapGenerator : IMapGenerator
    {
        #region Private fields
        private readonly IEnumerable<IMapPluginGenerator> _generators;
        private readonly IRandomProvider _random;
        private const string PluginId = "BaseGame.Map";
        #endregion

        #region Debug
        private readonly Stopwatch _stopwatch = new();
        #endregion

        #region Constructors
        public MapGenerator(IEnumerable<IMapPluginGenerator> generators, IRandomProvider randomProvider)
        {
            _generators = generators;
            _random = randomProvider;
        }
        #endregion

        #region Public methods
        public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context)
        {
            IRandom random = _random.GetRandom(context.Seed, PluginId);

            var sortedGenerators = _generators.OrderBy(g => g.Phase).ToList();
#pragma warning disable IDE0028 // Simplify collection initialization
            List<BuildingEntity> structures = new(context.Settings.MaxCities + context.Settings.MaxStructure);
#pragma warning restore IDE0028 // Simplify collection initialization

            foreach (var generator in sortedGenerators)
            {
                _stopwatch.Restart();
                if (generator is INoiseGenerator noiseGen)
                {
                    var noiseMap = noiseGen.GenerateNoiseMap(context.Width, context.Height, context.Seed);
                    Array2DCopy(noiseMap, context.NoiseMap, sizeof(float));
                }
                else if (generator is ITerrainGenerator terrainGen)
                {
                    var heightMap = terrainGen.GenerateTerrain(context.NoiseMap, context);
                    Array2DCopy(heightMap, context.HeightMap, sizeof(int));
                    for (int x = 0; x < context.Width; x++)
                    {
                        for (int y = 0; y < context.Height; y++)
                        {
                            if (heightMap[x, y] == 0)
                            {
                                context.WaterMap[x, y] = true;
                            }
                        }
                    }
                }
                else if (generator is IWaterGenerator waterGen)
                {
                    var waterMap = waterGen.GenerateWaterMap(context.NoiseMap, context.WaterMap, context);
                    Array2DCopy(waterMap, context.WaterMap, sizeof(bool));
                }
                else if (generator is IForestGenerator forestGen)
                {
                    var forestMap = forestGen.GenerateForests(context.HeightMap, context);
                    Array2DCopy(forestMap, context.ForestMap, sizeof(int));
                }
                else if (generator is IStructureGenerator structureGen)
                {
                    HandleStructureGeneration(context, structureGen, ref structures, random);
                }
                _stopwatch.Stop();
                Debug.WriteLine($"{generator.GetType().Name} took: {_stopwatch.ElapsedMilliseconds} ms");
            }

            _stopwatch.Restart();
            IField[,] map = ConvertHeightMapToFields(context);
            _stopwatch.Stop();
            Debug.WriteLine($"Map conversion took: {_stopwatch.ElapsedMilliseconds} ms");

            // Generate structures
            _stopwatch.Restart();
            PlaceDownStructuresOnMap(map, structures);
            _stopwatch.Stop();
            Debug.WriteLine($"Placing structures time: {_stopwatch.ElapsedMilliseconds} ms");

            return (map, structures);
        }
        #endregion

        #region Private methods
        private void HandleStructureGeneration(MapGenerationContext context, IStructureGenerator structureGenerator, ref List<BuildingEntity> structures, IRandom random)
        {
            for (int i = 0; i < context.Settings.MinCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                structureGenerator.ForcePlace(context.HeightMap, context.WaterMap, context.StructureMap, city, context, -1, -1);
                structures.Add(city);
            }

            // Try to generate rest
            for (int i = context.Settings.MinCities; i < context.Settings.MaxCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                structureGenerator.TryPlace(context.HeightMap, context.WaterMap, context.StructureMap, city, context, -1, -1);
                structures.Add(city);
            }

            for (int i = 1; i < context.Settings.MinStructure; i += 2)
            {
                (SiteEntity se, IndustryEntity ie) = GenerateRandomEntityPair(random);
                structureGenerator.ForcePlace(context.HeightMap, context.WaterMap, context.StructureMap, se, context, -1, -1);
                (int x, int y) = se.TopLeftPoints;
                structureGenerator.ForcePlace(context.HeightMap, context.WaterMap, context.StructureMap, ie, context, x, y);

                structures.Add(se);
                structures.Add(ie);
            }

            // Handle odd number of structures if necessary
            if (context.Settings.MinStructure % 2 != 0)
            {
                BuildingEntity be = GenerateRandomEntity(random);
                structureGenerator.ForcePlace(context.HeightMap, context.WaterMap, context.StructureMap, be, context, -1, -1);
                structures.Add(be);
            }

            for (int i = context.Settings.MinStructure; i < context.Settings.MaxStructure; i++)
            {
                BuildingEntity be = GenerateRandomEntity(random);
                structureGenerator.ForcePlace(context.HeightMap, context.WaterMap, context.StructureMap, be, context, -1, -1);
                structures.Add(be);
            }
        }

        private void PlaceDownStructuresOnMap(IField[,] map, List<BuildingEntity> structures)
        {
            structures.ForEach(structure =>
            {
                structure.MapPoints.ToList()
                .ForEach(kv =>
                {
                    map[kv.Key.X, kv.Key.Y] = kv.Value;
                    //Debug.WriteLine($"Placing {kv.Value.FieldType} at ({kv.Key.X}, {kv.Key.Y})");
                });
                //Debug.WriteLine($"Placed {structure.GetType().Name} at ({structure.MapPoints.Keys.First().X}, {structure.MapPoints.Keys.First().Y})");
            });
        }

        private IField[,] ConvertHeightMapToFields(MapGenerationContext context)
        {
            IField[,] map = new IField[context.Width, context.Height];
            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
                {
                    if (context.WaterMap[x, y] || context.HeightMap[x, y] == 0)
                    {
                        map[x, y] = new Water(x, y);
                    }
                    else
                    {
                        Terrain terrain = new(x, y, context.HeightMap[x, y])
                        {
                            Trees = context.ForestMap[x, y]
                        };
                        map[x, y] = terrain;
                    }
                }
            }
            return map;
        }

        private (SiteEntity se, IndustryEntity ie) GenerateRandomEntityPair(IRandom random)
        {
            return random.Next(0, 3) switch
            {
                0 => (new FarmEntity(), new MillEntity()),
                1 => (new MineEntity(), new PlantEntity()),
                _ => (new LumberCampEntity(), new FactoryEntity())
            };
        }

        private BuildingEntity GenerateRandomEntity(IRandom random)
        {
            return GenerateRandomEntityPair(random) switch
            {
                (SiteEntity se, IndustryEntity ie) => random.Next(0, 2) == 0 ? se : ie
            };
        }

        private void Array2DCopy<T>(T[,] source, T[,] destination, int size)
        {
            int totalBytes = source.Length * size;
            Buffer.BlockCopy(source, 0, destination, 0, totalBytes);
        }
        #endregion
    }
}
