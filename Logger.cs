using System;
using System.IO;
using System.Linq;

/*
Author: *Alec Mills*
Revised: *10/3/19*
*/
namespace SimpleLogger
{
    /// <summary>
    /// Logs strings to a given filepath. Attempts to log its own errors.
    /// </summary>
    public static class Logger
    {
        //default metalog path
        private const string metaLogPath = "meta_log";
        //default date format emphasizes time of day over date
        private const string defaultFormat = "HH:mm:ss MM-dd-yy";

        /// <summary>
        /// Attempts to append the specified `logEntry` string to file located at relative path `./filePath`
        /// </summary>
        /// <returns>Reports success or failure</returns>
        public static bool Log(string filePath, string logEntry)
        {
            if (logEntry == null || !VerifyUriStrings(filePath))
                return false;

            try
            {
                File.AppendAllText(filePath,
                $"{DateTime.Now.ToString(defaultFormat)}: {logEntry}\n");

                return true;
            }
            catch (Exception e)
            {
                MetaLog(new LogMessage(logEntry, defaultFormat, e));
                return false;
            }
        }

        /// <summary>
        /// Reports whether all elements of `paths` are non-null and valid uri strings
        /// </summary>
        private static bool VerifyUriStrings(params string[] paths)
        {
            return 
                paths.All(path => 
                !(path is null) &&
                Uri.IsWellFormedUriString(path, UriKind.Relative));
        }

        /// <summary>
        /// Records information the meta log about a failed log attempt
        /// </summary>
        /// <param name="failedLog">The `LogMessage` that failed to be written to the standard log</param>
        private static void MetaLog(LogMessage failedLog)
        {
            InitMetaLog();
            File.AppendAllText(metaLogPath, "\n" + failedLog.ToString());
        }

        //ensures metalogfile exists
        private static bool InitMetaLog()
        {
            return InitLogFile(metaLogPath);
        }

        private static bool InitLogFile(string path = "log")
        {
            //create the file "./{path}" if it doesn't already exist.
            if (!File.Exists(path))
                using (File.Create(path))
                    return true;//file was created

            return true;//file already existed
        }

        /// <summary>
        /// Represents a given attempt at logging. Exception is null if the attempt was sucessful.
        /// Otherwise, Exception = the Exception that caused this attempt to fail
        /// </summary>
        private struct LogMessage
        {
            public string Message { get; }
            public string Timestamp { get; }
            public Exception Exception { get; }

            public LogMessage(string message, string timeFormatString = defaultFormat, Exception exception = null)
            {
                Exception = exception;
                Message = message;
                Timestamp = DateTime.Now.ToString(timeFormatString);
            }

            /// <summary>
            /// Returns a string representation of `this` including triggered `Exception` information if applicable
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Timestamp}: {Message}" +
                        //add exception info if applicable
                        //$"\n\tException: {(Exception != null ? $"{Exception.GetType()}: {Exception.Message}") : "n/a"}";
                        $"\n\tException: {(Exception != null ? $"{Exception.GetType()}: {Exception.Message}" : "n/a")}";
            }
        }
    }
}
