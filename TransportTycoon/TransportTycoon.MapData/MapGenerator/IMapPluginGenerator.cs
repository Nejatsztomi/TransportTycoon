namespace TransportTycoon.MapData.MapGenerator
{
    public enum GenerationPhase
    {
        Noise = 0,
        BaseTerrain = 10,
        WaterLayer = 20,
        Forest = 30,
        Structures = 40,
    }
    public interface IMapPluginGenerator
    {
        GenerationPhase Phase { get; }
    }
}
