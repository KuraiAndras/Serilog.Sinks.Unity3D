using MainThreadDispatcher;
using MainThreadDispatcher.Unity;
using Serilog.Core;
using Serilog.Events;
using System;
using UnityEngine;

namespace Serilog.Sinks.Unity3D
{
    public sealed class Unity3DLogEventSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IMainThreadDispatcher _dispatcher;

        public Unity3DLogEventSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
            _dispatcher = UnityMainThreadDispatcherExtensions.Instance;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            _dispatcher.Invoke(() =>
            {
                switch (logEvent.Level)
                {
                    case LogEventLevel.Verbose:
                    case LogEventLevel.Debug:
                    case LogEventLevel.Information:
                        Debug.Log(message);
                        break;
                    case LogEventLevel.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogEventLevel.Error:
                        Debug.LogError(message);
                        break;
                    case LogEventLevel.Fatal:
                        Debug.LogError(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown log level");
                }
            });
        }
    }
}