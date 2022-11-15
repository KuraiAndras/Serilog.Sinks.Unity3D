using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Serilog.Sinks.Unity3D
{
    public sealed class Unity3DLogEventSink : ILogEventSink
    {
        public const string UNITY_CONTEXT_KEY = "%UNITY_ID%";

        static internal readonly Dictionary<int, UnityEngine.Object> _objectstoLog = new Dictionary<int, UnityEngine.Object>();

        private readonly ITextFormatter _formatter;
        private readonly UnityEngine.ILogger _logger;

        public Unity3DLogEventSink(ITextFormatter formatter, UnityEngine.ILogger logger = null)
        {
            _formatter = formatter;
            _logger = Debug.unityLogger;
        }

        public void Emit(LogEvent logEvent)
        {
            using (var buffer = new StringWriter())
            {
                _formatter.Format(logEvent, buffer);

                var level = logEvent.Level switch
                {
                    LogEventLevel.Verbose
                    or LogEventLevel.Debug
                    or LogEventLevel.Information => LogType.Log,

                    LogEventLevel.Warning => LogType.Warning,

                    LogEventLevel.Error or LogEventLevel.Fatal => LogType.Error,

                    _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level), "Unknown log level")
                };

                if (TryGetContext(logEvent, out var context))
                {
                    _logger.Log(level, "", buffer.ToString().Trim(), context);
                }
                else
                {
                    _logger.Log(level, buffer.ToString().Trim());
                }
            }
        }

        private static bool TryGetContext(LogEvent logEvent, out UnityEngine.Object unityContext)
        {
            unityContext = null;
#if UNITY_EDITOR 
            if (logEvent.Properties.TryGetValue(UNITY_CONTEXT_KEY, out var propertyValue)
                && propertyValue is ScalarValue scalarValue
                && scalarValue.Value is int id
                && _objectstoLog.TryGetValue(id, out var unityObj))
            {
                unityContext = unityObj;
                _objectstoLog.Remove(id);
                return true;
            }
#endif
            return false;
        }
    }
}