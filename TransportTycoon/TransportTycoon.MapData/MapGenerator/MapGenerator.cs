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

            INoiseGenerator noiseGenerator = PerlinNoiseGeneratorFactory.Create(randomProvider, context);
            ICityGenerator cityGenerator = CityGeneratorFactory.Create(randomProvider, context);

            ITerrainGenerator terrainGenerator = TerraingGeneratorFactory.Create(noiseGenerator, randomProvider, context);
            IForestGenerator forestGenerator = ForestGeneratorFactory.Create(noiseGenerator, randomProvider, context);
            IWaterGenerator waterGenerator = WaterGeneratorFactory.Create(noiseGenerator, randomProvider, context);
            IStructureGenerator structureGenerator = StructureGeneratorFactory.Create(cityGenerator, randomProvider, context);
            return new MapGenerator(terrainGenerator, forestGenerator, waterGenerator, structureGenerator);
        }
    }

    internal class MapGenerator : IMapGenerator
    {
        #region Private fields
        private readonly ITerrainGenerator _terrainGenerator;
        private readonly IForestGenerator _forestGenerator;
        private readonly IWaterGenerator _waterGenerator;
        private readonly IStructureGenerator _structureGenerator;
        #endregion

        #region Constructors
        public MapGenerator(ITerrainGenerator terrainGenerator, IForestGenerator forestGenerator, IWaterGenerator waterGenerator, IStructureGenerator structureGenerator)
        {
            _terrainGenerator = terrainGenerator;
            _forestGenerator = forestGenerator;
            _waterGenerator = waterGenerator;
            _structureGenerator = structureGenerator;
        }
        #endregion

        #region Public methods
        public Field[,] GenerateMap(MapGenerationContext context)
        {
            int[,] heightMap = _terrainGenerator.GenerateTerrain(context);
            int[,] forestMap = _forestGenerator.GenerateForests(heightMap, context);
            bool[,] waterMap = _waterGenerator.GenerateWaterMap(heightMap, context);
            bool[,] structureMap = GenerateEmptyStructureMap(context.Width, context.Height);

            List<BuildingEntity> structures = [];

            // Force generates 2 cities
            for (int i = 0; i < 2; i++)
            {
                CityEntity city = new(5, 5);
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, city, context, -1, -1);
                structures.Add(city);
            }

            // Generate terrain
            Field[,] map = new Field[context.Width, context.Height];
            for (int x = 0; x < context.Width; x++)
            {
                for (int y = 0; y < context.Height; y++)
                {
                    if (waterMap[x, y])
                    {
                        map[x, y] = new Water(x, y);
                    }
                    else
                    {
                        Terrain terrain = new(x, y, heightMap[x, y])
                        {
                            Trees = forestMap[x, y]
                        };
                        map[x, y] = terrain;
                    }
                }
            }

            // Generate structures
            structures.ForEach(structure =>
            {
                structure.MapPoints.ToList()
                .ForEach(kv =>
                {
                    map[kv.Key.X, kv.Key.Y] = kv.Value;
                    Debug.WriteLine($"Placing {kv.Value.FieldType} at ({kv.Key.X}, {kv.Key.Y})");
                });
                Debug.WriteLine($"Placed {structure.GetType().Name} at ({structure.MapPoints.Keys.First().X}, {structure.MapPoints.Keys.First().Y})");
            });

            return map;
        }
        #endregion

        #region Private methods
        private bool[,] GenerateEmptyStructureMap(int width, int height)
        {
            bool[,] structureMap = new bool[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    structureMap[i, j] = false;
                }
            }
            return structureMap;
        }
        #endregion
    }
}
