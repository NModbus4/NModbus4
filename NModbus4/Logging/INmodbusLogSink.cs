namespace Modbus.Logging
{
    /// <summary>
    /// Logger interface for simple integration with logging frameworks
    /// </summary>
    public interface INmodbusLogSink
    {
        /// <summary>
        /// Log message with appropriate severity
        /// </summary>
        /// <param name="severity">Message severity</param>
        /// <param name="message">Message to log</param>
        void Log(Severity severity, string message);
    }
}
