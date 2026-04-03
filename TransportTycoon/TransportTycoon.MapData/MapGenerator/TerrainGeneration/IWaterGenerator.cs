namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IWaterGenerator
    {
        public bool[,] GenerateWaterMap(int[,] heightMap, MapGenerationContext context);
    }
}
