namespace TransportTycoon.Persistence
{
    public interface IPersistence
    {
        public Task SaveGame(string uri, GameSaveData data);

        public Task<GameSaveData?> LoadGame(string uri);
    }
}
