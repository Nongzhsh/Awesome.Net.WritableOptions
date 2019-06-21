using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.WritableOptions.Extensions
{
    public class JsonFileHelper
    {
        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, Action<T> updateAction = null)
            where T : class, new()
        {
            CreateJsonFile(jsonFilePath);

            var jsonContent = File.ReadAllText(jsonFilePath);

            var jObject = JsonConvert.DeserializeObject<JObject>(jsonContent);

            var sectionObject = jObject.TryGetValue(sectionName, out var sectionValue)
                ? JsonConvert.DeserializeObject<T>(sectionValue.ToString())
                : (new T());

            updateAction?.Invoke(sectionObject);

            jObject[sectionName] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));

            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }

        public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, T value)
        {
            CreateJsonFile(jsonFilePath);

            var jsonContent = File.ReadAllText(jsonFilePath);

            var jObject = JsonConvert.DeserializeObject<JObject>(jsonContent);

            if(typeof(T) == typeof(string) || typeof(T).IsValueType)
            {
                jObject[sectionName] = new JValue(value);
            }
            else
            {
                jObject[sectionName] = JObject.Parse(JsonConvert.SerializeObject(value));
            }

            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }

        public static bool TryGet<T>(string jsonFilePath, string sectionName, out T value)
        {
            value = default;
            if(File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllText(jsonFilePath);
                var jObject = JsonConvert.DeserializeObject<JObject>(jsonContent);
                if(jObject.TryGetValue(sectionName, out var sectionValue))
                {
                    if(typeof(T) == typeof(string) || typeof(T).IsValueType)
                    {
                        value = sectionValue.Value<T>();
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject<T>(sectionValue.ToString());
                    }
                    return true;
                }
            }

            return false;
        }

        private static void CreateJsonFile(string jsonFilePath)
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
        }
    }
}
