using System;
using System.Collections.Generic;

namespace KeyboardRemapper
{
    /// <summary>
    /// キー名と仮想キーコードの相互変換
    /// </summary>
    public static class KeyCodeConverter
    {
        // 仮想キーコード定数
        private static readonly Dictionary<string, uint> KeyNameToVKey = new(StringComparer.OrdinalIgnoreCase)
        {
            // 修飾キー
            { "LCtrl", 0xA2 },
            { "RCtrl", 0xA3 },
            { "LShift", 0xA0 },
            { "RShift", 0xA1 },
            { "LAlt", 0xA4 },
            { "RAlt", 0xA5 },
            { "LWin", 0x5B },
            { "RWin", 0x5C },

            // 特殊キー
            { "CapsLock", 0x14 },
            { "NumLock", 0x90 },
            { "ScrollLock", 0x91 },
            { "Escape", 0x1B },
            { "Esc", 0x1B },
            { "Tab", 0x09 },
            { "Backspace", 0x08 },
            { "Enter", 0x0D },
            { "Return", 0x0D },
            { "Space", 0x20 },
            { "Insert", 0x2D },
            { "Delete", 0x2E },
            { "Home", 0x24 },
            { "End", 0x23 },
            { "PageUp", 0x21 },
            { "PageDown", 0x22 },

            // 矢印キー
            { "Left", 0x25 },
            { "Up", 0x26 },
            { "Right", 0x27 },
            { "Down", 0x28 },

            // ファンクションキー
            { "F1", 0x70 },
            { "F2", 0x71 },
            { "F3", 0x72 },
            { "F4", 0x73 },
            { "F5", 0x74 },
            { "F6", 0x75 },
            { "F7", 0x76 },
            { "F8", 0x77 },
            { "F9", 0x78 },
            { "F10", 0x79 },
            { "F11", 0x7A },
            { "F12", 0x7B },

            // 数字キー
            { "0", 0x30 },
            { "1", 0x31 },
            { "2", 0x32 },
            { "3", 0x33 },
            { "4", 0x34 },
            { "5", 0x35 },
            { "6", 0x36 },
            { "7", 0x37 },
            { "8", 0x38 },
            { "9", 0x39 },

            // 文字キー
            { "A", 0x41 },
            { "B", 0x42 },
            { "C", 0x43 },
            { "D", 0x44 },
            { "E", 0x45 },
            { "F", 0x46 },
            { "G", 0x47 },
            { "H", 0x48 },
            { "I", 0x49 },
            { "J", 0x4A },
            { "K", 0x4B },
            { "L", 0x4C },
            { "M", 0x4D },
            { "N", 0x4E },
            { "O", 0x4F },
            { "P", 0x50 },
            { "Q", 0x51 },
            { "R", 0x52 },
            { "S", 0x53 },
            { "T", 0x54 },
            { "U", 0x55 },
            { "V", 0x56 },
            { "W", 0x57 },
            { "X", 0x58 },
            { "Y", 0x59 },
            { "Z", 0x5A },

            // 記号キー
            { "Semicolon", 0xBA },
            { "Colon", 0xBA },
            { "Equals", 0xBB },
            { "Plus", 0xBB },
            { "Comma", 0xBC },
            { "LessThan", 0xBC },
            { "Minus", 0xBD },
            { "Underscore", 0xBD },
            { "Period", 0xBE },
            { "GreaterThan", 0xBE },
            { "Slash", 0xBF },
            { "Question", 0xBF },
            { "Grave", 0xC0 },
            { "Tilde", 0xC0 },
            { "LeftBracket", 0xDB },
            { "LeftBrace", 0xDB },
            { "Backslash", 0xDC },
            { "Pipe", 0xDC },
            { "RightBracket", 0xDD },
            { "RightBrace", 0xDD },
            { "Apostrophe", 0xDE },
            { "Quote", 0xDE },

            // テンキー
            { "NumPad0", 0x60 },
            { "NumPad1", 0x61 },
            { "NumPad2", 0x62 },
            { "NumPad3", 0x63 },
            { "NumPad4", 0x64 },
            { "NumPad5", 0x65 },
            { "NumPad6", 0x66 },
            { "NumPad7", 0x67 },
            { "NumPad8", 0x68 },
            { "NumPad9", 0x69 },
            { "NumPadMultiply", 0x6A },
            { "NumPadAdd", 0x6B },
            { "NumPadSubtract", 0x6D },
            { "NumPadDecimal", 0x6E },
            { "NumPadDivide", 0x6F },

            // 日本語キーボード固有キー
            { "Muhenkan", 0x1D },  // 無変換
            { "Henkan", 0x1C },    // 変換
            { "Hiragana", 0xF2 },  // ひらがな
            { "Katakana", 0xF1 },  // カタカナ
        };

        private static readonly Dictionary<uint, string> VKeyToKeyName = new();

        static KeyCodeConverter()
        {
            // 逆引きテーブルを構築
            foreach (var kvp in KeyNameToVKey)
            {
                if (!VKeyToKeyName.ContainsKey(kvp.Value))
                {
                    VKeyToKeyName[kvp.Value] = kvp.Key;
                }
            }
        }

        /// <summary>
        /// キー名から仮想キーコードに変換
        /// </summary>
        public static uint GetVKeyFromName(string keyName)
        {
            if (KeyNameToVKey.TryGetValue(keyName, out var vkey))
            {
                return vkey;
            }

            // 16進数文字列の場合
            if (keyName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (uint.TryParse(keyName.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out var hexValue))
                {
                    return hexValue;
                }
            }

            throw new ArgumentException($"Unknown key name: {keyName}");
        }

        /// <summary>
        /// 仮想キーコードからキー名に変換
        /// </summary>
        public static string GetNameFromVKey(uint vkey)
        {
            if (VKeyToKeyName.TryGetValue(vkey, out var name))
            {
                return name;
            }

            return $"0x{vkey:X2}";
        }

        /// <summary>
        /// キー名が有効かどうかを確認
        /// </summary>
        public static bool IsValidKeyName(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return false;

            if (KeyNameToVKey.ContainsKey(keyName))
                return true;

            if (keyName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return uint.TryParse(keyName.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out _);
            }

            return false;
        }

        /// <summary>
        /// 利用可能なすべてのキー名を取得
        /// </summary>
        public static IEnumerable<string> GetAvailableKeyNames()
        {
            return KeyNameToVKey.Keys;
        }
    }
}
