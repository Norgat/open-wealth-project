﻿using System;

namespace OpenWealth.Simple
{
    public class Log : ILog, ILogManager
    {
        string name;
        static int level = 5; // по умолчанию логирование выключено, включается, например, при подключении DevTools

        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                WriteEntry( message.ToString(), LogLevel.Debug);
            }
        }
        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                WriteEntry( message.ToString(), LogLevel.Info);
            }
        }
        public void Warn(object message)
        {
            if (IsWarnEnabled)
            {
                WriteEntry(message.ToString(), LogLevel.Warn);
            }
        }
        public void Error(object message)
        {
            if (IsErrorEnabled)
            {
                WriteEntry(message.ToString(), LogLevel.Error);
            }
        }
        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                WriteEntry(message.ToString() + " Exception: " + exception.ToString(), LogLevel.Error);
            }
        }
        public void Fatal(object message)
        {
            if (IsFatalEnabled)
            {
                WriteEntry( message.ToString(), LogLevel.Fatal);
            }
        }

        public bool IsDebugEnabled { get { return (level == 0); } }
        public bool IsInfoEnabled  { get { return (level <= 1); } }
        public bool IsWarnEnabled  { get { return (level <= 2); } }
        public bool IsErrorEnabled { get { return (level <= 3); } }
        public bool IsFatalEnabled { get { return (level <= 4); } }

        /// <summary>
        /// Конструктор, может вызываться только из Core
        /// </summary>
        /// <param name="name">Имя создаваемого логгера</param>
        internal Log(string name)
        {
            this.name = name;
        }
        
        static event LogEventHandler staticLogEvent;
        public event LogEventHandler LogEvent
        {
            add { staticLogEvent += value; }
            remove { staticLogEvent -= value; }
        }

        private void WriteEntry(string message, LogLevel level)
        {
            DateTime dt = DateTime.Now;
            //TODO Вызывающий метод
            System.Diagnostics.Trace.WriteLine(
                    string.Format("{0},{1},{2},{3},{4}",
                                  dt.ToString("yyyy-MM-dd HH:mm:ss"),
                                  System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() , //TODO номер потока в шеснадцатиричном виде
                                  level.ToString() ,
                                  name,
                                  message));
            // вызывать событие
            LogEventHandler logEvent = staticLogEvent;
            if (logEvent != null)
                logEvent(this, new LogEventArgs(dt, name, message, level));
        }

        #region Реализация интерфейса ILogManager

        public void SetLevel(LogLevel newLogLevel)
        {
            System.Threading.Interlocked.Exchange(ref level, (int)newLogLevel);
        }

        public ILog GetLogger(string name)
        {
            //TODO проверку, что такого логгера ещё нет
            return new Log(name);
        }

        #endregion Реализация интерфейса ILogManager
    }
}