#nullable enable
using Serilog.Extensions.Logging;

namespace Serilog.Sinks.Unity3D
{
    /// <summary>
    /// A thin wrapper around <see cref="SerilogLoggerProvider"/>.
    /// </summary>
    public class SerilogUnityContextScopeLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        private readonly SerilogLoggerProvider _innerProvider;

        public SerilogUnityContextScopeLoggerProvider(ILogger? logger, bool dispose)
        {
            _innerProvider = new SerilogLoggerProvider(logger, dispose);
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new SerilogUnityContextScopeLogger(_innerProvider.CreateLogger(categoryName));
        }

        public void Dispose()
        {
            _innerProvider.Dispose();
        }
    }
}
