using DotNetStandardEssentials.Configurations;
using DotNetStandardEssentials.Results;
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
        private T _configuration;
        private string _configFullPath;
        private IBackgroundHandler _backgroundHandler;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public LocalConfigurationManager(IBackgroundHandler backgroundHandler, ISettingsManager settings)
        {
            _backgroundHandler = backgroundHandler;
            _settings = settings;
        }

        #endregion

        #region Methods
        public async Task<Result<T>> LoadConfigurationAsync(string filename)
        {
            _configFullPath = Path.Combine(_settings.ConfigFileDirectory, filename);
            if (!_configFullPath.EndsWith(".json"))
            {
                throw new ArgumentException("file path is not a json file");
            }
            _backgroundHandler.Log($"**************************");
            _backgroundHandler.Log($"{_configFullPath} config file loaded");
            return await LoadFromJsonAsync(_configFullPath).ConfigureAwait(false);
        }

        public async Task<Result<T>> LoadConfigurationAsync(string pathToParentDirectory, string filename)
        {
            Result setDirectory = _settings.SetConfigFileDirectory(pathToParentDirectory);
            if (setDirectory.Failure)
            {
                return new Result<T>(setDirectory);
            }
            return await LoadConfigurationAsync(filename);
        }

        public void CreateDefaultConfiguration()
        {
            _configuration = new T();
        }

        public async Task<Result> SaveConfigurationAsync(string? configuration = default)
        {
            if (configuration != default)
            {
                string newPath = Path.Combine(_settings.ConfigFileDirectory, configuration);

                Result saveJson = await SaveToJsonAsync(newPath).ConfigureAwait(false);
                if (saveJson.Failure)
                {
                    return saveJson;
                }

                _settings.ConfigFile = configuration;
                _configFullPath = newPath;
                await _settings.SaveSettingsAsync().ConfigureAwait(false);
                return Result.Ok;
            }

            Result saveDefaultJson = await SaveToJsonAsync(_configFullPath).ConfigureAwait(false);
            if (saveDefaultJson.Failure)
            {
                return saveDefaultJson;
            }
            return Result.Ok;
        }

        private async Task<Result<T>> LoadFromJsonAsync(string fullPath)
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
                T? deserializedObject = JsonConvert.DeserializeObject<T>(jsonString);

                if (deserializedObject is null)
                {
                    string error = $"Failed to deserialize config for {_configuration.GetType().Name}";
                    return new Result<T>(new Exception(error));
                }

                _configuration = deserializedObject;
                return _configuration;
            }
            catch (Exception ex)
            {
                //CreateDefaultConfiguration();
                string errorString = $"Load configuration from json error: {ex.Message}";
                _backgroundHandler.Notify(errorString, GeneralMessageType.Error);
                return new Result<T>(new Exception(errorString));
            }
        }

        private async Task<Result> SaveToJsonAsync(string fullPath)
        {
            if (!fullPath.EndsWith(".json"))
            {
                fullPath = $"{fullPath}.json";
            }

            try
            {
                using StreamWriter file = File.CreateText(fullPath);
                if (_configuration == null)
                {
                    CreateDefaultConfiguration();
                }
                string serializedObject = JsonConvert.SerializeObject(_configuration);

                await file.WriteAsync(serializedObject).ConfigureAwait(false);
                return Result.Ok;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save configuration to json error: {ex.Message}");
                return new Result(ex);
            }
        }
        #endregion
    }
}
