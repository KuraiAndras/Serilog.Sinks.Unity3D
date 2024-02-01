#nullable enable
using Serilog.Context;
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
         private static SynchronizationContext? _synchronizationContext;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }
        
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private AsyncLocal<UnityContextScope?>? _unityContextScope;

        private UnityContextScope? CurrentScope
        {
            get => _unityContextScope?.Value;
            set => (_unityContextScope ??= new AsyncLocal<UnityContextScope?>()).Value = value;
        }

        public SerilogUnityContextScopeLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_unityContextScope?.Value != null)
            {
                var unityContext = _unityContextScope.Value.UnityContext;
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
                if (SynchronizationContext.Current != _synchronizationContext)
                {
                    throw new NotSupportedException("BeginScope(UnityEngine.Object) can only be used from the main thread.");
                }

                IDisposable? scopeDisposable = _logger.BeginScope(unityContext);
                return CurrentScope = new UnityContextScope(this, scopeDisposable, unityContext);
            }

            return scopeDisposable;
        }

        private class UnityContextScope : IDisposable
        {
            private readonly SerilogUnityContextScopeLogger _logger;
            private readonly IDisposable? _chainedDisposable;

            private readonly WeakReference<UnityEngine.Object> _unityContextReference;
            private bool _disposed;

            public UnityContextScope? Parent { get; }

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

                Parent = logger.CurrentScope;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                // Just like in Serilog.Extensions.Logging.SerilogLoggerScope.Dispose():
                // In case one of the parent scopes has been disposed out-of-order, don't
                // just blindly reinstate our own parent.
                for (var scan = _logger.CurrentScope; scan != null; scan = scan.Parent)
                {
                    if (ReferenceEquals(scan, this))
                        _logger.CurrentScope = Parent;
                }

                _chainedDisposable?.Dispose();
            }
        }

        public class UnityObjectToStringWrapper
        {
            private UnityEngine.Object _unityContext;
            private string? _toString = null;

            public UnityObjectToStringWrapper(UnityEngine.Object unityContext)
            {
                _unityContext = unityContext;
            }

            public override string ToString()
            {
                if (_toString != null)
                    return _toString;

                _synchronizationContext?.Send(GetObjectName, null);
                return _toString!;
            }

            private void GetObjectName(object state)
            {
                _toString = _unityContext.ToString();
            }
        }

    }
}