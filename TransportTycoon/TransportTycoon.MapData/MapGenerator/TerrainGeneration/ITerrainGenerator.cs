namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface ITerrainGenerator : IMapPluginGenerator
    {
        public int[,] GenerateTerrain(float[,] noiseMap, MapGenerationContext context);
    }
}
