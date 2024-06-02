using System;
using System.Diagnostics;
using System;


namespace Airports
{
        public class LogEventArgs : EventArgs
        {
        public string Message { get; }
        public TraceLevel Level { get; }

        public LogEventArgs(string message, TraceLevel level)
            {
                Message = message;
                Level = level;
            }
        }

    public class Logging
    {
        public event EventHandler<LogEventArgs> LoggingEvent;

        public void Write(string message, TraceLevel level = TraceLevel.Verbose)
        {
            string logMessage = Identifier(level) + message;
            Debug.WriteLine(logMessage);
            OnLoggingEvent(new LogEventArgs(logMessage, level));
        }

        public void Write(Exception ex, TraceLevel level = TraceLevel.Error)
        {
            string logMessage = Identifier(level) + ex.Message;
            Debug.WriteLine(logMessage);
            OnLoggingEvent(new LogEventArgs(logMessage, level));

            string stackTrace = Identifier(level) + ex.StackTrace;
            Debug.WriteLine(stackTrace);
            OnLoggingEvent(new LogEventArgs(stackTrace, level));
        }

        protected virtual void OnLoggingEvent(LogEventArgs e)
        {
            LoggingEvent?.Invoke(this, e);
        }

        private string Identifier(TraceLevel level)
        {
            string identifier;
            if (level == TraceLevel.Error)
                identifier = "LOG: ERROR: >>>>>>>>>>>>>>>>>>>>>> ";
            else
                identifier = "LOG: " + level.ToString() + ": ";

            return DateTime.Now.TimeOfDay + " : " + identifier;
        }
    }
}
