namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public static class StructureGeneratorFactory
    {
        public static IStructureGenerator Create(ICityGenerator cityGenerator) => new StructureGenerator(cityGenerator);
    }

    internal class StructureGenerator : IStructureGenerator
    {
        #region Properties
        private ICityGenerator CityGenerator { get; }
        #endregion

        #region Constructors
        public StructureGenerator(ICityGenerator cityGenerator)
        {
            CityGenerator = cityGenerator;
        }
        #endregion

        #region Public methods
        public SavedEntity? TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, string type, int x, int y, int radius, MapGenerationContext context)
        {
            throw new NotImplementedException();
        }

        public SavedEntity ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, string type, int x, int y, int radius, MapGenerationContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
