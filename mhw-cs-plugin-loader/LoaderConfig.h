#pragma once
#include <nlohmann/json.hpp>

namespace preloader
{
    struct ConfigFile {
        bool LogFile = true;
        bool LogCmd = false;
        std::string LogLevel = "ERROR";
        bool OutputEveryPath = false;
        bool EnablePluginLoader = true;
        struct {
            bool ImGuiRenderingEnabled = true;
            bool PrimitiveRenderingEnabled = true;
            std::string MenuKey = "F9";
        };
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
        inline bool get_imgui_rendering_enabled() const { return this->config.ImGuiRenderingEnabled; }
        inline bool get_primitive_rendering_enabled() const { return this->config.PrimitiveRenderingEnabled; }
        inline std::string get_menu_key() const { return this->config.MenuKey; }
    };
} // namespace preloader