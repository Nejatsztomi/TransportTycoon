using System.Diagnostics;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.StructureGeneration;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    public static class MapGeneratorFactory
    {
        public static IMapGenerator CreateMapGenerator(MapGenerationSettings settings)
        {
            INoiseGenerator noiseGenerator = PerlinNoiseGeneratorFactory.Create();
            ICityGenerator cityGenerator = CityGeneratorFactory.Create();

            ITerrainGenerator terrainGenerator = TerraingGeneratorFactory.Create(noiseGenerator, settings.TerrainNoiseScale);
            IForestGenerator forestGenerator = ForestGeneratorFactory.Create(noiseGenerator, settings.ForestNoiseScale, settings.ForestPercentage);
            IWaterGenerator waterGenerator = WaterGeneratorFactory.Create(noiseGenerator, settings.WaterNoiseScale);
            IStructureGenerator structureGenerator = StructureGeneratorFactory.Create(cityGenerator);
            return new MapGenerator(settings, terrainGenerator, forestGenerator, waterGenerator, structureGenerator);
        }
    }

    internal class MapGenerator : IMapGenerator
    {
        #region Properties
        private MapGenerationSettings Settings { get; }
        private ITerrainGenerator TerrainGenerator { get; }
        private IForestGenerator ForestGenerator { get; }
        private IWaterGenerator WaterGenerator { get; }
        private IStructureGenerator StructureGenerator { get; }
        #endregion

        #region Constructors
        public MapGenerator(MapGenerationSettings settings, ITerrainGenerator terrainGenerator, IForestGenerator forestGenerator, IWaterGenerator waterGenerator, IStructureGenerator structureGenerator)
        {
            Settings = settings;
            TerrainGenerator = terrainGenerator;
            ForestGenerator = forestGenerator;
            WaterGenerator = waterGenerator;
            StructureGenerator = structureGenerator;
        }
        #endregion

        #region Public methods
        public Field[,] GenerateMap(MapGenerationContext context)
        {
            int[,] heightMap = TerrainGenerator.GenerateTerrain(Settings.Biome, context);
            int[,] forestMap = ForestGenerator.GenerateForests(heightMap, context);
            bool[,] waterMap = WaterGenerator.GenerateWaterMap(Settings.RiverCount, heightMap, context);
            bool[,] structureMap = GenerateEmptyStructureMap(context.Width, context.Height);

            List<BuildingEntity> structures = [];

            // Force generates 2 cities
            for (int i = 0; i < 2; i++)
            {
                CityEntity city = new(5, 5);
                StructureGenerator.ForcePlace(heightMap, waterMap, structureMap, city, context, -1, -1, 0);
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
