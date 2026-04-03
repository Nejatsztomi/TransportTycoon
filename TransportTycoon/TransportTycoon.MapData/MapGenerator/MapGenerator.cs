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

            ITerrainGenerator terrainGenerator = TerraingGeneratorFactory.Create(noiseGenerator);
            IForestGenerator forestGenerator = ForestGeneratorFactory.Create(noiseGenerator);
            IWaterGenerator waterGenerator = LakeGeneratorFactory.Create(noiseGenerator);
            IStructureGenerator structureGenerator = StructureGeneratorFactory.Create(cityGenerator, randomProvider, context);
            return new MapGenerator(randomProvider.GetRandom(context.Seed, GenerationDomain.Map), terrainGenerator, forestGenerator, waterGenerator, structureGenerator);
        }
    }

    internal class MapGenerator : IMapGenerator
    {
        #region Private fields
        private readonly ITerrainGenerator _terrainGenerator;
        private readonly IForestGenerator _forestGenerator;
        private readonly IWaterGenerator _lakeGenerator;
        private readonly IStructureGenerator _structureGenerator;
        private readonly IRandom _random;
        #endregion

        #region Constructors
        public MapGenerator(IRandom random, ITerrainGenerator terrainGenerator, IForestGenerator forestGenerator, IWaterGenerator waterGenerator, IStructureGenerator structureGenerator)
        {
            _terrainGenerator = terrainGenerator;
            _forestGenerator = forestGenerator;
            _lakeGenerator = waterGenerator;
            _structureGenerator = structureGenerator;
            _random = random;
        }
        #endregion

        #region Public methods
        public Field[,] GenerateMap(MapGenerationContext context)
        {
            int[,] heightMap = _terrainGenerator.GenerateTerrain(context);
            bool[,] lakeMap = _lakeGenerator.GenerateWaterMap(heightMap, context);

            bool[,] waterMap = new bool[context.Width, context.Height];
            for (int i = 0; i < context.Width; i++)
            {
                for (int j = 0; j < context.Height; j++)
                {
                    waterMap[i, j] = lakeMap[i, j] || heightMap[i, j] == 0;
                }
            }

            int[,] forestMap = _forestGenerator.GenerateForests(heightMap, context);
            bool[,] structureMap = GenerateEmptyStructureMap(context.Width, context.Height);

            List<BuildingEntity> structures = [];

            // Force generates N cities
            for (int i = 0; i < context.Settings.MinCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, city, context, -1, -1);
                structures.Add(city);
            }

            // Try to generate rest
            for (int i = context.Settings.MinCities; i < context.Settings.MaxCities; i++)
            {
                CityEntity city = new(context.Settings.CityWidth, context.Settings.CityHeight);
                _structureGenerator.TryPlace(heightMap, waterMap, structureMap, city, context, -1, -1);
                structures.Add(city);
            }

            // Force place other structures in pairs
            for (int i = 1; i < context.Settings.MinStructure; i += 2)
            {
                (SiteEntity se, IndustryEntity ie) = GenerateRandomEntityPair();
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, se, context, -1, -1);
                (int x, int y) = se.TopLeftPoints;
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, ie, context, x, y);

                structures.Add(se);
                structures.Add(ie);
            }

            // Handle odd number of structures if necessary
            if (context.Settings.MinStructure % 2 != 0)
            {
                BuildingEntity be = GenerateRandomEntity();
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, be, context, -1, -1);
                structures.Add(be);
            }

            for (int i = context.Settings.MinStructure; i < context.Settings.MaxStructure; i++)
            {
                BuildingEntity be = GenerateRandomEntity();
                _structureGenerator.ForcePlace(heightMap, waterMap, structureMap, be, context, -1, -1);
                structures.Add(be);
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

        private (SiteEntity se, IndustryEntity ie) GenerateRandomEntityPair()
        {
            return _random.Next(0, 3) switch
            {
                0 => (new FarmEntity(), new MillEntity()),
                1 => (new MineEntity(), new PlantEntity()),
                _ => (new LumberCampEntity(), new FactoryEntity())
            };
        }

        private BuildingEntity GenerateRandomEntity()
        {
            return GenerateRandomEntityPair() switch
            {
                (SiteEntity se, IndustryEntity ie) => _random.Next(0, 2) == 0 ? se : ie
            };
        }
        #endregion
    }
}
