using PluginUpdater.Engine;
using PluginUpdater.Models;
using PluginUpdater.Controls;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace PluginUpdater.ViewModels
{
    public class ApplicationViewModel : BaseNotifyPropertyChanged
    {
        private PluginsConfig m_config;
        private PluginsUsedCollection m_pluginsUsed;

        private PluginsCollection m_plugins;
        public PluginsCollection Plugins
        {
            get { return m_plugins; }
            set
            {
                m_plugins = value;
                OnPropertyChanged(nameof(Plugins));
            }
        }

        private Visibility m_applicationVisibility = Visibility.Visible;
        public Visibility ApplicationVisibility
        {
            get { return m_applicationVisibility; }
            set
            {
                m_applicationVisibility = value;
                OnPropertyChanged(nameof(ApplicationVisibility));
            }
        }

        private bool m_isProgress = true;
        public bool IsProgress
        {
            get { return m_isProgress; }
            set
            {
                m_isProgress = value;
                OnPropertyChanged(nameof(IsProgress));
            }
        }

        private const string m_waitMessage = "Ожидание данных...";
        private string m_warningMessage = m_waitMessage;
        public string WarningMessage
        {
            get { return m_warningMessage; }
            set
            {
                m_warningMessage = value;
                OnPropertyChanged(nameof(WarningMessage));
                OnPropertyChanged(nameof(IsFail));
            }
        }

        public bool IsFail
        {
            get { return !string.IsNullOrEmpty(WarningMessage) &&
                    !WarningMessage.Equals(m_waitMessage); }
        }

        public ApplicationViewModel()
        {
            //---------------- test xml file plugens
            //LoadPlugins(new PluginsLoaderTest());
            //---------------- work
            LoadPlugins(new PluginsLoader());
        }

        public async Task LoadPlugins(IPluginsLoader pluginsLoader)
        {
            IsProgress = true;
            bool isValid = true;

            try
            {
                m_config = await Storage.Instance.LoadPluginsConfigAsync();
                if (m_config == null)
                    m_config = PluginsConfig.GetDefault();

                string ivalidMessage = m_config.IvalidMessage();
                isValid = string.IsNullOrEmpty(ivalidMessage);
                if (!isValid)
                {
                    WarningMessage = ivalidMessage;
                    return;
                }
                pluginsLoader.URL = m_config.PluginsURL;
                Plugins = await pluginsLoader.LoadAsync();

                m_pluginsUsed = await Storage.Instance.LoadPluginsUsed();
                if (m_pluginsUsed == null)
                    m_pluginsUsed = new PluginsUsedCollection();
                else
                    m_plugins.UpdateByUsing(m_pluginsUsed);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Error on LoadPlugins");
                MessageBox.Show(ex.Message, "Error on Load plugins", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (isValid)
                {
                    IsProgress = false;
                    WarningMessage = "";
                }
            }
        }

        public RelayCommand ApplayCommand
        {
            get
            {
                return new RelayCommand(obj => 
                { 
                    RunInstallPlugins(new PluginsInstaller()); 
                });
            }
        }

        private void RunInstallPlugins(IPluginsInstaller pluginsInstaller)
        {
            try
            {
                InstallPluginsViewModel installVM = new InstallPluginsViewModel(m_plugins, m_pluginsUsed, m_config.PluginsInstallPath, pluginsInstaller);
                pluginsInstaller.ProgressEvent += (obj, progressInfo) => { ProgressInstallChange(progressInfo); };


                if (!installVM.IsInstallValid())
                {
                    MessageBox.Show("Отсутсвуют плагины новыйх версий!", "Установка плагинов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                var installControl = new InstallPluginsDialog();
                installVM.CloseAction = () => { installControl.Close(); };
                installControl.DataContext = installVM;
                SwithVisible();

                installControl.ShowDialog();

                m_pluginsUsed.Save();

                if (installVM.IsCompleted)
                {
                    Storage.Instance.RunApplication(m_config.PluginApplicationOwnerPath);
                    Exit();
                }
                else
                {
                    SwithVisible();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on ApplicationViewModel.RunInstallPlugins");
                MessageBox.Show(ex.Message, "Error on Install plugins", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProgressInstallChange(IProgressInfo _progressInfo)
        {
            ProgressInfo progressInfo = _progressInfo as ProgressInfo;
            WpfHelper.InvokeMethod(() =>
            {
                switch (progressInfo.StatusResult)
                {
                    case TypeResult.Comleted:
                        m_pluginsUsed.Completed(progressInfo);
                        Plugins.Completed(progressInfo.Plagin);
                        break;
                    case TypeResult.Fail:
                        var message = progressInfo.ErrorMessage;
                        break;
                    case TypeResult.Cancel:
                        break;
                }
            });
        }


        private void SwithVisible()
        {
            ApplicationVisibility = ApplicationVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
