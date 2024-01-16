#include "LoaderConfig.h"
#include <string>
#include <filesystem>
#include <fstream>

#include "Config.h"

namespace preloader
{
    using json = nlohmann::json;

    void to_json(json& j, const ConfigFile& c) {
        j = json{
            {"logfile", c.LogFile},
            {"logcmd", c.LogCmd},
            {"logLevel", c.LogLevel},
            {"outputEveryPath", c.OutputEveryPath},
            {"enablePluginLoader", c.EnablePluginLoader}
        };
    }

    void from_json(const json& j, ConfigFile& c) {
        j.at("logfile").get_to(c.LogFile);
        j.at("logcmd").get_to(c.LogCmd);
        j.at("logLevel").get_to(c.LogLevel);
        j.at("outputEveryPath").get_to(c.OutputEveryPath);
        j.at("enablePluginLoader").get_to(c.EnablePluginLoader);
    }

    LoaderConfig& LoaderConfig::get()
    {
        static LoaderConfig instance;
        return instance;
    }

    LoaderConfig::LoaderConfig()
    {
        // Attempt to load file from disk
        if (std::filesystem::exists(config::SPL_LOADER_CONFIG_FILE))
        {
            std::ifstream file(config::SPL_LOADER_CONFIG_FILE);
            json data = json::parse(file);
            this->config = data.get<ConfigFile>();
            file.close();
        }
        else
        {
            // No existing config, create a default and save to disk.
            this->config = ConfigFile
            {
                .LogFile = true,
                .LogCmd = true,
                .LogLevel = "INFO",
                .OutputEveryPath = false,
                .EnablePluginLoader = true,
            };

            std::ofstream file(config::SPL_LOADER_CONFIG_FILE);
            if (file.is_open())
            {
                json data = this->config;
                file << std::setw(4) << data << "\n";
                file.close();
            }
        }
    }
} // namespace preloader