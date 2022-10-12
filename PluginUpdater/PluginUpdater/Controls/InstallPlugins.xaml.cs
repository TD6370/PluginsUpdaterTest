using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PluginUpdater.Controls
{
    /// <summary>
    /// Interaction logic for InstallPlugins.xaml
    /// </summary>
    public partial class InstallPlugins : Window
    {
        public InstallPlugins()
        {
            InitializeComponent();

            Loaded += LoadedChange;
        }

        private InstallPluginsViewModel ViewModel
        {
            get { return DataContext as InstallPluginsViewModel; }
        }

        private void LoadedChange(object sender, RoutedEventArgs args)
        {
            ViewModel.StartInstall();
        }
    }
}
