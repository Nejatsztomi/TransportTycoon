namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    public interface INoiseGenerator : IMapPluginGenerator
    {
        public float GenerateNoise(float x, float y, int seed);
        public float[,] GenerateNoiseMap(int width, int height, int seed);
    }
}
