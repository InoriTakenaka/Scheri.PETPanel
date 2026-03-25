using NLog;
using NLog.Config;
using NLog.Targets;
using Scheri.PETPanel.Interfaces;
using System;
using System.IO;

namespace Scheri.PETPanel.Desktop;

public class NLogDesktopConfig : IPlatformLogConfig
{
    public LoggingConfiguration GetLoggingConfiguration()
    {
        var config = new LoggingConfiguration();
        var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        var fileTarget = new FileTarget("file") {
            FileName = Path.Combine(logFolder, "${shortdate}", "${logger}.txt"),
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}",
            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveDays = 30,
            CreateDirs = true
        };
        config.AddTarget(fileTarget);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);

        var consoleTarget = new ColoredConsoleTarget("console") {
            Layout = "${time} ${level:uppercase=true} ${message}"
        };
        config.AddTarget(consoleTarget);
        config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);

        return config;
    }
}
