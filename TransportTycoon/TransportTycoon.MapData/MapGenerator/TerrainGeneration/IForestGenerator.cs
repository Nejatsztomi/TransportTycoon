namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IForestGenerator
    {
        public int[,] GenerateForests(int[,] heightMap, MapGenerationContext context);
    }
}
