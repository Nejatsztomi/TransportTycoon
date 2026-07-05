namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An enum representing the different phases of map generation.
    /// Each phase corresponds to a specific step in the map generation process, allowing for a structured and organized approach to generating various aspects of the map, such as terrain, water, forests, and structures.
    /// </summary>
    public enum GenerationPhase
    {
        Noise = 0,
        BaseTerrain = 10,
        WaterLayer = 20,
        Forest = 30,
        Structures = 40,
    }

    /// <summary>
    /// An interface for map plugin generators, which are responsible for generating specific aspects of the map during the map generation process.
    /// Each plugin generator is associated with a specific generation phase, allowing for a modular and extensible approach to map generation.
    /// Implementations of this interface can be used to create various types of map features, such as terrain, water, forests, and structures, by implementing the appropriate generation logic for each phase.
    /// </summary>
    public interface IMapPluginGenerator
    {
        #region Public properties
        /// <summary>
        /// A property that indicates the generation phase during which this plugin generator should be executed. This allows the map generation system to determine the order of execution for different plugin generators based on their associated phases, ensuring that the map is generated in a logical and coherent manner.
        /// </summary>
        public GenerationPhase Phase { get; }
        #endregion
    }
}
