#nullable enable
using Serilog.Context;
using System.Collections.Generic;
using System;

namespace Serilog.Sinks.Unity3D
{
    /// <summary>
    /// A wrapper around <see cref="Serilog.Extensions.Logging.SerilogLogger"/>.
    /// Keeps track of <see cref="UnityEngine.Object"/>s used as scope items.
    /// </summary>
    public class SerilogUnityContextScopeLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        private Stack<UnityContextScope> _unityContextStack = new Stack<UnityContextScope>();

        public SerilogUnityContextScopeLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_unityContextStack.TryPeek(out var unityContextScope))
            {
                LogContext.PushProperty(UnityObjectEnricher.UnityContextKey, unityContextScope.UnityContext, true);
            }

            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            var scopeDisposable = _logger.BeginScope(state);

            if (state is UnityEngine.Object unityContext)
            {
                var unityContextScope = new UnityContextScope(this, scopeDisposable, unityContext);
                _unityContextStack.Push(unityContextScope);
                return unityContextScope;
            }

            return scopeDisposable;
        }

        private class UnityContextScope : IDisposable
        {
            private readonly SerilogUnityContextScopeLogger _logger;
            private readonly IDisposable? _chainedDisposable;
            public UnityEngine.Object UnityContext { get; private set; }

            public UnityContextScope(SerilogUnityContextScopeLogger logger, IDisposable? chainedDisposable, UnityEngine.Object unityContext)
            {
                _logger = logger;
                _chainedDisposable = chainedDisposable;
                UnityContext = unityContext;
            }

            public void Dispose()
            {
                _logger._unityContextStack.Pop();
                _chainedDisposable?.Dispose();
            }
        }
    }
}