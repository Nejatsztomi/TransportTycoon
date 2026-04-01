namespace TransportTycoon.MapData.MapGenerator.NoiseGeneration
{
    public interface INoiseGenerator
    {
        public float[,] GenerateNoise(float noiseScale);
    }
}
