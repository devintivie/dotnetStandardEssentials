using DotNetStandardEssentials.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials
{
    /// <summary>
    /// Loads from local JSON configuration files.
    /// Loading information from a database is not done here.
    /// </summary>
    /// <inheritdoc/>
    public class LocalConfigurationManager<T> : IConfigManager<T> where T : Configuration, new()
    {
        #region Fields
        private ISettingsManager _settings;
        private T Configuration;
        private string _configFullPath;
        private IBackgroundHandler _backgroundHandler;
        #endregion

        #region Properties
        T IConfigManager<T>.Configuration => Configuration;

        #endregion

        #region Constructors
        public LocalConfigurationManager(IBackgroundHandler backgroundHandler, ISettingsManager settings)
        {
            _backgroundHandler = backgroundHandler;
            _settings = settings;
        }

        #endregion

        #region Methods
        public async Task LoadConfigurationAsync(string filename)
        {
            _configFullPath = Path.Combine(_settings.ConfigFileDirectory, filename);
            if (!_configFullPath.EndsWith(".json"))
            {
                throw new ArgumentException("file path is not a json file");
            }
            _backgroundHandler.Log($"**************************");
            _backgroundHandler.Log($"{_configFullPath} config file loaded");
            await LoadFromJsonAsync(_configFullPath).ConfigureAwait(false);
        }

        public async Task LoadConfigurationAsync(string pathToParentDirectory, string filename)
        {
            if (_settings.SetConfigFileDirectory(pathToParentDirectory))
            {
                await LoadConfigurationAsync(filename);
            }
            //_configFullPath = Path.Combine(_settings.ConfigFileDirectory, filename);
            //if (!_configFullPath.EndsWith(".json"))
            //{
            //    throw new ArgumentException("file path is not a json file");
            //}
            //_backgroundHandler.Log($"**************************");
            //_backgroundHandler.Log($"{_configFullPath} config file loaded");
            //await LoadFromJson(_configFullPath).ConfigureAwait(false);
        }

        public void CreateDefaultConfiguration()
        {
            Configuration = new T();
        }

        public async Task SaveConfigurationAsync(string? configuration = default)
        {
            if (configuration != default)
            {
                string newPath = Path.Combine(_settings.ConfigFileDirectory, configuration);

                bool success = await SaveToJsonAsync(newPath).ConfigureAwait(false);
                if (success)
                {
                    _settings.ConfigFile = configuration;
                    _configFullPath = newPath;
                    await _settings.SaveSettingsAsync().ConfigureAwait(false);
                }
            }
            else
            {
                bool success = await SaveToJsonAsync(_configFullPath).ConfigureAwait(false);
                if (success)
                {
                    Console.WriteLine("Save successful");
                }
                else
                {
                    Console.WriteLine("Save unsuccessful");
                }
            }
        }

        private async Task LoadFromJsonAsync(string fullPath)
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
                T deserializedObject = JsonConvert.DeserializeObject<T>(jsonString);
                Configuration = deserializedObject;
            }
            catch (Exception ex)
            {
                CreateDefaultConfiguration();
                string errorString = $"Load configuration from json error: {ex.Message}";
                Console.WriteLine(errorString);
                _backgroundHandler.Notify(errorString, GeneralMessageType.Error);
            }
        }

        private async Task<bool> SaveToJsonAsync(string fullPath)
        {
            if (!fullPath.EndsWith(".json"))
            {
                fullPath = $"{fullPath}.json";
            }

            try
            {
                using (var file = File.CreateText(fullPath))
                {
                    if (Configuration == null)
                    {
                        CreateDefaultConfiguration();
                    }
                    string serializedObject = JsonConvert.SerializeObject(Configuration);

                    await file.WriteAsync(serializedObject).ConfigureAwait(false);
                }

                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save configuration to json error: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
