namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    public interface INoiseGenerator
    {
        public float GenerateNoise(float x, float y, int seed);
    }
}
