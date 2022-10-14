using PluginUpdater.Engine;
using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PluginUpdater.ViewModels
{
    public class PluginsUsedCollection : Collection<PluginsUsed>
    {
        public PluginsUsedCollection()
        {
        }

        public bool IsNew(PluginViewModel plugin)
        {
            var pluginUsed = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID));
            return pluginUsed == null || pluginUsed.Version < plugin.Version;
        }

        public bool IsExist(PluginViewModel plugin)
        {
            //var pluginUsed = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID) && p.Version.Equals(plugin.Version));
            var pluginUsed = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID));
            return pluginUsed != null;
        }

        public void AddPlugin(PluginViewModel plugin)
        {
            Items.Add(new PluginsUsed()
            {
                ID = plugin.ID,
                Version = plugin.Version
            });
        }

        public void UpdateByCheckedChange(IEnumerable<PluginViewModel> plugins)
        {
            Items.Clear();
            plugins.Where(p=>p.Checked).ToList().ForEach(p => AddPlugin(p));
            Save();
        }

        public void Save()
        {
            Storage.Instance.SavePluginsUsed(this);
        }

        public void Completed(IPlugin plugin)
        {
            var pluginVM = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID));
            if (pluginVM != null)
                pluginVM.Version = plugin.Version;
            else
                AddPlugin(new PluginViewModel(plugin));
        }

        public void Completed(ProgressInfo progressInfo)
        {
            IPlugin plugin = progressInfo.Plagin;
            var pluginVM = Items.FirstOrDefault(p => p.ID.Equals(plugin.ID));
            if (progressInfo.ActionType == TypeAction.Delete)
            {
                Remove(pluginVM);
                return;
            }
            if (progressInfo.ActionType == TypeAction.Install ||
                progressInfo.ActionType == TypeAction.Update)
            {
                if (pluginVM != null)
                    pluginVM.Version = plugin.Version;
                else
                    AddPlugin(new PluginViewModel(plugin));
            }
        }
        
    }
}
