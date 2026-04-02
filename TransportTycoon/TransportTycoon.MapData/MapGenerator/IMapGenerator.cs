namespace TransportTycoon.MapData.MapGenerator
{
    public interface IMapGenerator
    {
        public Field[,] GenerateMap(MapGenerationContext context);
    }
}
