using Awesome.Net.WritableOptions.ConsoleTestApp.Models;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.WritableOptions.ConsoleTestApp.Services
{
    public class TestService : ITestService
    {
        private readonly ILogger<TestService> _logger;
        private readonly IWritableOptions<AppSettings> _options;

        public TestService(ILogger<TestService> logger,
            IWritableOptions<AppSettings> options)
        {
            _logger = logger;
            _options = options;
        }

        public void Run()
        {
            _logger.LogInformation($"Default LogLevel: {_options.Value.MinLevel}");

            _options.Update(opt =>
            {
                switch (opt.MinLevel)
                {
                    case LogLevel.Debug:
                        opt.MinLevel = LogLevel.Trace;
                        break;
                    case LogLevel.Trace:
                        opt.MinLevel = LogLevel.Debug;
                        break;
                }
            });

            _logger.LogWarning($"Updated LogLevel: {_options.Value.MinLevel}");
        }
    }
}