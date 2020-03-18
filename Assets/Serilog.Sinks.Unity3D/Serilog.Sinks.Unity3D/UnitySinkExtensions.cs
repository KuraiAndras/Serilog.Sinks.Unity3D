using Serilog.Configuration;
using System;

namespace Serilog.Sinks.Unity3D.Serilog.Sinks.Unity3D
{
    public static class UnitySinkExtensions
    {
        public static LoggerConfiguration Unity3D(this LoggerSinkConfiguration loggerSinkConfiguration, IFormatProvider formatProvider = null) =>
            loggerSinkConfiguration.Sink(new Unity3DLogEventSink(formatProvider));
    }
}
