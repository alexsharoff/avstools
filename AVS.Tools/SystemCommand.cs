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

namespace AVS.Tools
{
    /// <summary>
    /// Системная команда.
    /// При выполнении консоль и окна не отображаются.
    /// </summary>
    public class SystemCommand
    {
        Process m_proc;
        /// <summary>
        /// ToString формат.
        /// </summary>
        const string Format = "\"{0}\" {1}";
        /// <summary>
        /// Путь к исполнительному файлу
        /// </summary>
        public string ExecutablePath
        {
            get;
            set;
        }
        /// <summary>
        /// Параметры передаваемые исполнительному файлу.
        /// </summary>
        public string Parameters
        {
            get;
            set;
        }
        /// <summary>
        /// Выполнить команду
        /// </summary>
        /// <returns>Возвращенный код</returns>
        public int Execute()
        {
            if (ExecutablePath == null)
            {
                throw new ArgumentNullException("ExecutablePath");
            }
            ProcessStartInfo netshStartInfo = new ProcessStartInfo(ExecutablePath);
            netshStartInfo.UseShellExecute = false;
            netshStartInfo.CreateNoWindow = true;
            netshStartInfo.Arguments = Parameters;
            m_proc = Process.Start(netshStartInfo);
            m_proc.WaitForExit();
            return m_proc.ExitCode;
        }

        public void Start()
        {
            if (ExecutablePath == null)
            {
                throw new ArgumentNullException("ExecutablePath");
            }
            ProcessStartInfo netshStartInfo = new ProcessStartInfo(ExecutablePath);
            netshStartInfo.UseShellExecute = false;
            netshStartInfo.CreateNoWindow = true;
            netshStartInfo.Arguments = Parameters;
            m_proc = Process.Start(netshStartInfo);
        }

        public Process Process
        {
            get { return m_proc; }
        }

        /// <summary>
        /// Приведение к строке
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString()
        {
            return string.Format(Format, ExecutablePath, Parameters);
        }
    }
}
