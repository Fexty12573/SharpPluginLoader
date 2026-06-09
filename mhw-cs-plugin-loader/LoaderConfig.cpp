#include "LoaderConfig.h"
#include <string>
#include <filesystem>
#include <fstream>

#include "Config.h"
#include "Json.h"

namespace preloader {
    static void to_json(json& j, const ConfigFile& c) {
        j = json{
            {"logfile", c.LogFile},
            {"logcmd", c.LogCmd},
            {"logLevel", c.LogLevel},
            {"outputEveryPath", c.OutputEveryPath},
            {"enablePluginLoader", c.EnablePluginLoader},
            {"SPL", {
                {"PreloadDLLs", c.PreloadDLLs},
                {"ImGuiRenderingEnabled", c.ImGuiRenderingEnabled},
                {"PrimitiveRenderingEnabled", c.PrimitiveRenderingEnabled},
                {"MenuKey", c.MenuKey},
                {"KeyboardNavigation", c.Gui.KeyboardNavigation},
                {"FontScale", c.Gui.FontScale},
                {"WindowTransparency", c.Gui.WindowTransparency},
            }}
        };
    }

#define json_try_get(j, key, out) if ((j).contains(key)) { (j).at(key).get_to(out); }
    static void from_json(const json& j, ConfigFile& c) {
        json_try_get(j, "logfile", c.LogFile);
        json_try_get(j, "logcmd", c.LogCmd);
        json_try_get(j, "logLevel", c.LogLevel);
        json_try_get(j, "outputEveryPath", c.OutputEveryPath);
        json_try_get(j, "enablePluginLoader", c.EnablePluginLoader);
        if (j.contains("SPL")) {
            const json& spl = j.at("SPL");
            json_try_get(spl, "PreloadDLLs", c.PreloadDLLs);
            json_try_get(spl, "ImGuiRenderingEnabled", c.ImGuiRenderingEnabled);
            json_try_get(spl, "PrimitiveRenderingEnabled", c.PrimitiveRenderingEnabled);
            json_try_get(spl, "MenuKey", c.MenuKey);
            json_try_get(spl, "KeyboardNavigation", c.Gui.KeyboardNavigation);
            json_try_get(spl, "FontScale", c.Gui.FontScale);
            json_try_get(spl, "WindowTransparency", c.Gui.WindowTransparency);
        }
    }
#undef json_try_get

    static void write_config(ConfigFile& c) {
        std::ofstream outfile(config::SPL_LOADER_CONFIG_FILE);
        if (outfile.is_open()) {
            json data = c;
            outfile << std::setw(4) << data << "\n";
            outfile.close();
        }
    }

    LoaderConfig& LoaderConfig::get() {
        static LoaderConfig instance;
        return instance;
    }

    void LoaderConfig::save_gui_config(LoaderGuiConfig* config) {
        ConfigFile& c = get().config;
        c.Gui = *config;
        c.MenuKey = std::string(c.Gui.UnmanagedMenuKey);
        c.Gui.UnmanagedMenuKey = c.MenuKey.c_str();
        write_config(c);
    }

    LoaderConfig::LoaderConfig() {
        ConfigFile& c = this->config;

        // Attempt to load file from disk.
        if (std::filesystem::exists(config::SPL_LOADER_CONFIG_FILE)) {
            std::ifstream file(config::SPL_LOADER_CONFIG_FILE);
            json data = json::parse(file, nullptr, false);
            if (!data.is_discarded()) {
                data.get_to<ConfigFile>(c);
            }
            file.close();
        }
        c.Gui.UnmanagedMenuKey = c.MenuKey.c_str();

        // Make sure any new options are added to the config file.
        write_config(c);
    }
} // namespace preloader
