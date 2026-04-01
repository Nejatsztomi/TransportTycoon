using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.MapData.MapGenerator
{
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
        public Field[,] GenerateMap(MapGenerationContext context) { }
        #endregion
    }
}
