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

        static LoaderConfig& get();

        inline bool get_log_file() const { return this->config.log_file; }
        inline bool get_log_cmd() const { return this->config.log_cmd; }
        inline std::string get_log_level() const { return this->config.log_level; }
        inline bool get_output_every_path() const { return this->config.output_every_path; }
        inline bool get_enable_plugin_loader() const { return this->config.enable_plugin_loader; }
    };
} // namespace preloader