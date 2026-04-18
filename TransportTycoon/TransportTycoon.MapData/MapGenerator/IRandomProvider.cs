namespace TransportTycoon.MapData.MapGenerator
{
    public interface IRandomProvider
    {
        IRandom GetRandom(int seed, string pluginId);
    }
}
