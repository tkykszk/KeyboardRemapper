using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KeyboardRemapper
{
    /// <summary>
    /// 設定ファイルの管理
    /// </summary>
    public class ConfigurationManager
    {
        private readonly string _configPath;
        private RemapperConfiguration _configuration;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ConfigurationManager(string configPath = null)
        {
            _configPath = configPath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "KeyboardRemapper",
                "config.json"
            );
        }

        /// <summary>
        /// 設定ファイルを読み込む
        /// </summary>
        public async Task<RemapperConfiguration> LoadAsync()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    Console.WriteLine($"Config file not found at {_configPath}. Creating default config.");
                    _configuration = new RemapperConfiguration();
                    await SaveAsync();
                    return _configuration;
                }

                var json = await File.ReadAllTextAsync(_configPath);
                _configuration = JsonSerializer.Deserialize<RemapperConfiguration>(json, JsonOptions) ?? new RemapperConfiguration();

                Console.WriteLine($"Configuration loaded from {_configPath}");
                return _configuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                _configuration = new RemapperConfiguration();
                return _configuration;
            }
        }

        /// <summary>
        /// 設定ファイルを保存
        /// </summary>
        public async Task SaveAsync()
        {
            try
            {
                var directory = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(_configuration, JsonOptions);
                await File.WriteAllTextAsync(_configPath, json);
                Console.WriteLine($"Configuration saved to {_configPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// 現在の設定を取得
        /// </summary>
        public RemapperConfiguration GetConfiguration()
        {
            return _configuration ?? new RemapperConfiguration();
        }

        /// <summary>
        /// 設定を更新
        /// </summary>
        public void UpdateConfiguration(RemapperConfiguration? config)
        {
            _configuration = config ?? new RemapperConfiguration();
        }

        /// <summary>
        /// デバイスのマッピング設定を取得
        /// </summary>
        public DeviceMapping? GetDeviceMapping(string deviceId)
        {
            var config = GetConfiguration();
            return config.Devices.Find(d => d.DeviceId == deviceId);
        }

        /// <summary>
        /// デバイスのマッピング設定を追加または更新
        /// </summary>
        public void SetDeviceMapping(DeviceMapping mapping)
        {
            var config = GetConfiguration();
            if (string.IsNullOrEmpty(mapping.DeviceId))
                return;
            
            var existing = config.Devices.FindIndex(d => d.DeviceId == mapping.DeviceId);
            
            if (existing >= 0)
            {
                config.Devices[existing] = mapping;
            }
            else
            {
                config.Devices.Add(mapping);
            }
        }
    }
}
