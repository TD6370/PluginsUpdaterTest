using PluginUpdater.Engine;
using PluginUpdater.ViewModels;
using System;

namespace PluginUpdater.Models
{
    public enum StatusChecked
    {
        None,
        NeedUpdated,
        NeedDeleted,
        NeedInstall,
    }

    public interface IPlugin
    {
        public string ID { get; set; }
        public long Version { get; set; }
        public string DownloadLink { get; set; }
    }

    public class Plugin : IPlugin
    {
        public string ID { get; set; }
        public long Version { get; set; }
        public string DownloadLink { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public StatusChecked Status { get; set; }
    }
}
