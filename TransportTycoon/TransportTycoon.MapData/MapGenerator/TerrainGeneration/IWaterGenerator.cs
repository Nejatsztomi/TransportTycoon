namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IWaterGenerator : IMapPluginGenerator
    {
        public bool[,] GenerateWaterMap(float[,] noiseMap, bool[,] waterMap, MapGenerationContext context);
    }
}
