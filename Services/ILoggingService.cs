using System;
using System.Runtime.CompilerServices;

namespace ReactAspNetApp.Services
{
    /// <summary>
    /// Interface for centralized logging service
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Log informational message
        /// </summary>
        void LogInformation(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log warning message
        /// </summary>
        void LogWarning(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log error message
        /// </summary>
        void LogError(string message, Exception? exception = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log critical error message
        /// </summary>
        void LogCritical(string message, Exception? exception = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log debug message
        /// </summary>
        void LogDebug(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log method entry
        /// </summary>
        void LogMethodEntry(string? additionalInfo = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Log method exit
        /// </summary>
        void LogMethodExit(string? additionalInfo = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}

