using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface ICityGenerator
    {
        public void GenerateCity(BuildingEntity city, MapGenerationContext context);
    }
}
