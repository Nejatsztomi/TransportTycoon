using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An interface for generating the map, which includes both terrain and structures. This interface serves as a high-level contract for map generation, allowing for the creation of various types of maps with different generation algorithms and strategies.
    /// </summary>
    public interface IMapGenerator
    {
        #region Public methods
        /// <summary>
        /// Generates a new map layout and a list of building entities based on the specified generation context.
        /// </summary>
        /// <param name="context">The context containing parameters and settings that influence map generation. Cannot be null.</param>
        /// <returns>A tuple containing a two-dimensional array of fields representing the generated map, and a list of building
        /// entities placed on the map. The array and list may be empty if no fields or buildings are generated.</returns>
        public (IField[,], List<BuildingEntity>) GenerateMap(MapGenerationContext context);
        #endregion
    }
}
