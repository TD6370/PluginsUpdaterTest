using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private void DownloadProgressChanged(int progress)
        {
            //ProgressInstallValue = progress;
        }

        public async Task StartInstall()
        {
            try
            {
                //Thread.Sleep(1000);
                var test = Task.CurrentId.ToString();

                var pluginsNeedDelete = FilterRemovePlugins();
                var pluginsNeedInstall = FilterNewPlugins();

                ProgressInstallMaxValue = pluginsNeedDelete.Count() + pluginsNeedInstall.Count();
                ProgressInstallValue = 0;

                await DeletePluginsAsync(pluginsNeedDelete);
                await InstallPluginsAsync(pluginsNeedInstall);

                IsCompleted = true;
            }catch(Exception ex)
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

        private async Task InstallPluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                var test = Task.CurrentId.ToString();

                foreach (var plugin in plugins)
                {
                    string fileName = plugin.GetNameFile();
                    string pathInstallFile = string.Concat(m_installPath, $"\\{plugin.ID}\\{fileName}");
                    
                    //TEST
                    //pathInstallFile += "_" + plugin.Version;

                    string pathInstall = Path.GetDirectoryName(pathInstallFile);
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        plugin.Delete(pathInstall);
                    }
                    plugin.DownloadProgressChanged = DownloadProgressChanged;
                    plugin.Install(pathInstallFile);

                    ProgressInstallValue++;
                }

                //TEST
                Thread.Sleep(500);
            });
        }

        private async Task DeletePluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                var test = Task.CurrentId.ToString();

                foreach (var plugin in plugins)
                {
                    string pathInstall = string.Concat(m_installPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                        plugin.Delete(pathInstall);

                    ProgressInstallValue++;

                    //TEST
                    Thread.Sleep(1000);
                }
            });
        }

        private void Close()
        {
            if (CloseAction != null)
                CloseAction();
        }
    }
}
