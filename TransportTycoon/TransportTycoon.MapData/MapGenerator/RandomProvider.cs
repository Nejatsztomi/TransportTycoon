namespace TransportTycoon.MapData.MapGenerator
{
    public class RandomProvider : IRandomProvider
    {
        #region Public methods
        public IRandom GetRandom(int baseSeed, GenerationDomain domain)
        {
            int domainOffset = (int)domain;

            // Needed not to throw OverflowException, since this is a hash
            unchecked
            {
                int combinedSeed = (baseSeed * 397) ^ domainOffset;
                return new SystemRandomWrapper(combinedSeed);
            }
        }
        #endregion
    }
}
