namespace TransportTycoon.MapData.MapGenerator.TerrainGeneration
{
    public interface ITerrainGenerator
    {
        public int[,] GenerateTerrain(IBiome biome, MapGenerationContext context);
    }
}
