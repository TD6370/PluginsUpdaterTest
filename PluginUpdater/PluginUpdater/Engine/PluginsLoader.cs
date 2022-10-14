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
                    new Plugin { ID = "sldahasljdhwoqeoquwoeuqou12o3", Version=1, DownloadLink ="https://drive.google.com/uc?export=download&id=1KR500Zp-wlDNRp2HkkqXp0fAgAUim7sA"},
                    new Plugin { ID = "23402374o324l234hjljwlfjds", Version=1, DownloadLink ="https://drive.google.com/uc?export=download&id=1ldIiVMzvvNvFUtOt4Z8wTAIfVw4vYGkj"},
                    new Plugin { ID = "2304923842h34jlk23h4o24230", Version=1, DownloadLink ="https://drive.google.com/uc?export=download&id=1P3FQmxfl6kUrLjm4wbnBOC09jp4LulpR"},
                    new Plugin { ID = "34092834234jk2nsdanfldf", Version=1, DownloadLink ="https://drive.google.com/uc?export=download&id=15bQbgQEodC0Cro4y1CTWhbVdLX2V_qZ-"},
                    new Plugin { ID = "3249g2348324j324jl23j4klj", Version=1, DownloadLink ="https://drive.google.com/uc?export=download&id=1QXYtJr9OLC4ysYRZZBvKnf8ZK5ngA8ym"}
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
            List<Plugin> plugins = await Task.Run(() =>
            {
                return Storage.Instance.GetWebContentAsync<List<Plugin>>(URL);
            });
            return plugins;
        }
    }
}
