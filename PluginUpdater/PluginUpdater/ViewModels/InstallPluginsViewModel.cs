using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PluginUpdater.ViewModels
{
    public class InstallPluginsViewModel : BaseNotifyPropertyChanged
    {
        private IPluginsInstaller m_pluginsInstaller;
        private PluginsCollection m_plugins;
        private PluginsUsedCollection m_pluginsUsed;
        private string m_installPath;
        public Action CloseAction { private get; set; }

        private bool m_isCompleted;
        public bool IsCompleted
        {
            get { return m_isCompleted; }
            set
            {
                m_isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }
        private ProgressInfoCollection m_progressCollection;
        public ProgressInfoCollection ProgressCollection => m_progressCollection;
        
        private int m_progressInstallValue;
        public int ProgressInstallValue
        {
            get { return m_progressInstallValue; }
            set { 
                m_progressInstallValue = value;
                OnPropertyChanged(nameof(ProgressInstallValue));
            }
        }

        private int m_progressInstallMaxValue = 1;
        public int ProgressInstallMaxValue
        {
            get { return m_progressInstallMaxValue; }
            set
            {
                m_progressInstallMaxValue = value;
                OnPropertyChanged(nameof(ProgressInstallMaxValue));
            }
        }

        public InstallPluginsViewModel(PluginsCollection plugins, PluginsUsedCollection pluginsUsed, string installPath, IPluginsInstaller pluginsInstaller)
        {
            m_plugins = plugins;
            m_installPath = installPath;
            m_pluginsUsed = pluginsUsed;
            m_pluginsInstaller = pluginsInstaller;
        }

        public void StartInstall()
        {
            StartInstall(m_pluginsInstaller);
        }

        public async Task StartInstall(IPluginsInstaller pluginsInstaller)
        {
            try
            {
                m_progressCollection = new ProgressInfoCollection();

                pluginsInstaller.InstallPath = m_installPath;
                pluginsInstaller.ProgressEvent += (obj, progressInfo) => { ProgressChange(progressInfo); };

                var pluginsNeedDelete = FilterRemovePlugins();
                var pluginsNeedInstall = FilterNewPlugins();

                ProgressInstallMaxValue = pluginsNeedDelete.Count() + pluginsNeedInstall.Count();
                ProgressInstallValue = 0;

                await pluginsInstaller.DeletePluginsAsync(pluginsNeedDelete);
                await pluginsInstaller.InstallPluginsAsync(pluginsNeedInstall);

                IsCompleted = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on InstallPluginsViewModel.StartInstall");
                MessageBox.Show(ex.Message, "Error on Start install plugins", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void ProgressChange(IProgressInfo _progressInfo)
        {
            ProgressInfo progressInfo = _progressInfo as ProgressInfo;
            WpfHelper.InvokeMethod(() =>
            {
                m_progressCollection.Update(progressInfo);
                OnPropertyChanged(nameof(ProgressCollection));
                ProgressInstallValue = progressInfo.Value;
            });
        }

        public bool IsInstallValid()
        {
            return FilterNewPlugins().Any() || FilterRemovePlugins().Any();
        }

        public IEnumerable<IPlugin> FilterNewPlugins()
        {
            return m_plugins.Where(p => p.Checked && m_pluginsUsed.IsNew(p)).Select( p=> p.Plugin);
        }

        public IEnumerable<IPlugin> FilterRemovePlugins()
        {
            return m_plugins.Where(p => !p.Checked && m_pluginsUsed.IsExist(p)).Select(p => p.Plugin);
        }
        
        private void Close()
        {
            if (CloseAction != null)
                CloseAction();
        }

        public void Closing(CancelEventArgs args)
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                args.Cancel = true;
                m_pluginsInstaller.Cancel();
            }
        }

        private void Cancel()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                m_pluginsInstaller.Cancel();
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Cancel();
                });
            }
        }
    }
}
