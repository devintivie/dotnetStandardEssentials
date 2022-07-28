using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public class ApplicationSettings
    {
        public string? LastConfigFile { get; set; }
        public string LastConfigPath { get; set; }
        public ConfigurationType ConfigurationType { get; set; }

        public ApplicationSettings(string? filename, string path, ConfigurationType type)
        {
            LastConfigFile = filename;
            LastConfigPath = path;
            ConfigurationType = type;
        }
    }
}
