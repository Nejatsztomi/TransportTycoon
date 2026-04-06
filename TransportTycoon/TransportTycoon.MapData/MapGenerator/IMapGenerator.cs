namespace TransportTycoon.MapData.MapGenerator
{
    public interface IMapGenerator
    {
        public IField[,] GenerateMap(MapGenerationContext context);
    }
}
