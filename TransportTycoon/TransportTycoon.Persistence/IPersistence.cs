namespace TransportTycoon.Persistence
{
    /// <summary>
    /// Defines methods for saving and loading game state data to and from a persistent storage location.
    /// </summary>
    /// <remarks>Implementations of this interface provide mechanisms to persist and retrieve game save data,
    /// such as to files, databases, or cloud storage. Thread safety and storage location details depend on the specific
    /// implementation.</remarks>
    public interface IPersistence
    {
        #region Public async methods
        /// <summary>
        /// Asynchronously saves the specified game data to the given URI.
        /// </summary>
        /// <param name="uri">The destination URI where the game data will be saved. This must be a valid and writable location.</param>
        /// <param name="data">The game data to save. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
        public Task SaveGame(string uri, GameSaveData data);

        /// <summary>
        /// Asynchronously loads game save data from the specified URI.
        /// </summary>
        /// <param name="uri">The URI identifying the location of the game save data to load. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>A <see cref="Task{GameSaveData?}"/> that represents the asynchronous load operation. The task result contains the loaded game save data,
        /// or <see langword="null"/> if no data is found at the specified URI.</returns>
        public Task<GameSaveData?> LoadGame(string uri);
        #endregion
    }
}
