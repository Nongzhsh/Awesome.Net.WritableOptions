using System;
using System.IO;
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
            IConfigurationSection section,
            string subPathOfFile = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                string jsonFilePath;

                var environment = provider.GetService<IHostingEnvironment>();
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
                var configuration = provider.GetService<IConfigurationRoot>();

                return new WritableOptions<T>(jsonFilePath, section.Key, options, configuration);
            });
        }
    }
}