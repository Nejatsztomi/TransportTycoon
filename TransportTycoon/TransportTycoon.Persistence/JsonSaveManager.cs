using System.Text.Json;
using System.Text.Json.Serialization;
using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.MapData.MapGenerator.TerrainGeneration;

namespace TransportTycoon.Persistence
{
    /// <summary>
    /// A <see cref="IBiome"/> object to JSON conververt (works also backwards).
    /// </summary>
    public class BiomeJsonConverter : JsonConverter<IBiome>
    {
        public override bool HandleNull => true;

        public override IBiome Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString() ?? throw new JsonException();
            return Biomes.GetById(value);
        }

        public override void Write(Utf8JsonWriter writer, IBiome value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }

    /// <summary>
    /// A <see cref="IWaterBiome"/> object to JSON conververt (works also backwards).
    /// </summary>
    public class WaterBiomeJsonConverter : JsonConverter<IWaterBiome>
    {
        public override bool HandleNull => true;

        public override IWaterBiome Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString() ?? throw new JsonException();
            return WaterBiomes.GetById(value);
        }

        public override void Write(Utf8JsonWriter writer, IWaterBiome value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }


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
                new JsonStringEnumConverter(),
                new BiomeJsonConverter(),
                new WaterBiomeJsonConverter(),
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
