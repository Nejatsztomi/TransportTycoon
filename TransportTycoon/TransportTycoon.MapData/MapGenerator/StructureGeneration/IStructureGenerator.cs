using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface IStructureGenerator
    {
        /// <summary>
        /// Tries to place a structure of the given type on the map.
        /// It checks if the placement is valid (e.g., not on water, not on another structure) and returns the placed entity if successful.
        /// If it cannot place the structure, it returns null.
        /// If x, y, and radius are provided, it tries to place the structure within the specified radius of the given coordinates.
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="waterMap"></param>
        /// <param name="structureMap"></param>
        /// <param name="buildingEntity">What structure to place</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY);

        /// <summary>
        /// Forces the placement of a structure of the given type on the map.
        /// If x, y, and radius are provided, it tries to place the structure within the specified radius of the given coordinates.
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="waterMap"></param>
        /// <param name="structureMap"></param>
        /// <param name="buildingEntity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, BuildingEntity buildingEntity, MapGenerationContext context, int centerX, int centerY);
    }
}
