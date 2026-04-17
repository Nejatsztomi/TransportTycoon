using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator
{
    public interface IMapGenerator
    {
        public (Field[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context);
    }
}
