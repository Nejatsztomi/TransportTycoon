namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface ICityGenerator
    {
        public SavedEntity GenerateCity(SavedEntity city, MapGenerationContext context);
    }
}
