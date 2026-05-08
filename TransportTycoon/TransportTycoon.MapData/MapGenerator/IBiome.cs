namespace TransportTycoon.MapData.MapGenerator
{
    /// <summary>
    /// An interface representing a biome, which defines the characteristics of a specific type of terrain or environment.
    /// </summary>
    public interface IBiome
    {
        #region Properties
        /// <summary>
        /// The name or ID of the biome, used for identification and retrieval purposes.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets how much of the terrain should be covered by water.
        /// A value between 0 and 1, where 0 means no water and 1 means the entire terrain is covered by water.
        /// Must be less than or equal to <see cref="PlainRange"/>.
        /// </summary>
        public float WaterRange { get; }

        /// <summary>
        /// Gets how much of the terrain should be covered by plain.
        /// A value between 0 and 1, where 0 means no plain and 1 means the entire terrain is covered by plain.
        /// Must be greater than or equal to <see cref="WaterRange"/> and less than or equal to <see cref="HillRange"/>.
        /// </summary>
        public float PlainRange { get; }

        /// <summary>
        /// Gets how much of the terrain should be covered by hills.
        /// A value between 0 and 1, where 0 means no hills and 1 means the entire terrain is covered by hills.
        /// Must be greater than or equal to <see cref="PlainRange"/> and less than or equal to <see cref="MountainRange"/>.
        /// </summary>
        public float HillRange { get; }

        /// <summary>
        /// Gets how much of the terrain should be covered by mountains.
        /// A value between 0 and 1, where 0 means no mountains and 1 means the entire terrain is covered by mountains.
        /// Must be greater than or equal to <see cref="HillRange"/> and less than or equal to <see cref="HighMountainRange"/>.
        /// </summary>
        public float MountainRange { get; }

        /// <summary>
        /// Gets how much of the terrain should be covered by high mountains.
        /// A value between 0 and 1, where 0 means no high mountains and 1 means the entire terrain is covered by high mountains.
        /// Must be greater than or equal to <see cref="MountainRange"/> and less than or equal to 1.
        /// </summary>
        public float HighMountainRange { get; }
        #endregion
    }
}
