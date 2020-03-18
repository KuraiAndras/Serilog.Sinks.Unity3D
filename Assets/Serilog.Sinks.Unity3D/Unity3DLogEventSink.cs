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

        public Unity3DLogEventSink(IFormatProvider formatProvider) => _formatProvider = formatProvider;

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            UnityMainThreadDispatcherExtensions.Instance.Invoke(() => Debug.Log(message));
        }
    }
}