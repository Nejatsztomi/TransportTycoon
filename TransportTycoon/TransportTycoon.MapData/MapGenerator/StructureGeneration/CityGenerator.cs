namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class CityGeneratorFactory
    {
        public static ICityGenerator Create() => new CityGenerator();
    }

    internal class CityGenerator : ICityGenerator
    {
        #region Constructor
        public CityGenerator() { }
        #endregion

        #region Public methods
        public SavedEntity GenerateCity(SavedEntity city, MapGenerationContext context)
        {
            return city;
        }
        #endregion
    }
}
