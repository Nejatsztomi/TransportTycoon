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
        /// <param name="type">What structure to place</param>
        /// <param name="x">If non-negative, it tries to place structure in a certain radius</param>
        /// <param name="y">If non-negative, it tries to place structure in a certain radius</param>
        /// <param name="radius">If positive, it tries to place structure in a certain radius</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SavedEntity? TryPlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, string type, int x, int y, int radius, MapGenerationContext context);

        /// <summary>
        /// Forces the placement of a structure of the given type on the map.
        /// If x, y, and radius are provided, it tries to place the structure within the specified radius of the given coordinates.
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="waterMap"></param>
        /// <param name="structureMap"></param>
        /// <param name="type"></param>
        /// <param name="x">If non-negative, it places structure in a certain radius</param>
        /// <param name="y">If non-negative, it places structure in a certain radius</param>
        /// <param name="radius">If positive, it place structure in a certain radius</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SavedEntity ForcePlace(int[,] heightMap, bool[,] waterMap, bool[,] structureMap, string type, int x, int y, int radius, MapGenerationContext context);
    }
}
