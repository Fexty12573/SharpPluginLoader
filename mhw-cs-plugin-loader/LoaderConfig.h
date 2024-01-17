#pragma once
#include <nlohmann/json.hpp>

namespace preloader
{
    struct ConfigFile {
        bool LogFile;
        bool LogCmd;
        std::string LogLevel;
        bool OutputEveryPath;
        bool EnablePluginLoader;
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

        static LoaderConfig& get();

        inline bool get_log_file() const { return this->config.LogFile; }
        inline bool get_log_cmd() const { return this->config.LogCmd; }
        inline std::string get_log_level() const { return this->config.LogLevel; }
        inline bool get_output_every_path() const { return this->config.OutputEveryPath; }
        inline bool get_enable_plugin_loader() const { return this->config.EnablePluginLoader; }
    };
} // namespace preloader