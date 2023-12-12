using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SharpPluginLoader.Core.Configuration
{
    public static class ConfigManager
    {
        public static T GetConfig<T>(IPlugin plugin) where T : class, IConfig, new()
        {
            var configPath = plugin.ConfigPath;
            Ensure.NotNull(configPath);

            if (Configs.TryGetValue(plugin.Key, out var cfg))
                return (T)cfg;

            if (!File.Exists(configPath))
            {
                cfg = new T();
                Configs.Add(plugin.Key, cfg);
                InternalSaveConfig(plugin, cfg);
                return (T)cfg;
            }

            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<T>(json, ReadOptions);
            Ensure.NotNull(config);

            Configs.Add(plugin.Key, config);
            return config;
        }

        public static void SaveConfig<T>(IPlugin plugin) where T : class, IConfig, new()
        {
            InternalSaveConfig(plugin, GetConfig<T>(plugin));
        }

        private static void InternalSaveConfig<T>(IPlugin plugin, T config) where T : class, IConfig
        {
            Ensure.NotNull(plugin.ConfigPath);
            var json = JsonSerializer.Serialize(config, WriteOptions);
            File.WriteAllText(plugin.ConfigPath, json);
        }

        internal static void SaveAndUnloadConfig(IPlugin plugin)
        {
            if (Configs.TryGetValue(plugin.Key, out var cfg))
            {
                InternalSaveConfig(plugin, cfg);
                Configs.Remove(plugin.Key);
            }
        }

        private static readonly Dictionary<string, IConfig> Configs = [];
        private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };
        private static readonly JsonSerializerOptions ReadOptions = new() { AllowTrailingCommas = true };
    }
}
