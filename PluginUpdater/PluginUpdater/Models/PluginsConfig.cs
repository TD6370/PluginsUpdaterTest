using PluginUpdater.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PluginUpdater.Models
{
    public class PluginsConfig
    {
        //путь(URL) к документу со списком плагинов
        public string PluginsURL { get; set; }
        //-- путь к локальной директории, где хранятся установленные плагины
        public string PluginsInstallPath { get; set; }
        //-- путь к программе, для которой эти плагины предназначены
        public string PluginApplicationOwnerPath { get; set; }

        public string IvalidMessage()
        {
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrEmpty(PluginsURL))
            {
                sb.Append("В настройках не указан путь(URL) к плагинам!");
                sb.AppendLine();
            }
            if (string.IsNullOrEmpty(PluginsInstallPath))
            {
                sb.Append("В настройках не указан путь к локальной директории, где хранятся установленные плагины!");
                sb.AppendLine();
            }
            if (!Directory.Exists(PluginsInstallPath))
            {
                sb.Append("В настройках путь к локальной директории, где хранятся установленные плагины указан не верно!");
                sb.AppendLine();
            }
            if (string.IsNullOrEmpty(PluginApplicationOwnerPath))
            {
                sb.Append("В настройках не указан путь к путь к программе для плагинов!");
                sb.AppendLine();
            }
            if (!File.Exists(PluginApplicationOwnerPath))
            {
                sb.Append("В настройках путь к программе для плагинов указан не верно!");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static PluginsConfig GetDefault()
        {
            var m_config = new PluginsConfig
            {
                PluginsURL = Storage.Instance.GetPlaginTestURL(),
                PluginsInstallPath = Storage.Instance.GetPathPluginInstall(),
                PluginApplicationOwnerPath = Storage.Instance.PluginApplicationOwnerPathDefault
            };
            Storage.Instance.Save(m_config, Storage.Instance.PathPluginConfig);
            return m_config;
        }
    }
}

