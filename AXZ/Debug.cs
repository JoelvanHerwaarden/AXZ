using System;
using System.Diagnostics;
using System.IO;

namespace AXZ
{
    /// <summary>
    /// Defines the severity level of a log message.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    /// <summary>
    /// Provides logging functionality for the AXZ application.
    /// Supports logging messages with different severity levels.
    /// </summary>
    public class Debug
    {
        /// <summary>
        /// Lazily initializes the log file path.
        /// </summary>
        private static readonly Lazy<string> LazyLogPath = new Lazy<string>(() =>
        {
            DateTime dt = DateTime.Now;
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                $"AXZ\\Logs\\AXZ_{dt:yyyyMMdd_HHmmssfff}.log");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Create(path).Close();
            return path;
        });

        /// <summary>
        /// Gets the path to the log file.
        /// </summary>
        private static string LogPath => LazyLogPath.Value;

        /// <summary>
        /// Logs a message with a specified severity level and optional function name.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The severity level of the log message.</param>
        /// <param name="functionName">The name of the function where the log is generated (optional).</param>
        public static void Log(string message, LogLevel level = LogLevel.Info, string functionName = null)
        {
            string timestamp = DateTime.Now.ToString("G");
            string levelTag = $"[{level.ToString().ToUpper()}]";
            string msg = functionName == null
                ? $"{timestamp} {levelTag} : {message}"
                : $"{timestamp} {levelTag} : {functionName} : {message}";

            using (StreamWriter sw = File.AppendText(LogPath))
            {
                sw.WriteLine(msg);
            }
        }

        /// <summary>
        /// Opens the log file using the default associated application.
        /// </summary>
        public static void OpenLog()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = LogPath,
                UseShellExecute = true
            });
        }
    }
}
