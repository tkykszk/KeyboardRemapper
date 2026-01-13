using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KeyboardRemapper
{
    /// <summary>
    /// キーマッピングのタイプ
    /// </summary>
    public enum MappingType
    {
        /// <summary>単一キーを別のキーにリマップ</summary>
        Remap,
        /// <summary>2つのキーを入れ替え</summary>
        Swap,
        /// <summary>キーを無効化</summary>
        Disable
    }

    /// <summary>
    /// 単一のキーマッピング定義
    /// </summary>
    public class KeyMapping
    {
        [JsonPropertyName("type")]
        public MappingType Type { get; set; } = MappingType.Remap;

        [JsonPropertyName("source_key")]
        public string? SourceKey { get; set; }

        [JsonPropertyName("target_key")]
        public string? TargetKey { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public override string ToString()
        {
            return Type switch
            {
                MappingType.Remap => $"{SourceKey} → {TargetKey}",
                MappingType.Swap => $"{SourceKey} ↔ {TargetKey}",
                MappingType.Disable => $"{SourceKey} (disabled)",
                _ => "Unknown"
            };
        }
    }

    /// <summary>
    /// デバイスごとのマッピング設定
    /// </summary>
    public class DeviceMapping
    {
        [JsonPropertyName("device_id")]
        public string? DeviceId { get; set; }

        [JsonPropertyName("device_name")]
        public string? DeviceName { get; set; }

        [JsonPropertyName("vid")]
        public string? VID { get; set; }

        [JsonPropertyName("pid")]
        public string? PID { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("mappings")]
        public List<KeyMapping> Mappings { get; set; } = new();

        public override string ToString()
        {
            return $"{DeviceName} ({VID}:{PID}) - {Mappings.Count} mappings";
        }
    }

    /// <summary>
    /// 全体の設定ファイル
    /// </summary>
    public class RemapperConfiguration
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("devices")]
        public List<DeviceMapping> Devices { get; set; } = new();

        [JsonPropertyName("global_settings")]
        public GlobalSettings GlobalSettings { get; set; } = new();
    }

    /// <summary>
    /// グローバル設定
    /// </summary>
    public class GlobalSettings
    {
        [JsonPropertyName("auto_start")]
        public bool AutoStart { get; set; } = false;

        [JsonPropertyName("start_minimized")]
        public bool StartMinimized { get; set; } = true;

        [JsonPropertyName("log_level")]
        public string LogLevel { get; set; } = "Info";
    }
}
