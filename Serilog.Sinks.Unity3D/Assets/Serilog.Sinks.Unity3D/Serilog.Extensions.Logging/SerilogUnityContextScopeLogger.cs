#nullable enable
using Serilog.Context;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

namespace Serilog.Sinks.Unity3D
{
    /// <summary>
    /// A wrapper around <see cref="Serilog.Extensions.Logging.SerilogLogger"/>.
    /// Keeps track of <see cref="UnityEngine.Object"/>s used as scope items.
    /// </summary>
    public class SerilogUnityContextScopeLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private AsyncLocal<UnityContextScope> _unityContextScopeStack = new AsyncLocal<UnityContextScope>();

        public SerilogUnityContextScopeLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_unityContextScopeStack.Value != null)
            {
                var unityContext = _unityContextScopeStack.Value.UnityContext;
                if (ReferenceEquals(unityContext, null) == false)
                {
                    LogWithUnityContext(logLevel, eventId, state, exception, formatter, unityContext);
                    return;
                }
            }

            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        private void LogWithUnityContext<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter,
            UnityEngine.Object unityContext)
        {
            LogContext.PushProperty(UnityObjectEnricher.UnityContextKey, unityContext, destructureObjects: true);
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
                return _unityContextScopeStack.Value = new UnityContextScope(this, scopeDisposable, unityContext);
            }

            return scopeDisposable;
        }

        private void RemoveUnityContextScope(UnityContextScope unityContextScope)
        {
            // In case one of the parent scopes has been disposed out-of-order, don't
            // just blindly reinstate our own parent.
            for (var scan = _unityContextScopeStack.Value; scan != null; scan = scan.Parent)
            {
                if (ReferenceEquals(scan, unityContextScope))
                    _unityContextScopeStack.Value = unityContextScope.Parent;
            }
        }

        private class UnityContextScope : IDisposable
        {
            private readonly SerilogUnityContextScopeLogger _logger;
            private readonly IDisposable? _chainedDisposable;

            private readonly WeakReference<UnityEngine.Object> _unityContextReference;
            private bool _disposed;

            public UnityContextScope Parent { get; }

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

                Parent = logger._unityContextScopeStack.Value;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                _logger.RemoveUnityContextScope(this);
                _chainedDisposable?.Dispose();
            }
        }
    }
}