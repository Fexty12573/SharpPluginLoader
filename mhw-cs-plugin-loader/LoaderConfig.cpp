#include "LoaderConfig.h"
#include <string>
#include <filesystem>
#include <fstream>


namespace preloader
{
    using json = nlohmann::json;

    const std::string config_name("loader-config.json");

    void to_json(json& j, const ConfigFile& c) {
        j = json{
            {"logfile", c.log_file},
            {"logcmd", c.log_cmd},
            {"logLevel", c.log_level},
            {"outputEveryPath", c.output_every_path},
            {"enablePluginLoader", c.enable_plugin_loader}
        };
    }

    void from_json(const json& j, ConfigFile& c) {
        j.at("logfile").get_to(c.log_file);
        j.at("logcmd").get_to(c.log_cmd);
        j.at("logLevel").get_to(c.log_level);
        j.at("outputEveryPath").get_to(c.output_every_path);
        j.at("enablePluginLoader").get_to(c.enable_plugin_loader);
    }

    LoaderConfig& LoaderConfig::Instance()
    {
        static LoaderConfig instance;
        return instance;
    }

    LoaderConfig::LoaderConfig()
    {
        // Attempt to load file from disk
        if (std::filesystem::exists(config_name))
        {
            std::ifstream file(config_name);
            json data = json::parse(file);
            this->config = data.get<ConfigFile>();
            file.close();
        }
        else
        {
            // No existing config, create a default and save to disk.
            this->config = ConfigFile
            {
                .log_file = true,
                .log_cmd = true,
                .log_level = "INFO",
                .output_every_path = false,
                .enable_plugin_loader = true,
            };

            std::ofstream file(config_name);
            if (file.is_open())
            {
                json data = this->config;
                file << std::setw(4) << data << "\n";
                file.close();
            }
        }
    }
} // namespace preloader