namespace Serilog
{
    public static class ILoggerExtensions
    {
        /// <summary>
        /// Add <see cref="UnityEngine.Object"/> context for the log.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public static ILogger ForContext(this ILogger logger, UnityEngine.Object context) => logger.ForContext(new UnityObjectEnricher(context));
    }
}
