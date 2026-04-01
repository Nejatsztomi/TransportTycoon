namespace TransportTycoon.MapData.MapGenerator.NoiseGenerator
{
    public interface INoiseGenerator
    {
        public float[,] GenerateNoise(float noiseScale);
    }
}
