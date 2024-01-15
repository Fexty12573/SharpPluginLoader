#pragma once
#include <nlohmann/json.hpp>

namespace preloader
{
    struct ConfigFile {
        bool log_file;
        bool log_cmd;
        std::string log_level;
        bool output_every_path;
        bool enable_plugin_loader;
    };
    void to_json(nlohmann::json& j, const ConfigFile& c);
    void from_json(const nlohmann::json& j, ConfigFile& c);


    class LoaderConfig {
    private:
        LoaderConfig();
        ~LoaderConfig() = default;

        ConfigFile config;
    public:
        LoaderConfig(const LoaderConfig&) = delete;
        LoaderConfig& operator = (const LoaderConfig&) = delete;

        static LoaderConfig& Instance();

        inline bool GetLogFile() const { return this->config.log_file; }
        inline bool GetLogCmd() const { return this->config.log_cmd; }
        inline std::string GetLogLevel() const { return this->config.log_level; }
        inline bool GetOutputEveryPath() const { return this->config.output_every_path; }
        inline bool GetEnablePluginLoader() const { return this->config.enable_plugin_loader; }
    };
} // namespace preloader