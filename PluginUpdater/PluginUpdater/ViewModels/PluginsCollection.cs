using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PluginUpdater.ViewModels
{
    public class PluginsCollection : ObservableCollection<PluginViewModel>, INotifyCollectionChanged
    {
        private List<PluginsUsed> m_pluginsUsed;

        public PluginsCollection()
        {

        }

        public static PluginsCollection Create<T>(List<T> plugins)
        {
            PluginsCollection collection = new PluginsCollection();
            foreach (T plugin in plugins)
            {
                PluginViewModel pluginVM = new PluginViewModel(plugin as IPlugin);
                pluginVM.CheckedChanged = p => { collection.UpdateByChecked(pluginVM); };
                collection.Add(pluginVM);
            }
            return collection;
        }

        private void CheckedWhenExist(PluginsUsed pluginUsed)
        {
            var item = Items.FirstOrDefault(p => p.ID.Equals(pluginUsed.ID));
            if (item != null)
                item.Checked = true;
        }

        private void UpdateByChecked(PluginViewModel pluginVM)
        {
            pluginVM.Status = StatusChecked.None;
            var item = m_pluginsUsed?.FirstOrDefault(p => p.ID.Equals(pluginVM.ID));
            if (item != null)
            {
                if (!pluginVM.Checked)
                {
                    pluginVM.Status = StatusChecked.NeedDeleted;
                }
                else if (item.Version < pluginVM.Version)
                {
                    pluginVM.Status = StatusChecked.NeedUpdated;
                }
            }
            else if (pluginVM.Checked)
            {
                pluginVM.Status = StatusChecked.NeedInstall;
            }
            pluginVM.UpdateView();
        }

        public void UpdateByUsing(PluginsUsedCollection pluginsUsed)
        {
            m_pluginsUsed = pluginsUsed.ToList();
            pluginsUsed.ToList().ForEach(p => CheckedWhenExist(p));
        }
    }
}
