namespace TransportTycoon.MapData.MapGenerator
{
    public enum GenerationDomain
    {
        Terrain = 1,
        Forests = 2,
        Rivers = 3,
        Cities = 4,
        Structures = 5
    }

    public interface IRandomProvider
    {
        IRandom GetRandom(int seed, GenerationDomain generationDomain);
    }
}
