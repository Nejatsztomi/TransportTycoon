namespace TransportTycoon.MapData.MapGenerator
{
    public sealed class RandomProvider : IRandomProvider
    {
        #region Public methods
        public IRandom GetRandom(int baseSeed, string pluginId)
        {
            int pluginIdHash = GetDeterministicStringHash(pluginId);

            // Needed not to throw OverflowException, since this is a hash
            unchecked
            {
                int combinedSeed = (baseSeed * 397) ^ pluginIdHash;
                return new SystemRandomWrapper(combinedSeed);
            }
        }
        #endregion

        #region Private methods
        private int GetDeterministicStringHash(string str)
        {
            unchecked
            {
                int hash = (int)2166136261; // FNV offset basis
                foreach (char c in str)
                {
                    hash = (hash ^ c) * 16777619; // FNV prime
                }
                return hash;
            }
        }
        #endregion
    }
}
