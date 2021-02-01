using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Awesome.Net.WritableOptions.Extensions
{
    public static class JsonFileHelper
    {
        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, Action<T> updateAction = null)
            where T : class, new()
        {
            var updatedValue = TryGet<T>(jsonFilePath, sectionName, out var value) ? value : new T();

            updateAction?.Invoke(updatedValue);

            AddOrUpdateSection(jsonFilePath, sectionName, updatedValue);
        }

        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, T value)
        {
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
                bool isWritten = false;
                var optionsElement = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(value));
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

        public static bool TryGet<T>(string jsonFilePath, string sectionName, out T value)
        {
            value = default;
            if(File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllBytes(jsonFilePath);

                using(var jsonDocument = JsonDocument.Parse(jsonContent))
                {
                    if(jsonDocument.RootElement.TryGetProperty(sectionName, out var sectionValue))
                    {
                        value = JsonSerializer.Deserialize<T>(sectionValue.ToString());
                        return true;
                    }
                }
            }
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
