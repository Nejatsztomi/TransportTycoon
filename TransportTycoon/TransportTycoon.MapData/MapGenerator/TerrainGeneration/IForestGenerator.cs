namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IForestGenerator : IMapPluginGenerator
    {
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context);
    }
}
