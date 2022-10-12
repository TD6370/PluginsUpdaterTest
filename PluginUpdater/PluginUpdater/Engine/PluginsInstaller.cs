using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginUpdater.Engine
{
    public interface IPluginsInstaller
    {
        public string InstallPath { get; set; }
        public Action<int> ProgressActtion { get; set; }

        public Task InstallPluginsAsync(IEnumerable<IPlugin> plugins);

        public Task DeletePluginsAsync(IEnumerable<IPlugin> plugins);
    }

    public class PluginsInstaller : IPluginsInstaller
    {
        public string InstallPath { get; set; }

        public Action<int> ProgressActtion { get; set; }

        private int m_progressInstallValue;

        public async Task InstallPluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                foreach (var plugin in plugins)
                {
                    string fileName = plugin.GetNameFile();
                    string pathInstallFile = string.Concat(InstallPath, $"\\{plugin.ID}\\{fileName}");

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

                    m_progressInstallValue++;
                    if (ProgressActtion != null)
                        ProgressActtion(m_progressInstallValue);
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
                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                        plugin.Delete(pathInstall);

                    m_progressInstallValue++;
                    if (ProgressActtion != null)
                        ProgressActtion(m_progressInstallValue);

                    //TEST
                    Thread.Sleep(500);
                }
            });
        }


        private void DownloadProgressChanged(int progress)
        {
            return;
            if (ProgressActtion != null)
                ProgressActtion(progress);
        }
    }
}
