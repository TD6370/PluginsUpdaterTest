using PluginUpdater.Engine;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PluginUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainApplication : Window
    {
        ApplicationViewModel m_applicationViewModel;

        public MainApplication()
        {
            InitializeComponent();

            InitApplication();
        }

        private void InitApplication()
        {
            try
            {
                m_applicationViewModel = new ApplicationViewModel();
                DataContext = m_applicationViewModel;
                //m_applicationViewModel.LoadPlugins();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on MainApplication.InitApplication");
                MessageBox.Show(ex.Message, "Error on init ppplication", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Visibility.Collapsed;

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Visibility.Visible;

            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
