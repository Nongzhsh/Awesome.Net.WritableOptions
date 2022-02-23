using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Awesome.Net.WritableOptions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritableOptions<T>(
            this IServiceCollection services,
            IConfigurationRoot configuration,
            string sectionName,
            string subPathOfFile = "appsettings.json",
            Func<JsonSerializerOptions> defaultSerializerOptions = null) where T : class, new()
        {
            services.Configure<T>(configuration.GetSection(sectionName));
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                string jsonFilePath;

#if NETSTANDARD2_1
                var environment = provider.GetService<Microsoft.Extensions.Hosting.IHostEnvironment>();
#else
                var environment = provider.GetService<IHostingEnvironment>();
#endif
                if(environment != null)
                {
                    var fileProvider = environment.ContentRootFileProvider;
                    var fileInfo = fileProvider.GetFileInfo(subPathOfFile);
                    jsonFilePath = fileInfo.PhysicalPath;
                }
                else
                {
                    jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subPathOfFile);
                }

                var options = provider.GetService<IOptionsMonitor<T>>();

                return new WritableOptions<T>(jsonFilePath, sectionName, options, configuration, defaultSerializerOptions);
            });
        }
    }
}