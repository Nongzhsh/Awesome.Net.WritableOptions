using Microsoft.Extensions.Logging;

namespace Awesome.Net.WritableOptions.ConsoleTestApp.Models
{
    public class AppSettings
    {
        public string ConsoleTitle { get; set; } = "ConsoleTestApp";

        public LogLevel MinLevel { get; set; } = LogLevel.Trace;
    }
}