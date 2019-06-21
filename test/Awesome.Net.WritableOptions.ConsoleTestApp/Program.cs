using System;
using System.IO;
using Awesome.Net.WritableOptions.ConsoleTestApp.Models;
using Awesome.Net.WritableOptions.ConsoleTestApp.Services;
using Awesome.Net.WritableOptions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Awesome.Net.WritableOptions.ConsoleTestApp
{
    public class Program
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

        static void Main()
        {
            var config = BuildConfiguration();
            var services = new ServiceCollection();
            ConfigureServices(services, config);

            var serviceProvider = services.BuildServiceProvider();

            var app = serviceProvider.GetService<App>();
            app.Run();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            const string appConfigJsonName = "appsettings.json";

            var appConfigJsonPath = Path.Combine(Directory.GetCurrentDirectory(), appConfigJsonName);

            if(!File.Exists(appConfigJsonPath))
            {
                JsonFileHelper.AddOrUpdateSection(appConfigJsonPath, nameof(AppSettings), new AppSettings());
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appConfigJsonName)
                .AddJsonFile("Resources/appsettings.custom.json", true)
                .AddEnvironmentVariables();

            if(!string.IsNullOrEmpty(EnvironmentName))
            {
                builder.AddJsonFile($"appsettings.{EnvironmentName}.json", true);
            }

            if(IsDevelopment())
            {
                builder.AddUserSecrets<Program>();
            }
            return builder.Build();
        }

        private static bool IsDevelopment()
        {
            if(string.IsNullOrEmpty(EnvironmentName))
            {
                return true;
            }

            var isDevelopment = EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);

            return isDevelopment;
        }

        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot config)
        {
            services.AddSingleton(config);

            ConfigureLogger(services, config);

            services.AddOptions();

            services.ConfigureWritableOptions<AppSettings>(config.GetSection(nameof(AppSettings)), "Resources/appsettings.custom.json");

            ConfigureConsole(config);

            services.AddTransient<ITestService, TestService>();
            services.AddTransient<App>();
        }

        private static void ConfigureLogger(IServiceCollection services, IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile("Logs/{Date}.log")
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging(configure => configure.AddSerilog());

            services.Configure<LoggerFilterOptions>(options =>
            {
                options.MinLevel = config.GetSection("AppSettings:MinLevel")
                    .Get<LogLevel>();
            });
        }

        private static void ConfigureConsole(IConfiguration config)
        {
            Console.Title = config.GetSection("AppSettings:ConsoleTitle").Get<string>();
        }
    }
}