namespace TransportTycoon.MapData.MapGenerator
{
    public interface INoiseGenerator
    {
        public float[,] GenerateNoise(float noiseScale);
    }
}
