using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetStandardEssentials.Configurations
{
    public interface IConfigManager<T> where T : IConfiguration
    {
        T Configuration { get; }

        void CreateDefaultConfiguration();

        /// <summary>
        /// Load configuration from file at current configuration file directory
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task LoadConfigurationAsync(string filename);

        /// <summary>
        /// Load configuration from file at specified path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task LoadConfigurationAsync(string pathToParentDirectory, string filename);

        /// <summary>
        /// Saves configuration to current configuarion file directory.
        /// Saves as json file.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Task SaveConfigurationAsync(string? configurationName = default);
    }
}
