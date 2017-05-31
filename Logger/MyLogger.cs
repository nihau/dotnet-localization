using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class MyLogger
    {
        private object _lock = new object();
        private const char DefaultSplitSymbol = '=';

        public string FilePath { get; set; }

        public bool TimeLoggingEnabled { get; set; } = true;

        public MyLogger(string filePath)
        {
            FilePath = filePath;
        }

        public void WriteLine(string str)
        {
            if (TimeLoggingEnabled)
                str = DateTime.Now.ToString("[hh:mm:ss] ") + str;

            lock (_lock)
            {
                using (var fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(str);
                }
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }

        public void WriteLineSplit(char c = DefaultSplitSymbol)
        {
            WriteLine(new string(c, 40));
        }

        public void WriteLineSplit(string str, char c = DefaultSplitSymbol)
        {
            WriteLineSplit();
            WriteLine(str);
            WriteLineSplit();
        }

        public void WriteLineSplit(string format, params object[] args)
        {
            WriteLineSplit();
            WriteLine(format, args);
            WriteLineSplit();
        }
    }

    public static class MyLoggerExtensions
    {
        public static void WriteLine(this IEnumerable<MyLogger> loggers, string str)
        {
            foreach (var logger in loggers)
            {
                logger.WriteLine(str);
            }
        }

        public static void WriteLine(this IEnumerable<MyLogger> loggers, string format, params object[] args)
        {
            foreach (var logger in loggers)
            {
                logger.WriteLine(format, args);
            }
        }

        public static void WriteLineSplit(this IEnumerable<MyLogger> loggers)
        {
            foreach (var logger in loggers)
            {
                logger.WriteLineSplit();
            }
        }

        public static void WriteLineSplit(this IEnumerable<MyLogger> loggers, string str)
        {
            foreach (var logger in loggers)
            {
                logger.WriteLineSplit(str);
            }
        }

        public static void WriteLineSplit(this IEnumerable<MyLogger> loggers, string format, params object[] args)
        {
            foreach (var logger in loggers)
            {
                logger.WriteLineSplit(format, args);
            }
        }
    }
}
