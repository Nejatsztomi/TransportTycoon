namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IWaterGenerator
    {
        public bool[,] GenerateWaterMap(int riverCount, int[,] heightMap, MapGenerationContext context);
    }
}
