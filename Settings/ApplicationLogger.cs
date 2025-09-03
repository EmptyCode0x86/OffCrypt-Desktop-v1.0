// ====================================================================================================
// File: ApplicationLogger.cs
// OffCryptDesktop - Application Load and Runtime Logging System
// 
// This class provides comprehensive logging functionality for the OffCryptDesktop application:
// - Application load logging
// - Runtime events and errors
// - Encryption operation tracking
// - Debug information for development use
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace OffCrypt
{
    /// <summary>
    /// Centralized logging functionality for the OffCryptDesktop application
    /// </summary>
    public static class ApplicationLogger
    {
        private static readonly object logLock = new object();
        private static readonly List<LogEntry> logEntries = new List<LogEntry>();
        private static readonly StringBuilder memoryLog = new StringBuilder();
        private static string? logFilePath = null;
        private static bool isInitialized = false;
        
        // Application startup tracking
        private static DateTime applicationStartTime;
        private static readonly List<string> initializationSteps = new List<string>();
        
        /// <summary>
        /// Log entry levels
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical,
            Crypto,
            Security
        }

        /// <summary>
        /// Log entry structure
        /// </summary>
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public string? CallerMember { get; set; }
            public string? CallerFile { get; set; }
            public int CallerLine { get; set; }
            public string? Exception { get; set; }
            public TimeSpan? Duration { get; set; }
        }

        /// <summary>
        /// Initializes the logging functionality at application startup
        /// </summary>
        public static void Initialize()
        {
            if (isInitialized) return;
            
            applicationStartTime = DateTime.Now;
            
            try
            {
                // Create log file in temp directory
                var logFileName = $"OffCrypt_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                logFilePath = Path.Combine(Path.GetTempPath(), logFileName);
                
                // Write header
                WriteToFile("════════════════════════════════════════════════");
                WriteToFile($"OffCryptDesktop Application Log");
                WriteToFile($"Started: {applicationStartTime:yyyy-MM-dd HH:mm:ss.fff}");
                WriteToFile($"Process ID: {Process.GetCurrentProcess().Id}");
                WriteToFile($"Log File: {logFilePath}");
                WriteToFile("════════════════════════════════════════════════");
                
                isInitialized = true;
                
                LogInfo("Application", "Logger initialized successfully");
                LogApplicationStart();
            }
            catch (Exception ex)
            {
                // If file logging fails, fall back to memory logging
                Debug.WriteLine($"Logger initialization failed: {ex.Message}");
                isInitialized = true; // Allow memory logging
                LogError("Logger", $"File logging initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs the application startup
        /// </summary>
        private static void LogApplicationStart()
        {
            LogInfo("Application", "═══ APPLICATION STARTUP ═══");
            LogInfo("Environment", $"OS: {Environment.OSVersion}");
            LogInfo("Environment", $"CLR Version: {Environment.Version}");
            LogInfo("Environment", $"Machine Name: {Environment.MachineName}");
            LogInfo("Environment", $"User: {Environment.UserName}");
            LogInfo("Environment", $"Working Directory: {Environment.CurrentDirectory}");
            LogInfo("Memory", $"Initial Memory: {GC.GetTotalMemory(false) / 1024} KB");
        }

        /// <summary>
        /// Logs a step in the application initialization
        /// </summary>
        public static void LogInitializationStep(string stepName)
        {
            lock (logLock)
            {
                initializationSteps.Add($"[{DateTime.Now:HH:mm:ss.fff}] {stepName}");
            }
            LogInfo("Initialization", stepName);
        }

        /// <summary>
        /// Logs the completion of the application load
        /// </summary>
        public static void LogApplicationLoadComplete()
        {
            var loadDuration = DateTime.Now - applicationStartTime;
            
            LogInfo("Application", "═══ APPLICATION LOAD COMPLETE ═══");
            LogInfo("Performance", $"Total load time: {loadDuration.TotalMilliseconds:F0}ms");
            LogInfo("Memory", $"Memory after load: {GC.GetTotalMemory(false) / 1024} KB");
            
            LogInfo("Initialization", "Initialization steps completed:");
            foreach (var step in initializationSteps)
            {
                LogInfo("Initialization", $"  • {step}");
            }
        }

        #region Core Logging Methods

        /// <summary>
        /// Debug level logging
        /// </summary>
        public static void LogDebug(string category, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Debug, category, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Info level logging
        /// </summary>
        public static void LogInfo(string category, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Info, category, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Warning level logging
        /// </summary>
        public static void LogWarning(string category, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Warning, category, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Error level logging
        /// </summary>
        public static void LogError(string category, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Error, category, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Critical error logging
        /// </summary>
        public static void LogCritical(string category, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Critical, category, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Cryptographic operation logging
        /// </summary>
        public static void LogCrypto(string operation, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Crypto, operation, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Security operation logging
        /// </summary>
        public static void LogSecurity(string operation, string message,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            Log(LogLevel.Security, operation, message, callerMember, callerFile, callerLine);
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        public static void LogException(string category, Exception exception,
            [CallerMemberName] string? callerMember = null,
            [CallerFilePath] string? callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.Error,
                Category = category,
                Message = exception.Message,
                CallerMember = callerMember,
                CallerFile = Path.GetFileName(callerFile),
                CallerLine = callerLine,
                Exception = exception.ToString()
            };

            AddLogEntry(entry);
        }

        /// <summary>
        /// Measure elapsed time for operations
        /// </summary>
        public static IDisposable MeasureTime(string category, string operation)
        {
            return new TimedOperation(category, operation);
        }

        #endregion

        #region Specialized Logging Methods

        /// <summary>
        /// Logs an encryption operation
        /// </summary>
        public static void LogEncryption(string algorithm, int dataSize, TimeSpan duration)
        {
            LogCrypto("Encryption", 
                $"Algorithm: {algorithm}, Data: {dataSize} bytes, Time: {duration.TotalMilliseconds:F1}ms");
        }

        /// <summary>
        /// Logs a decryption operation
        /// </summary>
        public static void LogDecryption(string algorithm, int dataSize, TimeSpan duration, bool success)
        {
            var status = success ? "SUCCESS" : "FAILED";
            LogCrypto("Decryption", 
                $"Algorithm: {algorithm}, Data: {dataSize} bytes, Time: {duration.TotalMilliseconds:F1}ms, Status: {status}");
        }

        /// <summary>
        /// Logs key generation
        /// </summary>
        public static void LogKeyGeneration(string keyType, int keySize, TimeSpan duration)
        {
            LogCrypto("KeyGeneration", 
                $"Type: {keyType}, Size: {keySize} bits, Time: {duration.TotalMilliseconds:F1}ms");
        }

        /// <summary>
        /// Logs a UI event
        /// </summary>
        public static void LogUIEvent(string control, string action, string details = "")
        {
            var message = string.IsNullOrEmpty(details) ? 
                $"Control: {control}, Action: {action}" :
                $"Control: {control}, Action: {action}, Details: {details}";
            LogInfo("UI", message);
        }

        /// <summary>
        /// Logs memory usage
        /// </summary>
        public static void LogMemoryUsage(string context)
        {
            var memoryUsage = GC.GetTotalMemory(false);
            LogInfo("Memory", $"Context: {context}, Usage: {memoryUsage / 1024} KB");
        }

        #endregion

        #region Core Implementation

        /// <summary>
        /// Centralized logging method
        /// </summary>
        private static void Log(LogLevel level, string category, string message,
            string? callerMember, string? callerFile, int callerLine)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Category = category,
                Message = message,
                CallerMember = callerMember,
                CallerFile = Path.GetFileName(callerFile),
                CallerLine = callerLine
            };

            AddLogEntry(entry);
        }

        /// <summary>
        /// Adds a log entry
        /// </summary>
        private static void AddLogEntry(LogEntry entry)
        {
            if (!isInitialized) Initialize();

            lock (logLock)
            {
                // Add to memory list
                logEntries.Add(entry);
                
                // Keep only the latest 1000 entries in memory
                if (logEntries.Count > 1000)
                {
                    logEntries.RemoveAt(0);
                }

                // Format the log entry
                var logLine = FormatLogEntry(entry);
                
                // Add to memory log
                memoryLog.AppendLine(logLine);
                
                // Write to file
                WriteToFile(logLine);
                
                // Write to console in debug mode
                Debug.WriteLine(logLine);
                
                // For critical errors, also show on console
                if (entry.Level >= LogLevel.Error)
                {
                    Console.WriteLine(logLine);
                }
            }
        }

        /// <summary>
        /// Formats a log entry
        /// </summary>
        private static string FormatLogEntry(LogEntry entry)
        {
            var levelIcon = entry.Level switch
            {
                LogLevel.Debug => "🐛",
                LogLevel.Info => "ℹ️",
                LogLevel.Warning => "⚠️",
                LogLevel.Error => "❌",
                LogLevel.Critical => "💥",
                LogLevel.Crypto => "🔐",
                LogLevel.Security => "🛡️",
                _ => "📝"
            };

            var durationText = entry.Duration.HasValue ? 
                $" [{entry.Duration.Value.TotalMilliseconds:F0}ms]" : "";

            var callerInfo = !string.IsNullOrEmpty(entry.CallerMember) ? 
                $" ({entry.CallerFile}:{entry.CallerLine} {entry.CallerMember})" : "";

            var logLine = $"[{entry.Timestamp:HH:mm:ss.fff}] {levelIcon} {entry.Level.ToString().ToUpper(),-8} " +
                         $"[{entry.Category}] {entry.Message}{durationText}{callerInfo}";

            // Add exception details if available
            if (!string.IsNullOrEmpty(entry.Exception))
            {
                logLine += $"\n    Exception: {entry.Exception}";
            }

            return logLine;
        }

        /// <summary>
        /// Writes to file in a thread-safe manner
        /// </summary>
        private static void WriteToFile(string content)
        {
            if (string.IsNullOrEmpty(logFilePath)) return;

            try
            {
                File.AppendAllText(logFilePath, content + Environment.NewLine);
            }
            catch
            {
                // Ignore file writing errors
            }
        }

        #endregion

        #region Timed Operations

        /// <summary>
        /// For measuring elapsed time of operations
        /// </summary>
        private class TimedOperation : IDisposable
        {
            private readonly string category;
            private readonly string operation;
            private readonly Stopwatch stopwatch;
            private bool disposed = false;

            public TimedOperation(string category, string operation)
            {
                this.category = category;
                this.operation = operation;
                this.stopwatch = Stopwatch.StartNew();
                
                LogInfo(category, $"Started: {operation}");
            }

            public void Dispose()
            {
                if (disposed) return;
                
                stopwatch.Stop();
                
                var entry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = LogLevel.Info,
                    Category = category,
                    Message = $"Completed: {operation}",
                    Duration = stopwatch.Elapsed
                };

                AddLogEntry(entry);
                disposed = true;
            }
        }

        #endregion

        #region Public Access Methods

        /// <summary>
        /// Returns all log entries
        /// </summary>
        public static List<LogEntry> GetAllLogEntries()
        {
            lock (logLock)
            {
                return new List<LogEntry>(logEntries);
            }
        }

        /// <summary>
        /// Returns log entries of a specific level
        /// </summary>
        public static List<LogEntry> GetLogEntriesByLevel(LogLevel level)
        {
            lock (logLock)
            {
                return logEntries.FindAll(e => e.Level == level);
            }
        }

        /// <summary>
        /// Returns the content of the memory log
        /// </summary>
        public static string GetMemoryLog()
        {
            lock (logLock)
            {
                return memoryLog.ToString();
            }
        }

        /// <summary>
        /// Returns the log file path
        /// </summary>
        public static string? GetLogFilePath()
        {
            return logFilePath;
        }

        /// <summary>
        /// Clears the memory logs (does not affect the file)
        /// </summary>
        public static void ClearMemoryLogs()
        {
            lock (logLock)
            {
                logEntries.Clear();
                memoryLog.Clear();
                LogInfo("Logger", "Memory logs cleared");
            }
        }

        /// <summary>
        /// Generates a summary report
        /// </summary>
        public static string GenerateSummaryReport()
        {
            lock (logLock)
            {
                var report = new StringBuilder();
                
                report.AppendLine("═══ APPLICATION LOG SUMMARY ═══");
                report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine($"Application runtime: {DateTime.Now - applicationStartTime}");
                report.AppendLine();
                
                // Statistics by level
                var levelCounts = new Dictionary<LogLevel, int>();
                foreach (LogLevel level in Enum.GetValues<LogLevel>())
                {
                    levelCounts[level] = logEntries.Count(e => e.Level == level);
                }
                
                report.AppendLine("📊 Log Level Statistics:");
                foreach (var kvp in levelCounts.Where(k => k.Value > 0))
                {
                    report.AppendLine($"   {kvp.Key}: {kvp.Value} entries");
                }
                report.AppendLine();
                
                // Recent errors
                var recentErrors = logEntries
                    .Where(e => e.Level >= LogLevel.Error)
                    .TakeLast(5)
                    .ToList();
                    
                if (recentErrors.Any())
                {
                    report.AppendLine("❌ Recent Errors:");
                    foreach (var error in recentErrors)
                    {
                        report.AppendLine($"   [{error.Timestamp:HH:mm:ss}] {error.Category}: {error.Message}");
                    }
                    report.AppendLine();
                }
                
                report.AppendLine($"📁 Full log file: {logFilePath}");
                report.AppendLine("═══════════════════════════════");
                
                return report.ToString();
            }
        }

        #endregion
    }
}