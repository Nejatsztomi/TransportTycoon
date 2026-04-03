namespace TransportTycoon.MapData.MapGenerator
{
    public class RandomProvider : IRandomProvider
    {
        #region Public methods
        public IRandom GetRandom(int baseSeed, GenerationDomain domain)
        {
            int domainOffset = (int)domain;

            int combinedSeed = (baseSeed * 397) ^ domainOffset;
            return new SystemRandomWrapper(combinedSeed);
        }
        #endregion
    }
}
