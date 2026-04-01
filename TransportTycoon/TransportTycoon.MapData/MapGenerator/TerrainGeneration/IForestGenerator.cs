namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface IForestGenerator
    {
        public int[,] GenerateForests(MapGenerationContext context);
    }
}
