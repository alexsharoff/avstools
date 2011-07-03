/// <summary>
/// Авторские права
/// Содержащаяся здесь информация является собственностью ООО НПФ "Гранч" и
/// представляет собой коммерческую тайну ООО НПФ "Гранч", или его лицензией,
/// и является предметом ограничений на использование и раскрытие информации.

/// Copyright (c) 2009, 2010 ООО НПФ "Гранч". Все права защищены.

/// Уведомления об авторских правах, указанные выше, не являются основанием и не
/// дают права для публикации данного материала.
/// </summary>
/// <author>Шаров Александр</author>
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AVS.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class UniversalLog : TextWriter
    {
        static TextWriter ConsoleOut;

        static UniversalLog()
        {
            ConsoleOut = Console.Out;
        }

        /// <summary>
        /// Общий формат сообщений: [time] [type] message
        /// </summary>
        private const string EntryFormat = "[{0:dd.MM.yyyy HH:mm:ss}] [{1}] {2}";
        /// <summary>
        /// Формат для исключений: message\r\nstack
        /// </summary>
        private const string ExceptionFormat = "Exception: {0}\r\n{1}";

        /// <summary>
        /// Флаг, указывающий на то, что сообщение о недостатке прав 
        /// для записи в EventLog уже было выведено.
        /// </summary>
        private bool m_eventLogMessageShown = false;
        /// <summary>
        /// Пусть к файлу с логом
        /// </summary>
        private string m_logFilePath = null;
        /// <summary>
        /// Поток лога
        /// </summary>
        private StreamWriter m_writer = null;
        /// <summary>
        /// Название источника для EventLog.
        /// </summary>
        private string m_source = null;
        /// <summary>
        /// Блокирующий объект для операций ввода-вывода.
        /// </summary>
        private object m_lock = new object();
        /// <summary>
        /// Конструктор
        /// </summary>
        /// 
        public UniversalLog()
        {
            RedirectToConsole = true;
            RedirectToDebug = true;
        }

        #region Properties
        /// <summary>
        /// Отображать сообщения на консоли
        /// </summary>
        public bool RedirectToConsole
        {
            get;
            set;
        }
        /// <summary>
        /// Выводить сообщения в системном журнале событий
        /// </summary>
        public bool RedirectToEventLog
        {
            get;
            set;
        }
        /// <summary>
        /// Записывать сообщения в файл.
        /// </summary>
        public bool RedirectToFile
        {
            get;
            set;
        }
        /// <summary>
        /// Записывать сообщения в Debug output.
        /// </summary>
        public bool RedirectToDebug
        {
            get;
            set;
        }
        /// <summary>
        /// Имя источника для системного журнала событий
        /// </summary>
        public string EventSource
        {
            get
            {
                return m_source;
            }
            set
            {
                m_source = value;
                try
                {
                    if (m_source != null && m_source.Length > 0 && !EventLog.SourceExists(m_source))
                        EventLog.CreateEventSource(m_source, "Application");
                }
                catch { }
            }

        }
        /// <summary>
        /// Путь к файлу для логгирования
        /// </summary>
        public string PathToFile
        {
            get
            {
                return m_logFilePath;
            }
            set
            {
                string logDir = Path.GetDirectoryName(value);
                if (logDir == null || logDir.Length == 0)
                {
                    m_logFilePath = Path.Combine(Path.GetDirectoryName(
                        Process.GetCurrentProcess().MainModule.FileName), value);
                }
                else
                {
                    m_logFilePath = value;
                }
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Запись сообщения
        /// </summary>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="messageFormat">формат сообщения</param>
        /// <param name="parameters">Параметры</param>
        public void Write(EventLogEntryType eventType, string messageFormat, params object[] parameters)
        {
            string message = string.Format(messageFormat, parameters);
            WriteToEventLog(message, eventType);
            WriteToStreams(message, eventType);
        }
        /// <summary>
        /// Запись ошибки
        /// </summary>
        /// <param name="e">Исключение</param>
        public void Write(Exception e)
        {
            string message = string.Format(ExceptionFormat, e.Message, e.StackTrace);
            WriteToEventLog(message, EventLogEntryType.Error);
            WriteToStreams(message, EventLogEntryType.Error);
        }
        #endregion

        #region Protected & private methods

        /// <summary>
        /// Запись сообщения
        /// </summary>
        /// <param name="messageFormat">формат сообщения</param>
        /// <param name="parameters">Параметры</param>
        void WriteHandler(string messageFormat, params object[] parameters)
        {
            Write(EventLogEntryType.Information, messageFormat, parameters);
        }
        /// <summary>
        /// Запись в потоки
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="type">Тип</param>
        protected void WriteToStreams(string message, EventLogEntryType type)
        {
            string entry = string.Format(EntryFormat, DateTime.Now,
                type, message);

            lock (m_lock)
            {
                if (RedirectToConsole)
                {
                    ConsoleOut.WriteLine(entry);
                    if (entry.Length >= 80)
                    {
                        ConsoleOut.WriteLine();
                    }
                }
                if (RedirectToFile)
                {
                    if (EnsureStreamIsOpen())
                    {
                        m_writer.WriteLine(entry);
                        m_writer.Flush();
                    }
                }
                if (RedirectToDebug)
                {
                    Debug.WriteLine(entry);
                }
            }
        }
        /// <summary>
        /// Запись в системный журнал событий
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="type">Тип сообщения</param>
        protected void WriteToEventLog(string message, EventLogEntryType type)
        {
            if (m_source != null)
            {
                try
                {
                    if (!EventLog.SourceExists(m_source))
                        EventLog.CreateEventSource(m_source, "Application");
                }
                catch
                {
                    if (!m_eventLogMessageShown)
                    {
                        WriteToStreams(string.Format(
                            "Невозможно создать журнал для источника {0}: нет доступа.", m_source),
                            EventLogEntryType.Information);
                        m_eventLogMessageShown = true;
                    }
                    return;
                }
                try
                {
                    EventLog.WriteEntry(m_source, message, type);
                }
                catch (Exception e)
                {
                    string msg = string.Format(ExceptionFormat, e.Message, e.StackTrace);
                    WriteToStreams(msg, EventLogEntryType.Information);
                }
            }
        }
        /// <summary>
        /// Удостовериться, что поток лога открыт.
        /// </summary>
        /// <returns></returns>
        private bool EnsureStreamIsOpen()
        {
            if (m_writer == null)
            {
                try
                {
                    m_writer = new StreamWriter(m_logFilePath, true, Encoding.UTF8);
                    m_writer.AutoFlush = true;
                }
                catch
                {
                    m_writer = null;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region TextWriter members
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(string format, object arg0)
        {
            WriteHandler(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WriteHandler(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteHandler(format, arg0, arg1, arg2);
        }

        public override void Write(string value)
        {
            WriteHandler(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteHandler(format, arg);
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteHandler(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteHandler(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteHandler(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string value)
        {
            WriteHandler(value);
        }
        #endregion
    }
}
