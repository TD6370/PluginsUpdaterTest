using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PluginUpdater.ViewModels
{
    
    public class PluginsCollection : ObservableCollection<IPlugin>, INotifyCollectionChanged
    {
        private List<PluginsUsed> m_pluginsUsed;

        public PluginsCollection()
        {

        }

        public void CreatePluginsCollection<T>(List<T> plugins)
        {
            foreach (T item in plugins)
            {
                IPlugin plugin = item as IPlugin;
                plugin.CheckedChanged = p => { UpdateByChecked(p); };
                Items.Add(plugin);
            }
        }

        public static PluginsCollection Create<T>(List<T> plugins)
        {
            PluginsCollection collection = new PluginsCollection();
            foreach (T item in plugins)
            {
                IPlugin plugin = item as IPlugin;
                plugin.CheckedChanged = p => { collection.UpdateByChecked(p); };
                collection.Add(plugin);
            }
            return collection;
        }

        private void CheckedWhenExist(PluginsUsed pluginUsed)
        {
            var item = Items.FirstOrDefault(p => p.ID.Equals(pluginUsed.ID));
            if (item != null)
                item.Checked = true;
        }

        private void UpdateByChecked(IPlugin pluginChecked)
        {
            if (m_pluginsUsed == null)
                return;

            pluginChecked.Status = StatusChecked.None;

            var item = m_pluginsUsed.FirstOrDefault(p => p.ID.Equals(pluginChecked.ID));
            if (item != null)
            {
                if (!pluginChecked.Checked)
                {
                    pluginChecked.Status = StatusChecked.NeedDeleted;
                }
                else if (item.Version < pluginChecked.Version)
                {
                    pluginChecked.Status = StatusChecked.NeedUpdated;
                }
            }
            else if (pluginChecked.Checked)
            {
                pluginChecked.Status = StatusChecked.NeedInstall;
            }
            pluginChecked.UpdateView();
        }

        public void UpdateByUsing(PluginsUsedCollection pluginsUsed)
        {
            m_pluginsUsed = pluginsUsed.ToList();
            pluginsUsed.ToList().ForEach(p => CheckedWhenExist(p));
        }
    }
}
