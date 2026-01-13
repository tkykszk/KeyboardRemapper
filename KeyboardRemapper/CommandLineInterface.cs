using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyboardRemapper
{
    /// <summary>
    /// コマンドラインインターフェース
    /// </summary>
    public class CommandLineInterface
    {
        private readonly KeyboardDeviceManager _deviceManager;
        private readonly ConfigurationManager _configManager;
        private readonly KeyRemapEngine _remapEngine;

        public CommandLineInterface()
        {
            _deviceManager = new KeyboardDeviceManager();
            _configManager = new ConfigurationManager();
            _remapEngine = new KeyRemapEngine();
        }

        /// <summary>
        /// CLIを実行
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine("=== Keyboard Remapper CLI ===");
            Console.WriteLine("Type 'help' for available commands\n");

            // 設定を読み込み
            await _configManager.LoadAsync();

            bool running = true;
            while (running)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0].ToLower();

                try
                {
                    running = await ExecuteCommandAsync(command, parts.Skip(1).ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// コマンドを実行
        /// </summary>
        private async Task<bool> ExecuteCommandAsync(string command, string[] args)
        {
            return command switch
            {
                "help" => ShowHelp(),
                "list" => ListDevices(),
                "detect" => DetectDevices(),
                "config" => await ConfigureDevice(args),
                "map" => await AddMapping(args),
                "show" => ShowConfiguration(args),
                "save" => await SaveConfiguration(),
                "load" => await LoadConfiguration(),
                "remove" => await RemoveMapping(args),
                "clear" => await ClearDeviceConfig(args),
                "test" => TestKeyMapping(args),
                "exit" => false,
                "quit" => false,
                _ => UnknownCommand(command)
            };
        }

        private bool ShowHelp()
        {
            Console.WriteLine(@"
Available commands:
  help                    - Show this help message
  list                    - List all connected keyboards
  detect                  - Detect connected keyboards
  config <device_id>      - Configure a specific device
  map <device_id> <src> <target> [type]
                          - Add key mapping (type: remap|swap|disable, default: remap)
  show [device_id]        - Show configuration (all or specific device)
  save                    - Save configuration to file
  load                    - Load configuration from file
  remove <device_id> <src_key>
                          - Remove a key mapping
  clear <device_id>       - Clear all mappings for a device
  test <device_id> <key>  - Test key mapping
  exit                    - Exit the application
  quit                    - Exit the application

Examples:
  map 0 CapsLock LCtrl remap    - Remap CapsLock to LCtrl
  map 0 CapsLock LCtrl swap     - Swap CapsLock and LCtrl
  map 0 CapsLock 0 disable      - Disable CapsLock
");
            return true;
        }

        private bool ListDevices()
        {
            var devices = _deviceManager.DetectKeyboards();
            
            if (devices.Count == 0)
            {
                Console.WriteLine("No keyboards detected.");
                return true;
            }

            Console.WriteLine($"Found {devices.Count} keyboard(s):");
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                Console.WriteLine($"  [{i}] {device}");
                Console.WriteLine($"       Handle: {device.DeviceHandle}");
                Console.WriteLine($"       ID: {device.DeviceId}");
            }

            return true;
        }

        private bool DetectDevices()
        {
            Console.WriteLine("Detecting keyboards...");
            var devices = _deviceManager.DetectKeyboards();
            
            if (devices.Count == 0)
            {
                Console.WriteLine("No keyboards detected.");
                return true;
            }

            Console.WriteLine($"Detected {devices.Count} keyboard(s):");
            foreach (var device in devices)
            {
                Console.WriteLine($"  - {device}");
            }

            return true;
        }

        private async Task<bool> ConfigureDevice(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: config <device_id>");
                return true;
            }

            var deviceId = args[0];
            var devices = _deviceManager.DetectKeyboards();
            var device = devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId) || d.DeviceName.Contains(deviceId));

            if (device == null)
            {
                Console.WriteLine($"Device not found: {deviceId}");
                return true;
            }

            Console.WriteLine($"Configuring device: {device}");
            
            var config = _configManager.GetConfiguration();
            var deviceMapping = config.Devices.FirstOrDefault(d => d.DeviceId == device.DeviceId)
                ?? new DeviceMapping
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    VID = device.VendorId.ToString("X4"),
                    PID = device.ProductId.ToString("X4")
                };

            _configManager.SetDeviceMapping(deviceMapping);
            await _configManager.SaveAsync();

            Console.WriteLine("Device configuration saved.");
            return true;
        }

        private async Task<bool> AddMapping(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: map <device_id> <source_key> <target_key> [type]");
                Console.WriteLine("Types: remap (default), swap, disable");
                return true;
            }

            var deviceId = args[0];
            var sourceKey = args[1];
            var targetKey = args[2];
            var typeStr = args.Length > 3 ? args[3].ToLower() : "remap";

            // キー名の検証
            if (!KeyCodeConverter.IsValidKeyName(sourceKey))
            {
                Console.WriteLine($"Invalid source key: {sourceKey}");
                return true;
            }

            if (typeStr != "disable" && !KeyCodeConverter.IsValidKeyName(targetKey))
            {
                Console.WriteLine($"Invalid target key: {targetKey}");
                return true;
            }

            var mappingType = typeStr switch
            {
                "swap" => MappingType.Swap,
                "disable" => MappingType.Disable,
                _ => MappingType.Remap
            };

            var config = _configManager.GetConfiguration();
            var deviceMapping = config.Devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId));

            if (deviceMapping == null)
            {
                Console.WriteLine($"Device not configured: {deviceId}");
                return true;
            }

            var keyMapping = new KeyMapping
            {
                Type = mappingType,
                SourceKey = sourceKey,
                TargetKey = targetKey,
                Description = $"{sourceKey} -> {targetKey}"
            };

            deviceMapping.Mappings.Add(keyMapping);
            _configManager.SetDeviceMapping(deviceMapping);
            await _configManager.SaveAsync();

            Console.WriteLine($"Mapping added: {keyMapping}");
            return true;
        }

        private bool ShowConfiguration(string[] args)
        {
            var config = _configManager.GetConfiguration();

            if (config.Devices.Count == 0)
            {
                Console.WriteLine("No devices configured.");
                return true;
            }

            if (args.Length > 0)
            {
                var deviceId = args[0];
                var device = config.Devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId));
                
                if (device == null)
                {
                    Console.WriteLine($"Device not found: {deviceId}");
                    return true;
                }

                ShowDeviceConfiguration(device);
            }
            else
            {
                foreach (var device in config.Devices)
                {
                    ShowDeviceConfiguration(device);
                    Console.WriteLine();
                }
            }

            return true;
        }

        private void ShowDeviceConfiguration(DeviceMapping device)
        {
            Console.WriteLine($"Device: {device.DeviceName} ({device.VID}:{device.PID})");
            Console.WriteLine($"  Enabled: {device.Enabled}");
            Console.WriteLine($"  Mappings ({device.Mappings.Count}):");

            if (device.Mappings.Count == 0)
            {
                Console.WriteLine("    (none)");
            }
            else
            {
                foreach (var mapping in device.Mappings)
                {
                    Console.WriteLine($"    - {mapping}");
                }
            }
        }

        private async Task<bool> SaveConfiguration()
        {
            await _configManager.SaveAsync();
            Console.WriteLine("Configuration saved.");
            return true;
        }

        private async Task<bool> LoadConfiguration()
        {
            await _configManager.LoadAsync();
            Console.WriteLine("Configuration loaded.");
            return true;
        }

        private async Task<bool> RemoveMapping(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: remove <device_id> <source_key>");
                return true;
            }

            var deviceId = args[0];
            var sourceKey = args[1];

            var config = _configManager.GetConfiguration();
            var device = config.Devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId));

            if (device == null)
            {
                Console.WriteLine($"Device not found: {deviceId}");
                return true;
            }

            var mapping = device.Mappings.FirstOrDefault(m => m.SourceKey == sourceKey);
            if (mapping == null)
            {
                Console.WriteLine($"Mapping not found: {sourceKey}");
                return true;
            }

            device.Mappings.Remove(mapping);
            _configManager.SetDeviceMapping(device);
            await _configManager.SaveAsync();

            Console.WriteLine($"Mapping removed: {mapping}");
            return true;
        }

        private async Task<bool> ClearDeviceConfig(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: clear <device_id>");
                return true;
            }

            var deviceId = args[0];
            var config = _configManager.GetConfiguration();
            var device = config.Devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId));

            if (device == null)
            {
                Console.WriteLine($"Device not found: {deviceId}");
                return true;
            }

            device.Mappings.Clear();
            _configManager.SetDeviceMapping(device);
            await _configManager.SaveAsync();

            Console.WriteLine($"All mappings cleared for {device.DeviceName}");
            return true;
        }

        private bool TestKeyMapping(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: test <device_id> <key_name>");
                return true;
            }

            var deviceId = args[0];
            var keyName = args[1];

            if (!KeyCodeConverter.IsValidKeyName(keyName))
            {
                Console.WriteLine($"Invalid key name: {keyName}");
                return true;
            }

            var vkey = KeyCodeConverter.GetVKeyFromName(keyName);
            Console.WriteLine($"Key: {keyName}");
            Console.WriteLine($"Virtual Key Code: 0x{vkey:X2} ({vkey})");

            var config = _configManager.GetConfiguration();
            var device = config.Devices.FirstOrDefault(d => d.DeviceId.Contains(deviceId));

            if (device != null)
            {
                var mapping = device.Mappings.FirstOrDefault(m => m.SourceKey == keyName);
                if (mapping != null)
                {
                    Console.WriteLine($"Mapped to: {mapping.TargetKey}");
                }
            }

            return true;
        }

        private bool UnknownCommand(string command)
        {
            Console.WriteLine($"Unknown command: {command}");
            Console.WriteLine("Type 'help' for available commands.");
            return true;
        }
    }
}
