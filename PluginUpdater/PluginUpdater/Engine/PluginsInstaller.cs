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

        public event EventHandler<IProgressInfo> ProgressEvent;

        public Task InstallPluginsAsync(IEnumerable<IPlugin> plugins);

        public Task DeletePluginsAsync(IEnumerable<IPlugin> plugins);

        public void Cancel();
    }

    public class PluginsInstaller : IPluginsInstaller
    {
        private int m_progressInstallValue;

        public event EventHandler<IProgressInfo> ProgressEvent;

        public string InstallPath { get; set; }

        public Action<IProgressInfo> ProgressAction { get; set; }

        CancellationTokenSource m_cancelTokenSource;
        private CancellationToken m_cancelToken;
        private Object m_cancelLock;

        public PluginsInstaller()
        {
            m_cancelTokenSource = new CancellationTokenSource();
            m_cancelToken = m_cancelTokenSource.Token;
        }

        public void Cancel()
        {
            m_cancelTokenSource.Cancel();
        }

        public async Task InstallPluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                foreach (var plugin in plugins)
                { 
                    var pluginVM = new PluginViewModel(plugin);
                    var typeAction = pluginVM.Status == StatusChecked.NeedUpdated ? TypeAction.Update : TypeAction.Install;

                    if (m_cancelToken.IsCancellationRequested)
                    {
                        OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, typeAction, TypeResult.Cancel));
                        return;
                    }

                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    if (isExist)
                    {
                        pluginVM.Delete(pathInstall);
                    }

                    OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, typeAction));
                    pluginVM.DownloadProgressChanged = DownloadProgressChanged;

# if DEBUG
                    Thread.Sleep(500);
                    //Thread.Sleep(2000);
#endif
                    var result = pluginVM.Install(pathInstall);
                    bool isFail = !string.IsNullOrEmpty(result);
                    TypeResult statusResult = isFail ? TypeResult.Fail : TypeResult.Comleted;

                    m_progressInstallValue++;
                    OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, typeAction, statusResult));
                }
#if DEBUG
                Thread.Sleep(500);
#endif
            });
        }

        private void OnProgressChanged(IProgressInfo progressInfo)
        {
            EventHandler<IProgressInfo> handler = ProgressEvent;
            if (handler != null)
            {
                handler(this, progressInfo);
            }
        }

        public async Task DeletePluginsAsync(IEnumerable<IPlugin> plugins)
        {
            await Task.Run(() =>
            {
                var test = Task.CurrentId.ToString();

                foreach (var plugin in plugins)
                {
                    var pluginVM = new PluginViewModel(plugin);

                    if (m_cancelToken.IsCancellationRequested)
                    {
                        OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete, TypeResult.Cancel));
                        return;
                    }
                    OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete));

                    string pathInstall = string.Concat(InstallPath, $"\\{plugin.ID}");
                    bool isExist = Storage.Instance.CheckDirectory(pathInstall);
                    bool isFail = false;
                    if (isExist)
                    {
                        if (ProgressAction != null)
                            ProgressAction(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete));

                        var result = pluginVM.Delete(pathInstall);
                        isFail = !string.IsNullOrEmpty(result);
                    }
                    TypeResult statusResult = isFail ? TypeResult.Fail : TypeResult.Comleted;

                    m_progressInstallValue++;
                    OnProgressChanged(new ProgressInfo(pluginVM, m_progressInstallValue, TypeAction.Delete, statusResult));
# if DEBUG
                    //TEST
                    Thread.Sleep(500);
                    //Thread.Sleep(2000);
#endif
                }
            });
        }

        private void DownloadProgressChanged(int progress)
        {
        }
    }
}
