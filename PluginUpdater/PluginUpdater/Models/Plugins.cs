using PluginUpdater.Engine;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace PluginUpdater.Models
{
    public interface IPlugin
    {
        public string ID { get; set; }
        public long Version { get; set; }

        public string DownloadLink { get; set; }

        public bool Checked { get; set; }

        public Action<int> DownloadProgressChanged { get; set; }

        public void Install(string pathInstall);

        public void Delete(string pathInstall);

        public string GetNameFile();
    }

    public class Plugin : IPlugin
    {
        public virtual string ID { get; set; }
        public virtual long Version { get; set; }
        public virtual string DownloadLink { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Checked { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Action<int> DownloadProgressChanged { get; set; }

        public void Install(string pathInstall)
        {
            Storage.Instance.DownloadFile(DownloadLink, pathInstall, DownloadProgressChanged);
        }

        public void Delete(string pathInstall)
        {
            Storage.Instance.DeleteDerictory(pathInstall);
        }

        public string GetNameFile()
        {
            if (string.IsNullOrEmpty(DownloadLink))
                return string.Empty;

            return Path.GetFileName(DownloadLink);
        }
    }

}
