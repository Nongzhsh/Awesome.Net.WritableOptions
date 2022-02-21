using System;
using System.Text.Json;
using Awesome.Net.WritableOptions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Awesome.Net.WritableOptions
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly string _sectionName;
        private readonly string _jsonFilePath;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly JsonSerializerOptions _defaultSerializerOptions;
        public T Value => _options.CurrentValue;

        public T Get(string name) => _options.Get(name);

        public WritableOptions(
            string jsonFilePath,
            string sectionName,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            Func<JsonSerializerOptions> defaultSerializerOptions = null)
        {
            _jsonFilePath = jsonFilePath;
            _sectionName = sectionName;
            _options = options;
            _configuration = configuration;
            _defaultSerializerOptions = defaultSerializerOptions == null ? JsonFileHelper.DefaultSerializerOptions() : defaultSerializerOptions();
        }

        public void Update(Action<T> updateAction, bool reload = true)
        {
            JsonFileHelper.AddOrUpdateSection(_jsonFilePath, _sectionName, updateAction, _defaultSerializerOptions);

            if(reload)
            {
                _configuration?.Reload();
            }
        }
    }
}