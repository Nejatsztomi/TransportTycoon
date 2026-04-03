using TransportTycoon.MapData.MapGenerator.NoiseGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
    public static class MapGeneratorFactory
    {
        public static IMapGenerator CreateMapGenerator(MapGenerationSettings settings)
        {
            INoiseGenerator noiseGenerator = PerlinNoiseGeneratorFactory.Create();
            ITerrainGenerator terrainGenerator = TerraingGeneratorFactory.Create(noiseGenerator, settings.TerrainNoiseScale);
            IForestGenerator forestGenerator = ForestGeneratorFactory.Create(noiseGenerator, settings.ForestNoiseScale, settings.ForestPercentage);
            IWaterGenerator waterGenerator = WaterGeneratorFactory.Create(noiseGenerator, settings.WaterNoiseScale);
            return new MapGenerator(settings, terrainGenerator, forestGenerator, waterGenerator);
        }
    }

    internal class MapGenerator : IMapGenerator
    {
        #region Properties
        private MapGenerationSettings Settings { get; }
        private ITerrainGenerator TerrainGenerator { get; }
        private IForestGenerator ForestGenerator { get; }
        private IWaterGenerator WaterGenerator { get; }
        #endregion

        #region Constructors
        public MapGenerator(MapGenerationSettings settings, ITerrainGenerator terrainGenerator, IForestGenerator forestGenerator, IWaterGenerator waterGenerator)
        {
            Settings = settings;
            TerrainGenerator = terrainGenerator;
            ForestGenerator = forestGenerator;
            WaterGenerator = waterGenerator;
        }
        #endregion

        #region Public methods
        public Field[,] GenerateMap(MapGenerationContext context)
        {
            int[,] heightMap = TerrainGenerator.GenerateTerrain(Settings.Biome, context);
            int[,] forestMap = ForestGenerator.GenerateForests(heightMap, context);
            bool[,] waterMap = WaterGenerator.GenerateWaterMap(Settings.RiverCount, heightMap, context);

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
            return map;
        }
        #endregion
    }
}
