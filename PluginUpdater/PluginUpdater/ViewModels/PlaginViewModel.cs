using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginUpdater.ViewModels
{
    
    public class PluginViewModel : BaseNotifyPropertyChanged
    {
        private string m_path;

        private IPlugin m_plagin;
        public IPlugin Plugin => m_plagin;
        
        private string m_errorMessage;
        public string ErrorMessage => m_errorMessage;

        public virtual string ID 
        {
            get { return m_plagin.ID; }
            set { m_plagin.ID = value; }
        }
        public virtual long Version
        {
            get { return m_plagin.Version; }
            set { m_plagin.Version = value; }
        }
        public virtual string DownloadLink
        {
            get { return m_plagin.DownloadLink; }
            set { m_plagin.DownloadLink = value; }
        }

        public PluginViewModel(IPlugin plagin)
        {
            m_plagin = plagin;
        }

        public StatusChecked Status
        {
            get {
                var plugin = m_plagin as Plugin;
                return plugin != null ? plugin.Status : StatusChecked.None;
            }
            set {
                var plugin = m_plagin as Plugin;
                if(plugin != null)
                    plugin.Status = value;
            }
        }

        private bool m_checked;
        public bool Checked
        {
            get { return m_checked; }
            set
            {
                m_checked = value;
                if (CheckedChanged != null)
                    CheckedChanged(m_plagin);
                OnPropertyChanged(nameof(Checked));
            }
        }

        public Action<int> DownloadProgressChanged { get; set; }

        public Action<IPlugin> CheckedChanged { get; set; }

        public string Path => m_path;

        public bool IsNeedUpdated { get; set; }

        public bool IsNeedDeleted { get; set; }

        public string Install(string path)
        {
            m_path = path;
            var result = Storage.Instance.DownloadFile(DownloadLink, ref m_path, DownloadProgressChanged);
            return m_errorMessage = result;
        }

        public string Delete(string path)
        {
            try
            {
                m_path = path;
                Storage.Instance.DeleteDerictory(path);
            }catch(Exception ex)
            {
                Logger.Error(ex, "Error on Delete", $"path={path}");
                return $"Error on Delete File:\nPath={path}\n{ex.Message}";
            }
            return string.Empty;
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
