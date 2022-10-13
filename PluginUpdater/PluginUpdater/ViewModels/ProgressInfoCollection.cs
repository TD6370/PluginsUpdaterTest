using PluginUpdater.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PluginUpdater.ViewModels
{
    public class ProgressInfoCollection : ObservableCollection<ProgressInfo>, INotifyCollectionChanged
    {
        public void Update(ProgressInfo progressInfo)
        {
            var progressItem = Items.FirstOrDefault(p => p.Plagin.ID.Equals(progressInfo.Plagin.ID));
            if (progressItem == null)
                Insert(0, progressInfo);
            else
                progressItem.Copy(progressInfo);
        }
    }
}
