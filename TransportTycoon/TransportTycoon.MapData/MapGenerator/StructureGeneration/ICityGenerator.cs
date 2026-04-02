using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface ICityGenerator
    {
        public BuildingEntity GenerateCity(BuildingEntity city, MapGenerationContext context);
    }
}
