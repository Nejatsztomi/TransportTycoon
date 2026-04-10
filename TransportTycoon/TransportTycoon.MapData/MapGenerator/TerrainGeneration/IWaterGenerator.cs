namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IWaterGenerator
    {
        public bool[,] GenerateWaterMap(int[,] heightMap, bool[,] waterMap, MapGenerationContext context);
    }
}
