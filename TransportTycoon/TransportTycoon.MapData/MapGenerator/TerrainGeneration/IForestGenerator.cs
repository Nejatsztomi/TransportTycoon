namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IForestGenerator
    {
        public int[,] GenerateForest(MapGenerationContext context);
    }
}
