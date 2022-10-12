using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Threading;
using System.Globalization;

namespace PluginUpdater.Engine
{
    public class FileLogger
    {
        private bool m_isLogCreateChecked = false;
        private Object m_lock = new object();

        public void Log(string message, string title = "")
        {
            string date = DateTime.Now.ToString();
            message = $"[{date}] {title}: {message}";

            SaveTitle(ref message);

            try
            {
                lock (m_lock)
                {
                    using (StreamWriter streamWriter = new StreamWriter(Storage.Instance.LogPath, true))
                    {
                        streamWriter.WriteLine(message);
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error on save log:\n{ex.Message}\n\nSave log:\n{message}", "Error on FileLogger.Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveTitle(ref string message)
        {
            try
            {
                if (m_isLogCreateChecked)
                    return;

                System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                string version = executingAssembly.GetName().Version.ToString();
                string appName = executingAssembly.GetName().Name;
                string splitter = new string('=', 100) + "\n";

                message = $"{splitter}ver: {version}\tPC: {Environment.MachineName}\n{message}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error on save title log:\n{ex.Message}\n\nSave log:\n{message}", "Error on FileLogger.SaveTitle", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                m_isLogCreateChecked = true;
            }
        }
    }

    public static class Logger
    {
        private static FileLogger m_logger = new FileLogger();

        public static void Error(Exception ex, string title = "", string description = "")
        {
            string strErr = ex.ToString();

            if (string.IsNullOrEmpty(title))
                title = "Error:";

            if (!string.IsNullOrEmpty(description))
                strErr = $"\n{description}\n{strErr}";

            strErr = $"{strErr}\n{Environment.StackTrace}";

            m_logger.Log(strErr, title);
        }

        public static void Debug(string message, string title = "")
        {
            if (string.IsNullOrEmpty(title))
                title = "Debug:";

            m_logger.Log(message, title);
        }
    }
}
