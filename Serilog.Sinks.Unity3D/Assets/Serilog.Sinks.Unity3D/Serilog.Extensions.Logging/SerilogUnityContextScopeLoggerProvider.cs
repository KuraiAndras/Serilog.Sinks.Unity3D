#nullable enable
using Serilog.Extensions.Logging;

namespace Serilog.Sinks.Unity3D
{
    public delegate object UnityObjectTransformerDelegate(UnityEngine.Object obj);

    /// <summary>
    /// A thin wrapper around <see cref="SerilogLoggerProvider"/>.
    /// </summary>
    public class SerilogUnityContextScopeLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        private readonly SerilogLoggerProvider _innerProvider;
        private readonly UnityObjectTransformerDelegate? _unityObjectTransformer;

        public SerilogUnityContextScopeLoggerProvider(ILogger? logger, bool dispose, UnityObjectTransformerDelegate? unityObjectTransformer = null)
        {
            _innerProvider = new SerilogLoggerProvider(logger, dispose);
            _unityObjectTransformer = unityObjectTransformer;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new SerilogUnityContextScopeLogger(_innerProvider.CreateLogger(categoryName), _unityObjectTransformer);
        }

        public void Dispose()
        {
            _innerProvider.Dispose();
        }
    }
}
