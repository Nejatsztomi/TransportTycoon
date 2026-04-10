namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface ITerrainGenerator
    {
        public int[,] GenerateTerrain(MapGenerationContext context);
    }
}
