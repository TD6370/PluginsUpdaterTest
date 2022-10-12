using PluginUpdater.Models;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PluginUpdater.Engine
{
    public interface IPluginsLoader
    {
        public string URL { get; set; }
        public Task<PluginsCollection> LoadAsync();
    }

    public class PluginsLoaderTest : IPluginsLoader
    {
        public string URL { get; set; }

        public async Task<PluginsCollection> LoadAsync()
        {
            var plugins = new List<Plugin>();

            await Task.Run(() => {
                plugins = new List<Plugin>
                {
                    new Plugin { ID = "1", Version=1, DownloadLink ="http://depositfiles.com/files/zzk9c66om"},
                    new Plugin { ID = "2", Version=1, DownloadLink ="http://depositfiles.com/files/xj27n5lbf"},
                    new Plugin { ID = "3", Version=1, DownloadLink ="http://depositfiles.com/files/bm9hm8ci1"},
                    new Plugin { ID = "4", Version=1, DownloadLink ="http://depositfiles.com/files/skgshzxo5"}
                };

                Random rnd = new Random();
                foreach (var item in plugins)
                {
                    item.Version = rnd.Next(1, 3);
                }

                Storage.Instance.Save(plugins, Storage.Instance.GetPlaginTestURL());
            });

            return PluginsCollection.Create(plugins);
        }
    }

    public class PluginsLoader : IPluginsLoader
    {
        public string URL { get; set; }

        public async Task<PluginsCollection> LoadAsync()
        {
            var plugins = await LoadWebAsync();
            return PluginsCollection.Create(plugins);
        }

        private async Task<List<Plugin>> LoadWebAsync()
        {
            List<Plugin> plugins = await Storage.Instance.GetWebContentAsync<List<Plugin>>(URL);
            return plugins;
        }
    }
}
