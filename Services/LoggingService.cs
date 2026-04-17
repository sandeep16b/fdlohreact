using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReactAspNetApp.Services
{
    /// <summary>
    /// Centralized logging service using NLog
    /// Provides comprehensive logging with class name, method name, exception details, and stack trace
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly NLog.ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(IHttpContextAccessor httpContextAccessor)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Extract class name from file path
        /// </summary>
        private string GetClassName(string sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                return "Unknown";

            return Path.GetFileNameWithoutExtension(sourceFilePath);
        }

        /// <summary>
        /// Get server name from environment
        /// </summary>
        private string GetServerName()
        {
            return Environment.MachineName ?? "Unknown";
        }

        /// <summary>
        /// Get additional context information from HTTP request
        /// </summary>
        private NLog.LogEventInfo CreateLogEvent(NLog.LogLevel level, string message, string className, string methodName, int lineNumber)
        {
            var logEvent = new NLog.LogEventInfo(level, _logger.Name, message);
            
            // Add custom properties
            logEvent.Properties["ClassName"] = className;
            logEvent.Properties["MethodName"] = methodName;
            logEvent.Properties["LineNumber"] = lineNumber;
            logEvent.Properties["ServerName"] = GetServerName();
            logEvent.Properties["MachineName"] = Environment.MachineName;
            logEvent.Properties["ProcessId"] = Environment.ProcessId;
            logEvent.Properties["ThreadId"] = Environment.CurrentManagedThreadId;

            // Add HTTP context information if available
            if (_httpContextAccessor?.HttpContext != null)
            {
                var context = _httpContextAccessor.HttpContext;
                logEvent.Properties["Url"] = context.Request.Path + context.Request.QueryString;
                logEvent.Properties["HttpMethod"] = context.Request.Method;
                logEvent.Properties["UserName"] = context.User?.Identity?.Name ?? "Anonymous";
                logEvent.Properties["RequestId"] = context.TraceIdentifier;
                logEvent.Properties["CorrelationId"] = context.TraceIdentifier; // You can use a custom correlation ID here
            }

            return logEvent;
        }

        /// <summary>
        /// Get formatted exception details including inner exceptions
        /// </summary>
        private string? GetExceptionDetails(Exception exception)
        {
            if (exception == null)
                return null;

            var sb = new StringBuilder();
            var currentException = exception;
            int level = 0;

            while (currentException != null)
            {
                if (level > 0)
                    sb.AppendLine($"--- Inner Exception {level} ---");

                sb.AppendLine($"Type: {currentException.GetType().FullName}");
                sb.AppendLine($"Message: {currentException.Message}");
                sb.AppendLine($"Stack Trace: {currentException.StackTrace}");

                if (currentException.Data != null && currentException.Data.Count > 0)
                {
                    sb.AppendLine("Data:");
                    foreach (var key in currentException.Data.Keys)
                    {
                        sb.AppendLine($"  {key}: {currentException.Data[key]}");
                    }
                }

                currentException = currentException.InnerException;
                level++;
            }

            return sb.ToString();
        }

        public void LogInformation(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var logEvent = CreateLogEvent(NLog.LogLevel.Info, message, className, memberName, sourceLineNumber);
            _logger.Log(logEvent);
        }

        public void LogWarning(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var logEvent = CreateLogEvent(NLog.LogLevel.Warn, message, className, memberName, sourceLineNumber);
            _logger.Log(logEvent);
        }

        public void LogError(string message, Exception? exception = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var logEvent = CreateLogEvent(NLog.LogLevel.Error, message, className, memberName, sourceLineNumber);

            if (exception != null)
            {
                logEvent.Exception = exception;
                logEvent.Properties["ExceptionDetails"] = GetExceptionDetails(exception);
                
                // Add inner exception separately for easier querying
                if (exception.InnerException != null)
                {
                    logEvent.Properties["InnerException"] = exception.InnerException.ToString();
                }
            }

            _logger.Log(logEvent);
        }

        public void LogCritical(string message, Exception? exception = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var logEvent = CreateLogEvent(NLog.LogLevel.Fatal, message, className, memberName, sourceLineNumber);

            if (exception != null)
            {
                logEvent.Exception = exception;
                logEvent.Properties["ExceptionDetails"] = GetExceptionDetails(exception);
                
                // Add inner exception separately for easier querying
                if (exception.InnerException != null)
                {
                    logEvent.Properties["InnerException"] = exception.InnerException.ToString();
                }
            }

            _logger.Log(logEvent);
        }

        public void LogDebug(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var logEvent = CreateLogEvent(NLog.LogLevel.Debug, message, className, memberName, sourceLineNumber);
            _logger.Log(logEvent);
        }

        public void LogMethodEntry(string? additionalInfo = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var message = $"Entering {className}.{memberName}";
            if (!string.IsNullOrEmpty(additionalInfo))
                message += $" - {additionalInfo}";

            var logEvent = CreateLogEvent(NLog.LogLevel.Trace, message, className, memberName, sourceLineNumber);
            _logger.Log(logEvent);
        }

        public void LogMethodExit(string? additionalInfo = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var className = GetClassName(sourceFilePath);
            var message = $"Exiting {className}.{memberName}";
            if (!string.IsNullOrEmpty(additionalInfo))
                message += $" - {additionalInfo}";

            var logEvent = CreateLogEvent(NLog.LogLevel.Trace, message, className, memberName, sourceLineNumber);
            _logger.Log(logEvent);
        }
    }
}

