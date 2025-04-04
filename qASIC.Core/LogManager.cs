using System;

namespace qASIC
{
    public class LogManager
    {
        public LogManager() : this(qDebug.DEFAULT_COLOR_TAG, qDebug.WARNING_COLOR_TAG, qDebug.ERROR_COLOR_TAG) { }
        public LogManager(string defaultColorTag) : this(defaultColorTag, qDebug.WARNING_COLOR_TAG, qDebug.ERROR_COLOR_TAG) { }
        public LogManager(string defaultColorTag, string warningColor, string errorColor)
        {
            DefaultColorTag = defaultColorTag;
            WarningColorTag = warningColor;
            ErrorColorTag = errorColor;
        }

        public string DefaultColorTag { get; set; }
        public string WarningColorTag { get; set; }
        public string ErrorColorTag { get; set; }

        #region Closing
        public bool Closed { get; private set; } = false;
        public Action<LogManager> OnClose;

        public virtual void Close()
        {
            Closed = true;
            OnClose?.Invoke(this);
        }
        #endregion

        #region Logging
        public event Action<qLog> OnLog;

        protected void InvokeOnLog(qLog log) =>
            OnLog?.Invoke(log);

        public virtual void Log(qLog log)
        {
            if (Closed)
                throw new Exception("Can't log, log manager closed!");

            InvokeOnLog(log);
        }

        public void Log(string message, string colorTag) =>
            Log(qLog.CreateNow(message, colorTag));

        public void Log(string message, qColor color) =>
            Log(qLog.CreateNow(message, color));

        public void Log(string message) =>
            Log(qLog.CreateNow(message, DefaultColorTag));

        public void LogWarning(string message) =>
            Log(qLog.CreateNow(message, WarningColorTag));

        public void LogError(string message) =>
            Log(qLog.CreateNow(message, ErrorColorTag));
        #endregion

        #region Loggables
        /// <summary>Subscribes to messages from a <see cref="IHasLogs"/>.</summary>
        /// <param name="loggable">The loggable to register.</param>
        /// <returns>Returns itself.</returns>
        public LogManager RegisterLoggable(IHasLogs loggable)
        {
            RegisterManager(loggable?.Logs);
            return this;
        }

        /// <summary>Subscribes to messages from a <see cref="LogManager"/>.</summary>
        /// <param name="other">The other manager to register.</param>
        /// <returns>Returns itself.</returns>
        public virtual LogManager RegisterManager(LogManager other)
        {
            if (other != null && other != this)
                other.OnLog += Log;

            return this;
        }

        /// <summary>Unsubscribes from messages from a <see cref="IHasLogs"/>.</summary>
        /// <param name="loggable">The loggable to deregister.</param>
        /// <returns>Returns itself.</returns>
        public LogManager UnregisterLoggable(IHasLogs loggable)
        {
            UnregisterManager(loggable?.Logs);
            return this;
        }

        /// <summary>Unsubscribes from messages from a <see cref="LogManager"/>.</summary>
        /// <param name="other">The other manager to deregister.</param>
        /// <returns>Returns itself.</returns>
        public virtual LogManager UnregisterManager(LogManager other)
        {
            if (other != null && other != this)
                other.OnLog -= Log;

            return this;
        }
        #endregion
    }
}