using Microsoft.Extensions.Logging;
using NLog.Conditions;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using LogLevel = NLog.LogLevel;

namespace Scheri.PETPanel.Utils;

public static class MsLogger
{
    private static readonly ILogger Logger;

    private static readonly Dictionary<NLog.LogLevel, Microsoft.Extensions.Logging.LogLevel> LevelMap =
    new() {
        [NLog.LogLevel.Trace] = Microsoft.Extensions.Logging.LogLevel.Trace,
        [NLog.LogLevel.Debug] = Microsoft.Extensions.Logging.LogLevel.Debug,
        [NLog.LogLevel.Info] = Microsoft.Extensions.Logging.LogLevel.Information,
        [NLog.LogLevel.Warn] = Microsoft.Extensions.Logging.LogLevel.Warning,
        [NLog.LogLevel.Error] = Microsoft.Extensions.Logging.LogLevel.Error,
        [NLog.LogLevel.Fatal] = Microsoft.Extensions.Logging.LogLevel.Critical,
    };

    static MsLogger()
    {
        using ILoggerFactory factory = LoggerFactory.Create(
            builder => { builder.AddNLog(CreateNLogConfig()); });
        Logger = factory.CreateLogger("app");
        Logger.LogInformation("Logger Init");
    }

    private static LoggingConfiguration CreateNLogConfig()
    {
        var config = new LoggingConfiguration();
        CreateFileTarget(config);
        CreateConsoleTarget(config);
        return config;
    }

    private static void CreateFileTarget(LoggingConfiguration config)
    {
        var fileTarget = new FileTarget("file") {
            FileName = GetAndroidLogFilePath("${shortdate}/${logger}.log"),
            Layout = @"Time: ${event-context:item=Data:objectpath=TimeOffset:format=yyyy-MM-dd HH:mm:ss.fff}
                       Level: ${event-context:item=Data:objectpath=Level}
                       Source: ${event-context:item=Data:objectpath=Source:whenEmpty=Unknown}
                       Content: ${event-context:item=Data:objectpath=Content}
                       ---------------------------------------------------",

            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveDays = 30,
            CreateDirs = true,
            KeepFileOpen = false,
            Encoding = System.Text.Encoding.UTF8
        };
        config.AddTarget(fileTarget);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
    }

    private static string GetAndroidLogFilePath(string relativePath)
    {
        string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Logs");
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
        return Path.Combine(logDir, relativePath);
    }

    private static void CreateConsoleTarget(LoggingConfiguration config)
    {
        var consoleTarget = new ColoredConsoleTarget("console") {
            Layout =
                @"${date:format=HH\:mm\:ss} ${level:uppercase=true} ${logger} ${message} ${exception:format=ToString}",
            UseDefaultRowHighlightingRules = false,
        };
        consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
            ConditionParser.ParseExpression("level == LogLevel.Debug"), ConsoleOutputColor.DarkGreen,
            ConsoleOutputColor.NoChange));
        consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
            ConditionParser.ParseExpression("level == LogLevel.Error"), ConsoleOutputColor.Red,
            ConsoleOutputColor.NoChange));
        consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
            ConditionParser.ParseExpression("level == LogLevel.Warning"), ConsoleOutputColor.Yellow,
            ConsoleOutputColor.NoChange));
        consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
            ConditionParser.ParseExpression("level == LogLevel.Information"), ConsoleOutputColor.Green,
            ConsoleOutputColor.NoChange));
        consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
            ConditionParser.ParseExpression("level == LogLevel.Trace"), ConsoleOutputColor.DarkGray,
            ConsoleOutputColor.NoChange));
        consoleTarget.EnableAnsiOutput = true;
        config.AddTarget("debugger", consoleTarget);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));
    }

    public static void Log(LogRecord record)
    {
        Logger.Log(LevelMap[record.Level], 0, record, null, (state, ex) => state.ToString());
    }

    public static void Debug(string msg)
    {
        Logger.LogDebug(msg);
    }
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