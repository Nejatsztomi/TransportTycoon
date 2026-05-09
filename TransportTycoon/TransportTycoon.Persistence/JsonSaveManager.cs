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
        #region Overrides
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
        #endregion
    }

    /// <summary>
    /// A <see cref="IWaterBiome"/> object to JSON conververt (works also backwards).
    /// </summary>
    public class WaterBiomeJsonConverter : JsonConverter<IWaterBiome>
    {
        #region Overrides
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
        #endregion
    }

    /// <summary>
    /// A factory class for creating instances of <see cref="IPersistence"/> that utilize JSON serialization for saving and loading game data.
    /// This class provides a simple interface for obtaining a JSON-based persistence manager, abstracting away the details of the implementation and allowing for easy integration into the game's save and load functionality.
    /// </summary>
    public static class JsonSaveManagerFactory
    {
        public static IPersistence Get() => new JsonSaveManager();
    }

    internal class JsonSaveManager : IPersistence
    {
        #region Private fields
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
        #endregion

        #region Public async methods
        /// <summary>
        /// Asynchronously loads game save data from the specified file using JSON serialization.
        /// </summary>
        /// <param name="uri">The path to the file containing the game save data. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>A <see cref="Task{GameSaveData?}"/> that represents the asynchronous load operation. The task result contains the deserialized game save
        /// data, or <see langword="null"/> if the file does not contain valid data.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file specified by uri does not exist.</exception>
        public async Task<GameSaveData?> LoadGame(string uri)
        {
            if (!File.Exists(uri))
            {
                throw new FileNotFoundException();
            }

            await using FileStream fs = File.OpenRead(uri);
            return await JsonSerializer.DeserializeAsync<GameSaveData>(fs, _options);
        }

        /// <summary>
        /// Asynchronously saves the specified game data to a file at the given URI in JSON format.
        /// </summary>
        /// <remarks>If a file already exists at the specified URI, it will be overwritten. The method
        /// uses JSON serialization and writes the data asynchronously to improve performance for large save
        /// files.</remarks>
        /// <param name="uri">The file path or URI where the game data will be saved. Must be a valid path that the application has
        /// permission to write to.</param>
        /// <param name="data">The game data to serialize and save. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
        public async Task SaveGame(string uri, GameSaveData data)
        {
            await using FileStream fs = File.Create(uri);
            await JsonSerializer.SerializeAsync<GameSaveData>(fs, data, _options);
        }
        #endregion
    }
}
