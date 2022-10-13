using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PluginUpdater.Engine
{
    public interface IPluginsInstaller
    {
        public string InstallPath { get; set; }

        public Action<ProgressInfo> ProgressAction { get; set; }

        public Task InstallPluginsAsync(IEnumerable<IPlugin> plugins);

        public Task DeletePluginsAsync(IEnumerable<IPlugin> plugins);
    }

    public class PluginsInstaller : IPluginsInstaller
    {
        public string InstallPath { get; set; }

        public Action<ProgressInfo> ProgressAction { get; set; }

        private int m_progressInstallValue;

        public async Task InstallPluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                foreach (var plugin in plugins)
                {
                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        plugin.Delete(pathInstall);
                    }

                    ProgressInfo.TypeAction typeAction = plugin.Status == StatusChecked.NeedUpdated ? ProgressInfo.TypeAction.Update : ProgressInfo.TypeAction.Install;

                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(plugin, m_progressInstallValue, typeAction));

                    plugin.DownloadProgressChanged = DownloadProgressChanged;

                    //TEST
                    Thread.Sleep(500);
                    //----------------------
                    plugin.Install(pathInstall);

                    m_progressInstallValue++;
                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(plugin, m_progressInstallValue, typeAction, true));
                }

                //TEST
                Thread.Sleep(500);
            });
        }

        public async Task DeletePluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                var test = Task.CurrentId.ToString();

                foreach (var plugin in plugins)
                {
                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(plugin, m_progressInstallValue, ProgressInfo.TypeAction.Delete));

                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        if (ProgressAction != null)
                            ProgressAction(new ProgressInfo(plugin, m_progressInstallValue, ProgressInfo.TypeAction.Delete));

                        plugin.Delete(pathInstall);
                    }

                    m_progressInstallValue++;
                    if (ProgressAction != null)
                        ProgressAction(new ProgressInfo(plugin, m_progressInstallValue, ProgressInfo.TypeAction.Delete, true));

                    //TEST
                    Thread.Sleep(500);
                }
            });
        }

        private void DownloadProgressChanged(int progress)
        {
        }
    }
}
