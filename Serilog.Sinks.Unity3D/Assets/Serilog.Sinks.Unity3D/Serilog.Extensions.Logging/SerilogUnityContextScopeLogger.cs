#nullable enable
using Serilog.Context;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Serilog.Sinks.Unity3D
{
    /// <summary>
    /// A wrapper around <see cref="Serilog.Extensions.Logging.SerilogLogger"/>.
    /// Keeps track of <see cref="UnityEngine.Object"/>s used as scope items.
    /// </summary>
    public class SerilogUnityContextScopeLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private AsyncLocal<List<UnityContextScope>> _unityContextScopeStack = new AsyncLocal<List<UnityContextScope>>();

        public SerilogUnityContextScopeLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_unityContextScopeStack.Value?.Count > 0)
            {
                var unityContext = _unityContextScopeStack.Value[_unityContextScopeStack.Value.Count - 1].UnityContext;
                if (ReferenceEquals(unityContext, null) == false)
                {
                    LogContext.PushProperty(UnityObjectEnricher.UnityContextKey, unityContext, destructureObjects: true);
                    LogContext.PushProperty(UnityTagEnricher.UnityTagKey, unityContext.ToString());
                }
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
                AddUnityContextScope(unityContextScope);
                return unityContextScope;
            }

            return scopeDisposable;
        }

        private void AddUnityContextScope(UnityContextScope unityContextScope)
        {
            _unityContextScopeStack.Value ??= new List<UnityContextScope>(1);
            _unityContextScopeStack.Value.Add(unityContextScope);
        }

        private void RemoveUnityContextScope(UnityContextScope unityContextScope)
        {
            _unityContextScopeStack.Value?.Remove(unityContextScope);
        }

        private class UnityContextScope : IDisposable
        {
            private readonly SerilogUnityContextScopeLogger _logger;
            private readonly IDisposable? _chainedDisposable;

            private readonly WeakReference<UnityEngine.Object> _unityContextReference;

            public UnityEngine.Object? UnityContext
            {
                get
                {
                    if (_unityContextReference.TryGetTarget(out var result) == false)
                        return null;

                    return result;
                }
            }

            public UnityContextScope(SerilogUnityContextScopeLogger logger, IDisposable? chainedDisposable, UnityEngine.Object unityContext)
            {
                _logger = logger;
                _chainedDisposable = chainedDisposable;
                _unityContextReference = new WeakReference<UnityEngine.Object>(unityContext);
            }

            public void Dispose()
            {
                _logger.RemoveUnityContextScope(this);
                _chainedDisposable?.Dispose();
            }
        }
    }
}