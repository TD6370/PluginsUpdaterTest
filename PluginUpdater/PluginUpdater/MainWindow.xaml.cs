using PluginUpdater.Engine;
using PluginUpdater.Models;
using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

    public class StatusToColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StatusChecked result = (StatusChecked)value;
            switch (result)
            {
                case StatusChecked.None:
                    return new SolidColorBrush(Colors.Transparent);
                case StatusChecked.NeedDeleted:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEDE3"));
                case StatusChecked.NeedUpdated:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFEE3"));
                case StatusChecked.NeedInstall:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEFFE9"));

                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new Exception("The method or operation is not implemented.");
            return true;
        }
    }

    public class StatusToMessageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StatusChecked result = (StatusChecked)value;
            switch (result)
            {
                case StatusChecked.None:
                    return null;
                case StatusChecked.NeedDeleted:
                    return "удалить";
                case StatusChecked.NeedUpdated:
                    return "обновить"; 
                case StatusChecked.NeedInstall:
                    return "установить";
                default:
                    return null;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new Exception("The method or operation is not implemented.");
            return true;
        }
    }
}
