using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PluginUpdater.ViewModels
{
    public class InstallPluginsViewModel : BaseNotifyPropertyChanged
    {
        private PluginsCollection m_plugins;
        private PluginsUsedCollection m_pluginsUsed;
        private string m_installPath;
        public Action CloseAction { private get; set; }
        public bool IsCompleted { get; private set; }

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

        public InstallPluginsViewModel(PluginsCollection plugins, PluginsUsedCollection pluginsUsed, string installPath)
        {
            m_plugins = plugins;
            m_installPath = installPath;
            m_pluginsUsed = pluginsUsed;
        }

        public async Task StartInstall(IPluginsInstaller pluginsInstaller)
        {
            try
            {
                pluginsInstaller.InstallPath = m_installPath;
                pluginsInstaller.ProgressActtion = progress =>
                {
                    ProgressInstallValue = progress;
                };

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

        public bool IsInstallValid()
        {
            return FilterNewPlugins().Any() || FilterRemovePlugins().Any();
        }

        public IEnumerable<IPlugin> FilterNewPlugins()
        {
            return m_plugins.Where(p => p.Checked && m_pluginsUsed.IsNew(p));
        }

        public IEnumerable<IPlugin> FilterRemovePlugins()
        {
            return m_plugins.Where(p => !p.Checked && m_pluginsUsed.IsExist(p));
        }
        
        private void Close()
        {
            if (CloseAction != null)
                CloseAction();
        }
    }
}
