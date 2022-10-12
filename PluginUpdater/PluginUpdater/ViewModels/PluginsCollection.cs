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
        public PluginsCollection()
        {

        }

        public PluginsCollection(List<Plugin> plugins)
        {
            foreach (var item in plugins)
            {
                Items.Add(item);
            }
        }

        public void CreatePluginsCollection<T>(List<T> plugins)
        {
            foreach (T item in plugins)
            {
                Items.Add(item as IPlugin);
            }
        }

        public static PluginsCollection Create<T>(List<T> plugins)
        {
            PluginsCollection collection = new PluginsCollection();
            foreach (T item in plugins)
            {
                collection.Add(item as IPlugin);
            }
            return collection;
        }

        private void CheckedWhenExist(PluginsUsed pluginUsed)
        {
            var item = Items.FirstOrDefault(p => p.ID.Equals(pluginUsed.ID) && p.Version.Equals(pluginUsed.Version));
            if (item != null)
                item.Checked = true;
        }

        public void UpdateByUsing(PluginsUsedCollection pluginsUsed)
        {
            pluginsUsed.ToList().ForEach(p => CheckedWhenExist(p));
        }
    }
}
