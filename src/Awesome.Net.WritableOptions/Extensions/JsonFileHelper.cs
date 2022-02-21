using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awesome.Net.WritableOptions.Extensions
{
    public static class JsonFileHelper
    {
        public static Func<JsonSerializerOptions> DefaultSerializerOptions = new Func<JsonSerializerOptions>(() => {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
        });

        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, Action<T> updateAction = null, JsonSerializerOptions serializerOptions = null)
            where T : class, new()
        {
            if(serializerOptions == null) serializerOptions = DefaultSerializerOptions();
            var updatedValue = TryGet<T>(jsonFilePath, sectionName, out var value, serializerOptions) ? value : new T();

            updateAction?.Invoke(updatedValue);

            AddOrUpdateSection(jsonFilePath, sectionName, updatedValue, serializerOptions);
        }

        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, T value, JsonSerializerOptions serializerOptions = null)
        {
            if(serializerOptions == null) serializerOptions = DefaultSerializerOptions();
            var jsonContent = ReadOrCreateJsonFile(jsonFilePath);

            using(var jsonDocument = JsonDocument.Parse(jsonContent))
            using(var stream = File.OpenWrite(jsonFilePath))
            {
                var writer = new Utf8JsonWriter(stream, new JsonWriterOptions()
                {
                    Indented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                writer.WriteStartObject();
                var isWritten = false;
                var optionsElement = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(value, serializerOptions));
                foreach(var element in jsonDocument.RootElement.EnumerateObject())
                {
                    if(element.Name != sectionName)
                    {
                        element.WriteTo(writer);
                        continue;
                    }
                    writer.WritePropertyName(element.Name);
                    optionsElement.WriteTo(writer);
                    isWritten = true;
                }
                if(!isWritten)
                {
                    writer.WritePropertyName(sectionName);
                    optionsElement.WriteTo(writer);
                }
                writer.WriteEndObject();
                writer.Flush();
                stream.SetLength(stream.Position);
            }
        }

        public static bool TryGet<T>(string jsonFilePath, string sectionName, out T value, JsonSerializerOptions serializerOptions = null)
        {
            if(File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllBytes(jsonFilePath);

                using(var jsonDocument = JsonDocument.Parse(jsonContent))
                {
                    if(jsonDocument.RootElement.TryGetProperty(sectionName, out var sectionValue))
                    {
                        if(serializerOptions == null) serializerOptions = DefaultSerializerOptions();
                        value = JsonSerializer.Deserialize<T>(sectionValue.ToString(), serializerOptions);
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        private static byte[] ReadOrCreateJsonFile(string jsonFilePath)
        {
            if(!File.Exists(jsonFilePath))
            {
                var fileDirectoryPath = Path.GetDirectoryName(jsonFilePath);
                if(!string.IsNullOrEmpty(fileDirectoryPath))
                {
                    Directory.CreateDirectory(fileDirectoryPath);
                }

                File.WriteAllText(jsonFilePath, "{}");
            }
            return File.ReadAllBytes(jsonFilePath);
        }
    }
}
