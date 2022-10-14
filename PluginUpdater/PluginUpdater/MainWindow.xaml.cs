using PluginUpdater.Engine;
using PluginUpdater.Models;
using PluginUpdater.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on MainApplication.InitApplication");
                MessageBox.Show(ex.Message, "Error on init ppplication", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var pluginVM = fe.DataContext as PluginViewModel;
            pluginVM.Checked = !pluginVM.Checked;
        }
    }

    public class BoolInverseConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return false;

            return !(bool)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
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
            var defaultColor = new SolidColorBrush(Colors.DarkGray);
            switch (result)
            {
                case StatusChecked.None:
                    return defaultColor;
                case StatusChecked.NeedDeleted:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEC7A74"));
                case StatusChecked.NeedUpdated:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECD074"));
                case StatusChecked.NeedInstall:
                    return new SolidColorBrush(Colors.LightGreen);
                default:
                    return defaultColor;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new Exception("The method or operation is not implemented.");
            return true;
        }
    }

    public class StatusResultToColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //-- title
            // <DataTrigger Binding="{Binding IsComleted}" Value="True">
            //    <Setter Property="Background" Value="LightGray"/>
            //</DataTrigger>
            //<DataTrigger Binding="{Binding IsComleted}" Value="False">
            //    <Setter Property="Background" Value="LightYellow"/>
            //</DataTrigger>
            //-- control
            // <DataTrigger Binding="{Binding IsComleted}" Value="True">
            //    <Setter Property="Background" Value="#FFB8B8B8"/>
            //</DataTrigger>
            //<DataTrigger Binding="{Binding IsComleted}" Value="False">
            //    <Setter Property="Background" Value="Yellow"/>
            //</DataTrigger>
            var par = parameter as string;
            bool isTitle = !string.IsNullOrEmpty(par) && par == "Title";

            TypeResult result = (TypeResult)value;
            var defaultColor = isTitle ? new SolidColorBrush(Colors.LightYellow): 
                new SolidColorBrush(Colors.Yellow);
            switch (result)
            {
                case TypeResult.None:
                    return defaultColor;
                case TypeResult.Cancel:
                    return isTitle ? new SolidColorBrush(Colors.LightSkyBlue) :
                                    new SolidColorBrush(Colors.LightBlue);
                case TypeResult.Comleted:
                    return isTitle ? new SolidColorBrush(Colors.LightGray) :
                                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB8B8B8"));
                case TypeResult.Fail:
                    return isTitle ? new SolidColorBrush(Colors.OrangeRed) :
                                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEC7A74"));
                default:
                    return defaultColor;
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
