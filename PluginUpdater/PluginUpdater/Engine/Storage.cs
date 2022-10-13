using PluginUpdater.Models;
using PluginUpdater.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace PluginUpdater.Engine
{
    class Storage
    {
        public string PathAppExe => Process.GetCurrentProcess().MainModule.FileName;
        public string PathApp => Environment.CurrentDirectory;
        public string PluginConfigFileName => "PluginConfig.xml";
        public string PluginUsedFileName => "PluginUsed.xml";
        public string FolderInstall => "PluginInstall";
        public string LogFileName => "Log.txt";
        public string TestPluginsFileName => "TestPlugins.xml";
        public string PathPluginConfig => GetNewPath(PluginConfigFileName);
        public string PathPluginInstall => GetNewPath(FolderInstall);
        public string PathPluginUsed => GetNewPath(PluginUsedFileName);
        public string PluginApplicationOwnerPathDefault => GetNewPath("TestApp.exe");
        public string LogPath => GetNewPath(LogFileName);
        
        private static Storage m_storage;
        public static Storage Instance
        {
            get
            {
                if (m_storage == null)
                    m_storage = new Storage();
                return m_storage;
            }
        }

        public string GetPathPluginInstall()
        {
            if (!Directory.Exists(PathPluginInstall))
                Directory.CreateDirectory(PathPluginInstall);
            return PathPluginInstall;
        }

        public string GetNewPath(string subPath)
        {
            return string.Concat(PathApp,"\\", subPath);
        }

        public string GetPlaginTestURL()
        {
            return GetNewPath(TestPluginsFileName);
        }

        public async Task<T> GetWebContentAsync<T>(string url)
        {
            T data;
            try
            {
                var webRequest = WebRequest.Create(url);
                webRequest.ContentType = "application/json";
                webRequest.Method = "GET";
                var response = await webRequest.GetResponseAsync();
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return default;

                    data = DeserializeObject<T>(dataStream);
                }
                return data;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Stages.GetWebContentAsync", $"url={url}");
                MessageBox.Show($"{ex.Message}\n\nUrl: {url}", $"Error on Get web request!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return default;
        }

        public T DeserializeObject<T>(Stream stream)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            return (T)deserializer.Deserialize(stream);
        }


        public async Task<PluginsConfig> LoadPluginsConfigAsync()
        {
            PluginsConfig config = null;
            try
            {
                string path = PathPluginConfig;
                if (!File.Exists(path))
                    return config;
                config = await ReadFromFile<PluginsConfig>(path);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Storage.LoadPluginsConfig");
                MessageBox.Show(ex.Message, "Error on Load plugins config", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return config;
        }

        public async Task<PluginsUsedCollection> LoadPluginsUsed()
        {
            PluginsUsedCollection pluginsUsed = null;
            try
            {
                string path = PathPluginUsed;
                if (!File.Exists(path))
                    return pluginsUsed;
                pluginsUsed = await ReadFromFile<PluginsUsedCollection>(path);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Storage.LoadPluginsUsed");
                MessageBox.Show(ex.Message, "Error on Load plugins used", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return pluginsUsed;
        }

        public Task<T> ReadFromFile<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;
              
                var mySerializer = new XmlSerializer(typeof(T));
                using (var myFileStream = new FileStream(path, FileMode.Open))
                {
                    var result = (T)mySerializer.Deserialize(myFileStream);
                    return Task.FromResult(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on ReadFromFile", $"Path: {path}");
                MessageBox.Show($"Error on Read File\n{path}\n{ex.Message}", "Error on Read File", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void Save<T>(T objectSave, string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    serializer.Serialize(writer, objectSave);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Save", $"path={path}");
                MessageBox.Show(ex.Message, $"Error on Saving file: {path}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SavePluginsUsed(PluginsUsedCollection objectSave)
        {
            string path = PathPluginUsed;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginsUsedCollection));
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    serializer.Serialize(writer, objectSave);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Save", $"path={path}");
                MessageBox.Show(ex.Message, $"Error on Saving file: {path}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DownloadFile(string url, ref string path, Action<int> DownloadProgressChanged)
        {
            try
            {
                string pathFolder = path;

                if (!Directory.Exists(pathFolder))
                    Directory.CreateDirectory(pathFolder);

                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        if (DownloadProgressChanged != null)
                            DownloadProgressChanged(e.ProgressPercentage);
                    };
                    client.OpenRead(url);
                    string header_contentDisposition = client.ResponseHeaders["content-disposition"];
                    if(header_contentDisposition == null)
                    {
                        Logger.Debug($"Error on DownloadFile: Not found file name by {url} !");
                        return;
                    }
                    string filename = new ContentDisposition(header_contentDisposition).FileName;
                    path = string.Concat(pathFolder, "\\", filename);
                    client.DownloadFile(new Uri(url), path);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on DownloadFile", $"url={url} path={path}");
                throw;
            }
        }

        public bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }

        public void DeleteDerictory(string path)
        {
            Directory.Delete(path, true);
        }

        public void RunApplication(string path, string args = null)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                    }
                };
                process.Start();
            } 
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on Run application", $"path={path}");
                MessageBox.Show("Error on Run application: {path}", "Error on Run application", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

