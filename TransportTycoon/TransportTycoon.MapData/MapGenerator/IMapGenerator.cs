namespace TransportTycoon.Model.MapGenerator
{
    public interface IMapGenerator
    {
        public int[,] GenerateMap(float noiseScale);
    }
}
