using Awesome.Net.WritableOptions.ConsoleTestApp.Models;
using Awesome.Net.WritableOptions.ConsoleTestApp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Awesome.Net.WritableOptions.ConsoleTestApp
{
    public class App
    {
        private readonly ITestService _testService;
        private readonly ILogger<App> _logger;
        private readonly AppSettings _config;

        public App(ITestService testService,
            IOptions<AppSettings> config,
            ILogger<App> logger)
        {
            _testService = testService;
            _logger = logger;
            _config = config.Value;
        }

        public void Run()
        {
            _logger.LogInformation($"App({_config.ConsoleTitle}) is running!");
            _testService.Run();
            System.Console.ReadKey();
        }
    }
}