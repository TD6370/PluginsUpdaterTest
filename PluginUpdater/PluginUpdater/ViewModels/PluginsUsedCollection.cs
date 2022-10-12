using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PluginUpdater.ViewModels
{
    public class PluginsUsedCollection : Collection<PluginsUsed>
    {
        public PluginsUsedCollection()
        {
        }

        public bool IsNew(IPlugin plugin)
        {
            var pluginUsed = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID));
            return pluginUsed == null || pluginUsed.Version < plugin.Version;
            //return !IsExist(plugin);
        }

        public bool IsExist(IPlugin plugin)
        {
            var pluginUsed = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID) && p.Version.Equals(plugin.Version));
            return pluginUsed != null;
        }

        public void AddPlugin(IPlugin plugin)
        {
            Items.Add(new PluginsUsed()
            {
                ID = plugin.ID,
                Version = plugin.Version
            });
        }

        public void UpdateByCheckedChange(IEnumerable<IPlugin> plugins)
        {
            Items.Clear();
            plugins.Where(p=>p.Checked).ToList().ForEach(p => AddPlugin(p));
            Storage.Instance.SavePluginsUsed(this);
        }
    }
}
