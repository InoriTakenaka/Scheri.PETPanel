using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Scheri.PETPanel.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using LogLevel = NLog.LogLevel;

namespace Scheri.PETPanel.Utils;

public static class AppLogger
{
    private static ILogger? _logger;
    private static readonly Lock _lock = new();

    private static ILogger Logger {
        get {
            if (_logger == null)
            {
                lock (_lock)
                {
                    if (_logger == null)
                    {
                        _logger ??= Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                    }
                }
            }
            return _logger;
        }
    }

    private static readonly Dictionary<LogLevel, Microsoft.Extensions.Logging.LogLevel> LevelMap =
    new() {
        [LogLevel.Trace] = Microsoft.Extensions.Logging.LogLevel.Trace,
        [LogLevel.Debug] = Microsoft.Extensions.Logging.LogLevel.Debug,
        [LogLevel.Info] = Microsoft.Extensions.Logging.LogLevel.Information,
        [LogLevel.Warn] = Microsoft.Extensions.Logging.LogLevel.Warning,
        [LogLevel.Error] = Microsoft.Extensions.Logging.LogLevel.Error,
        [LogLevel.Fatal] = Microsoft.Extensions.Logging.LogLevel.Critical,
    };

    /// <summary>
    /// Initializes the logging infrastructure using the specified platform logging configuration.
    /// </summary>
    /// <remarks>This method configures both NLog and Microsoft.Extensions.Logging to use the provided
    /// platform configuration. It should be called once during application startup before any logging is performed.
    /// This method is thread-safe.</remarks>
    /// <param name="platformConfig">The platform-specific logging configuration to use for initializing the logging system. Cannot be null.</param>
    public static void Initialize(IPlatformLogConfig platformConfig)
    {
        lock (_lock)
        {
            var nlogConfig = platformConfig.GetLoggingConfiguration();
            NLog.LogManager.Configuration = nlogConfig;

            using ILoggerFactory factory = LoggerFactory.Create(builder => {
                builder.ClearProviders();
                builder.AddNLog(nlogConfig);
            });

            _logger = factory.CreateLogger("PETPanelApp");
            _logger.LogInformation("Log initialized for Specific Platform.");
        }
    }   

    private static void Log(LogRecord record)
    {
        Logger.Log(LevelMap[record.Level], 0, record, null, (state, ex) => state.ToString());
    }

    public static void Debug(string message, string? source = null) => Log(new LogRecord(DateTime.Now, LogLevel.Debug, message, source));

    public static void Info(string message, string? source = null) => Log(new LogRecord(DateTime.Now, LogLevel.Info, message, source));

    public static void Warn(string message, string? source = null) => Log(new LogRecord(DateTime.Now, LogLevel.Warn, message, source));

    public static void Error(string message, string? source = null) => Log(new LogRecord(DateTime.Now, LogLevel.Error, message, source));
}

public record LogRecord(
    DateTimeOffset TimeOffset,
    LogLevel Level,
    string Content,
    string? Source = null)
{
    public LogRecord(string content, LogLevel level, string? source = null)
        : this(DateTimeOffset.Now, level, content, source)
    {

    }
}