using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface ICityGenerator
    {
        public void GenerateCity(int branchCount, int roadCount, BuildingEntity city, MapGenerationContext context);
    }
}
