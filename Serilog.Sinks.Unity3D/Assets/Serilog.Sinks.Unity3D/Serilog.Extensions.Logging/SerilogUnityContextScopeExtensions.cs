#nullable enable
using System;

namespace Serilog.Sinks.Unity3D
{
    public static class SerilogUnityContextScopeExtensions
    {
        /// <summary>
        /// <para>
        /// Enables <see cref="Microsoft.Extensions.Logging.ILogger.BeginScope{TState}(TState)"/> to play nicely with <see cref="UnityEngine.Object"/>.
        /// This way you can use e.g. <code>log.BeginScope(gameObject)</code> and the log entry will be clickable in Unity's console window.
        /// </para>
        /// <para>
        /// To be used in conjunction with <see cref="SerilogUnityContextScopeExtensions.AddSerilogWithUnityObjectScope(Microsoft.Extensions.Logging.ILoggerFactory, ILogger?, bool)"/>
        /// when you set up your factory:
        /// <code>
        /// new Microsoft.Extensions.Logging.LoggerFactory().AddSerilogWithUnityObjectScope(logger, dispose: true)
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <returns></returns>
        public static LoggerConfiguration EnableUnityObjectScope(this LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration
                .Destructure.With<UnityObjectDestructuringPolicy>();
        }

        /// <summary>
        /// <para>
        /// Add Serilog to the logging pipeline and enable <see cref="UnityEngine.Object"/> to be used with <see cref="Microsoft.Extensions.Logging.ILogger.BeginScope{TState}(TState)"/>
        /// </para>
        /// </summary>
        /// <param name="factory">The logger factory to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Serilog.Log"/> will be used.</param>
        /// <param name="dispose">When true, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is true, the <see cref="Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Log"/> class instead.</param>
        /// <returns>Reference to the supplied <paramref name="factory"/>.</returns>
        public static Microsoft.Extensions.Logging.ILoggerFactory AddSerilogWithUnityObjectScope(
            this Microsoft.Extensions.Logging.ILoggerFactory factory,
            Serilog.ILogger? logger = null,
            bool dispose = false)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            factory.AddProvider(new SerilogUnityContextScopeLoggerProvider(logger, dispose));

            return factory;
        }
    }
}
