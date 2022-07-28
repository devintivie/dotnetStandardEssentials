using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetStandardEssentials
{
    /// <summary>
    /// Interface to manage application level runtime settings
    /// </summary>
    public interface ISettingsManager
    {
        string ConfigFileExt { get; }
        string ConfigFile { get; set; }
        string ConfigFileDirectory { get; }
        ConfigurationType ConfigType { get; set; }
        ApplicationPlatform Platform { get; }

        /// <summary>
        /// Update <see cref="ConfigFileDirectory"/>
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>Returns true if directory exists and <see cref="ConfigFileDirectory"/> has been updated</returns>
        bool SetConfigFileDirectory(string directoryPath);

        /// <summary>
        /// Find all config files that can be used from current <see cref="ConfigFileDirectory"/>
        /// </summary>
        /// <returns></returns>
        List<string> FindLoadableConfigFiles();

        /// <summary>
        /// Asynchronously delete ConfigFile from OS
        /// </summary>
        /// <param name="configurationName">Configuration filename minus extension</param>
        /// <returns></returns>
        Task DeleteConfigFileAsync(string configName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task DeleteAllConfigFilesInConfigFileDirectoryAsync();

        /// <summary>
        /// Get Previously used <see cref="ApplicationSettings"/> to be loaded again
        /// </summary>
        /// <returns></returns>
        Task<string> GetPreviousConfigurationAsync();// bool freshStart = false);


        Task<string> UseConfigFile(string filename);
        //Task SaveSettings(string filename);

        /// <summary>
        /// Shortcut for saving settings on any platform
        /// </summary>
        /// <returns></returns>
        Task SaveSettingsAsync();
    }
}
