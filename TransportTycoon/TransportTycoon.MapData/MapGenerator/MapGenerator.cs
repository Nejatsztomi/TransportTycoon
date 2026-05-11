using System.Diagnostics;
using System.Runtime.CompilerServices;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// A factory class for creating instances of <see cref="IMapGenerator"/>.
    /// This class is responsible for assembling the various components and generators required to produce a complete map, including terrain, water, forests, and structures.
    /// By centralizing the creation logic, it allows for easy configuration and extension of the map generation process, enabling different types of maps to be generated based on varying algorithms and strategies.
    /// </summary>
    public static class MapGeneratorFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IMapGenerator"/> based on the provided <see cref="MapGenerationContext"/>.
        /// </summary>
        /// <param name="context">The context containing the settings and parameters for map generation.</param>
        /// <returns>A new instance of <see cref="IMapGenerator"/> initialized with the specified context.</returns>
        public static IMapGenerator CreateMapGenerator(MapGenerationContext context)
        {
            var randomProvider = new RandomProvider();

            var noiseGenerator = ValueNoiseGeneratorFactory.Create(0.05f);

            var terrainGenerator = TerraingGeneratorFactory.Create();
            var forestGenerator = ForestGeneratorFactory.Create(noiseGenerator);
            var riverGenerator = RiverGeneratorFactory.Create(randomProvider, context);
            var structureGenerator = StructureGeneratorFactory.Create(randomProvider, context);
            var cityGenerator = CityGeneratorFactory.Create(randomProvider, context);
            List<IMapPluginGenerator> generators = [noiseGenerator, terrainGenerator, riverGenerator, structureGenerator, cityGenerator, forestGenerator];
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
        internal MapGenerator(IEnumerable<IMapPluginGenerator> generators, IRandomProvider randomProvider)
        {
            _generators = generators;
            _random = randomProvider;
        }
        #endregion

        #region Public methods
        public (Field[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context)
        {
            var random = _random.GetRandom(context.Seed, PluginId);

            var sortedGenerators = _generators.OrderBy(g => g.Phase).ToList();
            var structures = new List<BuildingEntity>(context.Settings.MaxCities + context.Settings.MaxStructure);

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
                    var generatedStructures = structureGen.GenerateStructures(context);
                    structures.AddRange(generatedStructures);
                }
                _stopwatch.Stop();
                Debug.WriteLine($"{generator.GetType().Name} took: {_stopwatch.ElapsedMilliseconds} ms");
            }

            _stopwatch.Restart();
            Field[,] map = ConvertHeightMapToFields(context);
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
        private void PlaceDownStructuresOnMap(Field[,] map, List<BuildingEntity> structures)
        {
            structures.ForEach(structure =>
            {
                structure.MapPoints.ToList()
                .ForEach(kv =>
                {
                    map[kv.Key.X, kv.Key.Y] = kv.Value;
                });
            });
        }

        private Field[,] ConvertHeightMapToFields(MapGenerationContext context)
        {
            var map = new Field[context.Width, context.Height];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Array2DCopy<T>(T[,] source, T[,] destination, int size)
        {
            int totalBytes = source.Length * size;
            Buffer.BlockCopy(source, 0, destination, 0, totalBytes);
        }
        #endregion
    }
}
