using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Neo
{
    public enum LogLevel
    {
        Fatal,
        Error,
        Warning,
        Debug
    }

    interface ILogSink
    {
        void AddMessage(LogLevel logLevel, string title, string message);
    }

    class ConsoleLogSink : ILogSink
    {
        private static void SetLevelColor(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        public void AddMessage(LogLevel logLevel, string title, string message)
        {
            lock(this)
            {
                SetLevelColor(logLevel);
                Console.Write(title);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            }
        }
    }

    class FileLogSink : ILogSink
    {
        public FileLogSink()
        {
            if (Directory.Exists("Logs") == false)
            {
	            Directory.CreateDirectory("Logs");
            }

	        File.Create($"Log{Path.DirectorySeparatorChar}Log.txt").Close();
        }

        public void AddMessage(LogLevel level, string title, string message)
        {
            lock (this)
            {
                File.AppendAllText($"Log{Path.DirectorySeparatorChar}Log.txt", $"{title}{message}\n");
            }
        }
    }

    static class Log
    {
        private static readonly List<ILogSink> Sinks = new List<ILogSink>();
        private const LogLevel LogLevel = Neo.LogLevel.Debug;

        static Log()
        {
            Sinks.Add(new ConsoleLogSink());
            Sinks.Add(new FileLogSink());
        }

        private static string GetTitle(string fileName, int line)
        {
            return $"{DateTime.Now:HH:mm:ss} {Path.GetFileName(fileName)}:{line}";
        }

        private static void AddMessage(LogLevel level, string title, string message)
        {
            if ((int) level > (int) LogLevel)
            {
	            return;
            }

	        lock(Sinks)
            {
                foreach (var sink in Sinks)
                {
	                sink.AddMessage(level, title, message);
                }
            }
        }

        public static void AddSink(ILogSink sink)
        {
            lock (Sinks)
            {
	            Sinks.Add(sink);
            }
        }

        public static void RemoveSink(ILogSink sink)
        {
            lock (Sinks)
            {
	            Sinks.Remove(sink);
            }
        }

        public static void Debug(object message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Debug, GetTitle(fileName, line) + " - Debug: ", message?.ToString() ?? "null");
        }

        public static void Warning(object message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Warning, GetTitle(fileName, line) + " - Warning: ", message?.ToString() ?? "null");
        }

        public static void Error(object message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Error, GetTitle(fileName, line) + " - ERROR: ", message?.ToString() ?? "null");
        }

        public static void Fatal(object message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Fatal, GetTitle(fileName, line) + " - FATAL: ", message?.ToString() ?? "null");
        }
    }
}
