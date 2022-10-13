using PluginUpdater.Engine;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


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

        public string Path { get; }

        public bool Checked { get; set; }

        public StatusChecked Status { get; set; }

        public Action<int> DownloadProgressChanged { get; set; }

        public Action<IPlugin> CheckedChanged { get; set; }

        public void Install(string path);

        public void Delete(string path);

        public string GetNameFile();

        public void UpdateView();
    }

    public class Plugin : BaseNotifyPropertyChanged, IPlugin
    {
        private string m_path;

        public virtual string ID { get; set; }
        public virtual long Version { get; set; }
        public virtual string DownloadLink { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public StatusChecked Status { get; set; }

        private bool m_checked;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Checked { 
            get { return m_checked; } 
            set { 
                m_checked = value;
                if (CheckedChanged != null)
                    CheckedChanged(this);
            } 
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Action<int> DownloadProgressChanged { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Action<IPlugin> CheckedChanged { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Path => m_path;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsNeedUpdated { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsNeedDeleted { get; set; }
                
        public void Install(string path)
        {
            m_path = path;
            Storage.Instance.DownloadFile(DownloadLink, ref m_path, DownloadProgressChanged);
        }

        public void Delete(string path)
        {
            m_path = path;
            Storage.Instance.DeleteDerictory(path);
        }

        public string GetNameFile()
        {
            return System.IO.Path.GetFileName(m_path);
        }

        public void UpdateView()
        {
            OnPropertyChanged(nameof(Status));
        }
    }

}
