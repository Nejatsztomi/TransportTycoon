namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IWaterGenerator
    {
        public bool[,] GenerateWaterMap(float[,] noiseMap, bool[,] waterMap, MapGenerationContext context);
    }
}
