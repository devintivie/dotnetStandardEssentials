using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials
{
    /// <summary>
    /// Save and load local settings files. 
    /// Implements <see cref="ISettingsManager"/>
    /// </summary>
    /// <remarks>
    /// Built or loaded before Root viewmodel/>.
    /// </remarks>
    public class SettingsManager : ISettingsManager
    {
        #region Fields
        private const string SETTINGS_FILENAME = "settings.json";
        private const string DEFAULT_CONFIG_EXT = ".json";
        private const string DEFAULT_CONFIG_FILE = "";
        private IBackgroundHandler _backgroundHandler;
        #endregion

        #region Properties
        public ApplicationPlatform Platform { get; private set; }
        public string ConfigFileDirectory { get; private set; }
        public string ConfigFileExt { get; private set; } = DEFAULT_CONFIG_EXT;
        public string ConfigFile { get; set; } = DEFAULT_CONFIG_FILE;
        public ConfigurationType ConfigType { get; set; } = ConfigurationType.LocalJSON;
        public List<string> LoadableConfigFiles { get; set; } = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates <see cref="SettingsManager"/>.
        /// Constructs using <paramref name="backgroundHandler"/>
        /// </summary>
        /// <param name="platform">Required: Selected <see cref="ApplicationPlatform"/> </param>
        /// <param name="backgroundHandler">Required: Dependency Injection for <see cref="IBackgroundHandler" /></param>
        /// <param name="configFileExtension">Optional: set file suffix</param>
        public SettingsManager(ApplicationPlatform platform
            , IBackgroundHandler backgroundHandler
            , string configFileExtension = DEFAULT_CONFIG_EXT)
        {
            Platform = platform;
            _backgroundHandler = backgroundHandler;
            ConfigFileExt = configFileExtension;
            SetDefaultBasePath();
        }

        /// <summary>
        /// Sets the base address for setting(s) files
        /// </summary>
        private void SetDefaultBasePath()
        {
            //switch (_platform)
            string settingsDirectory;
            switch (Platform)
            {
                case ApplicationPlatform.WPF:
                    settingsDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                case ApplicationPlatform.Xamarin:
                    settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    break;
                case ApplicationPlatform.ASPnet:
                    //Still in testing, use local config file for now
                    settingsDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                default:
                    settingsDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    break;
            }

            //Set default config file directory to same directory that holds application settings file
            ConfigFileDirectory = settingsDirectory;
        }

        public bool SetConfigFileDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                ConfigFileDirectory = directoryPath;
                return true;
            }

            return false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load settings file from filepath
        /// </summary>
        /// <param name="fullPath">OS full file path</param>
        /// <returns></returns>
        private async Task<ApplicationSettings> GetSettingsAsync(string fullPath)
        {
            ApplicationSettings settings;

            if (Platform == ApplicationPlatform.WPF)
            {
                string filePath = new FileInfo(fullPath).FullName;
                //Read file into string
                string jsonString = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);

                //Create ApplicationSettings
                settings = JsonConvert.DeserializeObject<ApplicationSettings>(jsonString);
                return settings;
            }

            //Platform is  Mobile
            else
            {
                string jsonString = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
                try
                {
                    //Create ApplicationSettings
                    settings = JsonConvert.DeserializeObject<ApplicationSettings>(jsonString);
                    return settings;
                }
                catch (Exception)
                {
                    return new ApplicationSettings(null, ConfigFileDirectory, ConfigurationType.LocalJSON);
                }
            }
        }

        public async Task<string> GetPreviousConfigurationAsync()//bool startFresh = false)
        {
            ApplicationSettings settings;
            string fullPath = Path.Combine(ConfigFileDirectory, SETTINGS_FILENAME);

            //Try to load settings from path, if unable, create a new settings file and save it 
            try
            {
                settings = await GetSettingsAsync(fullPath).ConfigureAwait(false);
                SetParametersFromApplicationSettings(settings);
            }
            catch (Exception ex)
                when (ex is FileNotFoundException 
                    || ex is JsonReaderException 
                    || ex is NullReferenceException)
            {
                await SaveSettingsAsync().ConfigureAwait(false);

                settings = await GetSettingsAsync(fullPath).ConfigureAwait(false);
                SetParametersFromApplicationSettings(settings);
            }

            //Move this to another method, was used for debugging
            //if (startFresh)
            //{
            //    var files = FindLoadableConfigFiles();
            //    foreach (var file in files)
            //    {
            //        await DeleteConfigFileAsync(file).ConfigureAwait(false);
            //    }

            //}

            if (ConfigFile != null)
            {
                CheckConfigFile(ConfigFile);
            }

            return ConfigFile;
        }

        /// <summary>
        /// Delete all config files in established <see cref="ConfigFileDirectory"/>
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllConfigFilesInConfigFileDirectoryAsync()
        {
            IEnumerable<string> files = FindLoadableConfigFiles();
            foreach (string file in files)
            {
                await DeleteConfigFileAsync(file).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Likely deprecated, need to check
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Task<string> UseConfigFile(string filename)
        {
            ConfigFile = filename ?? DEFAULT_CONFIG_FILE;
            CheckConfigFile(ConfigFile);
            return Task.FromResult(ConfigFile);
        }

        /// <summary>
        /// Check that config file exists
        /// </summary>
        private void CheckConfigFile(string configFile)
        {
            IEnumerable<string> files = FindLoadableConfigFiles();

            if (!files.Any(configFile.Contains))
            {
                _backgroundHandler.Notify($"Config File {ConfigFile} not found", GeneralMessageType.Warning);
                ConfigFile = DEFAULT_CONFIG_FILE;
            }
        }

        /// <summary>
        /// Copy application file directory to Config File properties/>
        /// </summary>
        /// <param name="settings">Loaded ApplicationSettings</param>
        private void SetParametersFromApplicationSettings(ApplicationSettings settings)
        {
            ConfigFile = settings.LastConfigFile ?? DEFAULT_CONFIG_FILE;
            ConfigType = settings.ConfigurationType;

            //If, for some reason, config path is null upon loading, default to the settings base path
            //ConfigFileDirectory = ConfigFileDirectory;// state.LastConfigPath ?? _settingsBasePath;
        }


        /// <summary>
        /// Method for saving <see cref="ApplicationSettings" for desktop application/>
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettingsDesktopAsync()//string filename)
        {
            string path = Path.Combine(ConfigFileDirectory, SETTINGS_FILENAME);
            string filePath = new FileInfo(path).FullName;

            var applicationSettings = new ApplicationSettings(ConfigFile, ConfigFileDirectory, ConfigType);

            using (StreamWriter file = File.CreateText(filePath))
            {
                string serialized = JsonConvert.SerializeObject(applicationSettings);
                await file.WriteAsync(serialized).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Method for saving <see cref="ApplicationSettings" for mobile application/>
        /// </summary>
        /// <returns></returns>
        private async Task SaveSettingsMobileAsync()
        {
            string path = Path.Combine(ConfigFileDirectory, SETTINGS_FILENAME);
            ApplicationSettings settings = new ApplicationSettings(ConfigFile, ConfigFileDirectory, ConfigType);

            //Asynchronously write object to file
            using (StreamWriter file = File.CreateText(path))
            {
                string serialized = JsonConvert.SerializeObject(settings);
                await file.WriteAsync(serialized).ConfigureAwait(false);
            }
        }


        public async Task SaveSettingsAsync()
        {
            switch (Platform)
            {
                case ApplicationPlatform.WPF:
                    await SaveSettingsDesktopAsync().ConfigureAwait(false);
                    break;
                case ApplicationPlatform.Xamarin:
                    await SaveSettingsMobileAsync().ConfigureAwait(false);
                    break;
                case ApplicationPlatform.ASPnet:
                    break;
                default:
                    break;
            }
        }

        #endregion

        public List<string> FindLoadableConfigFiles()
        {
            try
            {
                // Find all files with expected file extension
                string[] filenames = Directory.GetFiles(ConfigFileDirectory, $"*{ConfigFileExt}");

                // Get rid of file extension for each file
                LoadableConfigFiles = filenames.Select(fn => Path.GetFileNameWithoutExtension(fn)).ToList();
                // Check to see if the files should be ignored
                LoadableConfigFiles.RemoveAll(PrivateJsonFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return LoadableConfigFiles;
        }


        public async Task DeleteConfigFileAsync(string configurationName)
        {
            string filename = $"{configurationName}{ConfigFileExt}";
            string fullPath = Path.Combine(ConfigFileDirectory, filename);
            File.Delete(fullPath);

            if (ConfigFile == configurationName)
            {
                ConfigFile = null;
                await SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns true if filename is considered private
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool PrivateJsonFile(string file)
        {
            if (file.Equals("settings"))
            {
                return true;
            }
            string friendlyName = AppDomain.CurrentDomain.FriendlyName;

            if (file.Contains(friendlyName))
            {
                return true;
            }
            return false;
        }

    }
}
