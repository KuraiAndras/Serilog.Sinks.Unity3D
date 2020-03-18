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
        private readonly UnityMainThreadDispatcher _dispatcher;

        public Unity3DLogEventSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
            _dispatcher = UnityMainThreadDispatcherExtensions.Instance;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            _dispatcher.Invoke(() => Debug.Log(message));
        }
    }
}