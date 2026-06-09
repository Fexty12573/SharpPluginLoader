#pragma once

#include <string>
#include <vector>

namespace preloader {
    struct LoaderGuiConfig {
        const char* UnmanagedMenuKey = nullptr;
        bool KeyboardNavigation = false;
        float FontScale = 1.0f;
        float WindowTransparency = 1.0f;
    };

    struct ConfigFile {
        bool LogFile = true;
        bool LogCmd = false;
        std::string LogLevel = "ERROR";
        bool OutputEveryPath = false;
        bool EnablePluginLoader = true;
        struct {
            // dinput8(Stracker's Loader): So any (native) plugins that rely on injecting code via
            //  relative calls will be able to find available memory for their hooks.
            // dxgi(ReShade): Allow ReShade to properly hook the game.
            std::vector<std::string> PreloadDLLs = { "dinput8.dll", "dxgi.dll" };
            bool ImGuiRenderingEnabled = true;
            bool PrimitiveRenderingEnabled = true;
            std::string MenuKey = "F9";
            LoaderGuiConfig Gui;
        };
    };


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
        inline const std::string& get_log_level() const { return this->config.LogLevel; }
        inline bool get_output_every_path() const { return this->config.OutputEveryPath; }
        inline const std::vector<std::string>& get_preload_dlls() const { return this->config.PreloadDLLs; }
        inline bool get_enable_plugin_loader() const { return this->config.EnablePluginLoader; }
        inline bool get_imgui_rendering_enabled() const { return this->config.ImGuiRenderingEnabled; }
        inline bool get_primitive_rendering_enabled() const { return this->config.PrimitiveRenderingEnabled; }
        inline const LoaderGuiConfig* get_gui_config() const { return &this->config.Gui; }

        static void save_gui_config(LoaderGuiConfig* config);
    };
} // namespace preloader
