using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransportTycoon.Persistence
{

    public static class JsonSaveManagerFactory
    {
        public static IPersistence Get() => new JsonSaveManager();
    }

    internal class JsonSaveManager : IPersistence
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            IndentSize = 4,
            Converters =
            {
                new JsonStringEnumConverter()
            },
        };

        public async Task<GameSaveData?> LoadGame(string uri)
        {
            if (!File.Exists(uri))
            {
                throw new FileNotFoundException();
            }

            await using FileStream fs = File.OpenRead(uri);
            return await JsonSerializer.DeserializeAsync<GameSaveData>(fs, _options);
        }

        public async Task SaveGame(string uri, GameSaveData data)
        {
            await using FileStream fs = File.Create(uri);
            await JsonSerializer.SerializeAsync<GameSaveData>(fs, data, _options);
        }
    }
}
