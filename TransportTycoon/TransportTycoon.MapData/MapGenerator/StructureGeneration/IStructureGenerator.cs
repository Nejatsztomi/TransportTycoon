using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    /// <summary>
    /// An interface for generating structures on the map.
    /// This includes buildings and other non-natural features that add character and functionality to the game world.
    /// Implementations of this interface will define how structures are placed, their types, and their interactions with the environment and other game elements.
    /// </summary>
    public interface IStructureGenerator : IMapPluginGenerator
    {
        /// <summary>
        /// Generates structures within the map using the specified generation context.
        /// </summary>
        /// <param name="context">The context that provides information and services required for map structure generation. Cannot be <see langword="null"/>.</param>
        public List<BuildingEntity> GenerateStructures(MapGenerationContext context);
    }
}
