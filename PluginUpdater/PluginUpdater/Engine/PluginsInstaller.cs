using PluginUpdater.Models;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PluginUpdater.Engine
{
    public interface IPluginsInstaller
    {
        public string InstallPath { get; set; }

        public Action<IProgressInfo> ProgressAction { get; set; }
        
        public Task InstallPluginsAsync(IEnumerable<IPlugin> plugins);

        public Task DeletePluginsAsync(IEnumerable<IPlugin> plugins);
    }

    public class PluginsInstaller : IPluginsInstaller
    {
        public string InstallPath { get; set; }

        public Action<IProgressInfo> ProgressAction { get; set; }

        private int m_progressInstallValue;

        public async Task InstallPluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                foreach (var plugin in plugins)
                { 
                    var pluginVM = new PluginViewModel(plugin);

                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        pluginVM.Delete(pathInstall);
                    }

                    var typeAction = pluginVM.Status == StatusChecked.NeedUpdated ? TypeAction.Update : TypeAction.Install;

                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, typeAction));

                    pluginVM.DownloadProgressChanged = DownloadProgressChanged;

# if DEBUG
                    Thread.Sleep(500);
#endif
                    pluginVM.Install(pathInstall);

                    m_progressInstallValue++;
                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, typeAction, true));
                }
#if DEBUG
                Thread.Sleep(500);
#endif
            });
        }

        public async Task DeletePluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                var test = Task.CurrentId.ToString();

                foreach (var plugin in plugins)
                {
                    var pluginVM = new PluginViewModel(plugin);

                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete));

                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        if (ProgressAction != null)
                            ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete));

                        pluginVM.Delete(pathInstall);
                    }

                    m_progressInstallValue++;
                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete, true));
# if DEBUG
                    //TEST
                    Thread.Sleep(500);
#endif
                }
            });
        }

        private void DownloadProgressChanged(int progress)
        {
        }
    }
}
