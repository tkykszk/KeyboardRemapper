using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;

namespace KeyboardRemapper
{
    /// <summary>
    /// キーイベント処理結果を表すクラス
    /// </summary>
    public class KeyEventResult
    {
        public string? MappedKey { get; set; }
        public string? MappingType { get; set; }
        public bool IsKeyPressed { get; set; }
        public bool IsDisabled { get; set; }
    }

    /// <summary>
    /// キーリマップエンジン
    /// </summary>
    public class KeyRemapEngine
    {
        // Windows API
        [DllImport("user32.dll")]
        private static extern void SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public INPUTUNION U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        private Dictionary<IntPtr, DeviceMapping> _deviceMappings = new();
        private Dictionary<IntPtr, Dictionary<uint, uint>> _remapCache = new();
        
        // テスト用: デバイスIDベースのマッピング
        private Dictionary<string, List<KeyMapping>> _testMappings = new();

        /// <summary>
        /// デバイスマッピングを設定
        /// </summary>
        public void SetDeviceMapping(IntPtr deviceHandle, DeviceMapping mapping)
        {
            _deviceMappings[deviceHandle] = mapping;
            _remapCache[deviceHandle] = BuildRemapCache(mapping);
        }

        /// <summary>
        /// リマップキャッシュを構築
        /// </summary>
        private Dictionary<uint, uint> BuildRemapCache(DeviceMapping mapping)
        {
            var cache = new Dictionary<uint, uint>();

            if (mapping?.Mappings == null)
                return cache;

            foreach (var keyMapping in mapping.Mappings)
            {
                if (keyMapping.Type == MappingType.Disable)
                {
                    // 無効化キーは0にマップ
                    try
                    {
                        var sourceVKey = KeyCodeConverter.GetVKeyFromName(keyMapping.SourceKey);
                        cache[sourceVKey] = 0;
                    }
                    catch { }
                }
                else if (keyMapping.Type == MappingType.Remap)
                {
                    // リマップ
                    try
                    {
                        var sourceVKey = KeyCodeConverter.GetVKeyFromName(keyMapping.SourceKey);
                        var targetVKey = KeyCodeConverter.GetVKeyFromName(keyMapping.TargetKey);
                        cache[sourceVKey] = targetVKey;
                    }
                    catch { }
                }
                else if (keyMapping.Type == MappingType.Swap)
                {
                    // スワップ（双方向）
                    try
                    {
                        var sourceVKey = KeyCodeConverter.GetVKeyFromName(keyMapping.SourceKey);
                        var targetVKey = KeyCodeConverter.GetVKeyFromName(keyMapping.TargetKey);
                        cache[sourceVKey] = targetVKey;
                        cache[targetVKey] = sourceVKey;
                    }
                    catch { }
                }
            }

            return cache;
        }

        /// <summary>
        /// キー入力を処理してリマップを適用
        /// </summary>
        public bool ProcessKeyInput(IntPtr deviceHandle, RawInputKeyboardData keyboardData, out uint remappedVKey, out bool shouldBlock)
        {
            remappedVKey = 0;
            shouldBlock = false;

            // デバイスマッピングがない場合はスルー
            if (!_remapCache.TryGetValue(deviceHandle, out var cache))
            {
                remappedVKey = (uint)keyboardData.Keyboard.VirutalKey;
                return false;
            }

            var vkey = (uint)keyboardData.Keyboard.VirutalKey;
            var isKeyUp = (keyboardData.Keyboard.Flags & RawKeyboardFlags.Up) != 0;

            // リマップ対象かチェック
            if (cache.TryGetValue(vkey, out var mappedVKey))
            {
                if (mappedVKey == 0)
                {
                    // 無効化キー
                    shouldBlock = true;
                    return true;
                }

                remappedVKey = mappedVKey;
                SendRemappedKey(mappedVKey, isKeyUp);
                shouldBlock = true;
                return true;
            }

            remappedVKey = vkey;
            return false;
        }

        /// <summary>
        /// リマップされたキーを送出
        /// </summary>
        private void SendRemappedKey(uint vkey, bool isKeyUp)
        {
            try
            {
                var input = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    U = new INPUTUNION
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)vkey,
                            wScan = 0,
                            dwFlags = isKeyUp ? KEYEVENTF_KEYUP : 0,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending remapped key: {ex.Message}");
            }
        }

        /// <summary>
        /// デバイスマッピングを削除
        /// </summary>
        public void RemoveDeviceMapping(IntPtr deviceHandle)
        {
            _deviceMappings.Remove(deviceHandle);
            _remapCache.Remove(deviceHandle);
        }

        /// <summary>
        /// すべてのマッピングをクリア
        /// </summary>
        public void ClearAllMappings()
        {
            _deviceMappings.Clear();
            _remapCache.Clear();
        }

        /// <summary>
        /// デバイスがマッピングを持つかチェック
        /// </summary>
        public bool HasMapping(IntPtr deviceHandle)
        {
            return _remapCache.ContainsKey(deviceHandle);
        }

        // ========== テスト用メソッド ==========

        /// <summary>
        /// テスト用: マッピングを追加
        /// </summary>
        public void AddMapping(string deviceId, KeyMapping mapping)
        {
            if (!_testMappings.ContainsKey(deviceId))
            {
                _testMappings[deviceId] = new List<KeyMapping>();
            }
            _testMappings[deviceId].Add(mapping);
        }

        /// <summary>
        /// テスト用: キーイベントを処理してマッピングを適用
        /// </summary>
        public KeyEventResult ProcessKeyEvent(string deviceId, string keyName, bool isPressed)
        {
            var result = new KeyEventResult
            {
                MappedKey = keyName,
                IsKeyPressed = isPressed,
                IsDisabled = false
            };

            if (!_testMappings.ContainsKey(deviceId))
            {
                return result;
            }

            var mappings = _testMappings[deviceId];

            // キーマッピングを検索
            foreach (var mapping in mappings)
            {
                if (mapping.SourceKey == keyName)
                {
                    result.MappingType = mapping.Type;

                    switch (mapping.Type)
                    {
                        case "remap":
                            result.MappedKey = mapping.TargetKey;
                            break;

                        case "swap":
                            // スワップの場合、ターゲットキーもマップ
                            result.MappedKey = mapping.TargetKey;
                            break;

                        case "disable":
                            result.IsDisabled = true;
                            result.MappedKey = null;
                            break;
                    }
                    break;
                }
                // スワップの場合、ターゲットキーからのリバースマップも処理
                else if (mapping.Type == "swap" && mapping.TargetKey == keyName)
                {
                    result.MappingType = mapping.Type;
                    result.MappedKey = mapping.SourceKey;
                    break;
                }
            }

            return result;
        }
    }
}
