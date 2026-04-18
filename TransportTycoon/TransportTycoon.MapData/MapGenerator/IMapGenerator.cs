using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator
{
    public interface IMapGenerator
    {
        public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context);
    }
}
