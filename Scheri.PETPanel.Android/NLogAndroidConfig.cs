using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Scheri.PETPanel.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Scheri.PETPanel.Android
{
    public class NLogAndroidConfig:IPlatformLogConfig
    {
        public LoggingConfiguration GetLoggingConfiguration()
        {
            var context = global::Android.App.Application.Context;
            var baseDir = context.GetExternalFilesDir(null)?.AbsolutePath;
            var logFolder = Path.Combine(baseDir ?? "", "logs");
            if(!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);           

            var config = new LoggingConfiguration();
            var jsonLayout = new JsonLayout();
            jsonLayout.Attributes.Add(new JsonAttribute("TimeOffset", "${date:format=yyyy-MM-ddTHH\\:mm\\:ss.fffK}"));
            jsonLayout.Attributes.Add(new JsonAttribute("Level", "${level:uppercase=true}"));
            jsonLayout.Attributes.Add(new JsonAttribute("Content", "${message}"));
            jsonLayout.Attributes.Add(new JsonAttribute("Source", "${logger}"));
            jsonLayout.Attributes.Add(new JsonAttribute("Exception", "${exception:format=tostring}"));

            var fileTarget = new FileTarget("file") {
                FileName = $"{logFolder}/${{shortdate}}/${{logger}}.txt",
                Layout = jsonLayout,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveDays = 30,
                CreateDirs = true,
                KeepFileOpen = true,
                OpenFileCacheTimeout = 30,
                AutoFlush = true,
                Encoding = Encoding.UTF8
            };

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            return config;
        }
    }
}
