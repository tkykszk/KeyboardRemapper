using Xunit;
using System.Collections.Generic;
using KeyboardRemapper;

namespace KeyboardRemapper.Tests
{
    /// <summary>
    /// キーリマップエンジンのユニットテスト
    /// キーボードイベント変換が正しく処理されているかを検証
    /// </summary>
    public class KeyRemapEngineTests
    {
        private KeyRemapEngine _engine;
        private KeyMapping _mapping;

        public KeyRemapEngineTests()
        {
            _engine = new KeyRemapEngine();
            _mapping = new KeyMapping();
        }

        #region リマップテスト

        [Fact]
        public void TestRemapCapsLockToLCtrl()
        {
            // Arrange: CapsLock -> LCtrl のリマップを設定
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl",
                Description = "CapsLock -> LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: CapsLock キーイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);

            // Assert: LCtrl に変換されていることを確認
            Assert.NotNull(result);
            Assert.Equal("LCtrl", result.MappedKey);
            Assert.Equal("remap", result.MappingType);
            Assert.True(result.IsKeyPressed);
        }

        [Fact]
        public void TestRemapMultipleKeys()
        {
            // Arrange: 複数のキーをリマップ
            var deviceId = "04FE:0021";
            var mappings = new List<KeyMapping>
            {
                new KeyMapping { Type = "remap", SourceKey = "CapsLock", TargetKey = "LCtrl" },
                new KeyMapping { Type = "remap", SourceKey = "Escape", TargetKey = "CapsLock" }
            };

            foreach (var mapping in mappings)
            {
                _engine.AddMapping(deviceId, mapping);
            }

            // Act: 各キーイベントを処理
            var result1 = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(deviceId, "Escape", true);

            // Assert: 各キーが正しくマップされていることを確認
            Assert.Equal("LCtrl", result1.MappedKey);
            Assert.Equal("CapsLock", result2.MappedKey);
        }

        [Fact]
        public void TestRemapKeyRelease()
        {
            // Arrange: CapsLock -> LCtrl のリマップを設定
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: キーリリースイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", false);

            // Assert: キーリリースが正しく処理されていることを確認
            Assert.NotNull(result);
            Assert.Equal("LCtrl", result.MappedKey);
            Assert.False(result.IsKeyPressed);
        }

        #endregion

        #region スワップテスト

        [Fact]
        public void TestSwapCapsLockAndLCtrl()
        {
            // Arrange: CapsLock <-> LCtrl をスワップ
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "swap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl",
                Description = "CapsLock <-> LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: CapsLock キーイベントを処理
            var result1 = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(deviceId, "LCtrl", true);

            // Assert: キーが交換されていることを確認
            Assert.Equal("LCtrl", result1.MappedKey);
            Assert.Equal("CapsLock", result2.MappedKey);
        }

        [Fact]
        public void TestSwapMultipleKeyPairs()
        {
            // Arrange: 複数のキーペアをスワップ
            var deviceId = "04FE:0021";
            var mappings = new List<KeyMapping>
            {
                new KeyMapping { Type = "swap", SourceKey = "CapsLock", TargetKey = "LCtrl" },
                new KeyMapping { Type = "swap", SourceKey = "LShift", TargetKey = "LAlt" }
            };

            foreach (var mapping in mappings)
            {
                _engine.AddMapping(deviceId, mapping);
            }

            // Act: 各キーイベントを処理
            var result1 = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(deviceId, "LCtrl", true);
            var result3 = _engine.ProcessKeyEvent(deviceId, "LShift", true);
            var result4 = _engine.ProcessKeyEvent(deviceId, "LAlt", true);

            // Assert: 各キーペアが正しく交換されていることを確認
            Assert.Equal("LCtrl", result1.MappedKey);
            Assert.Equal("CapsLock", result2.MappedKey);
            Assert.Equal("LAlt", result3.MappedKey);
            Assert.Equal("LShift", result4.MappedKey);
        }

        #endregion

        #region 無効化テスト

        [Fact]
        public void TestDisableKey()
        {
            // Arrange: CapsLock を無効化
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "disable",
                SourceKey = "CapsLock",
                TargetKey = "0",
                Description = "CapsLock disabled"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: CapsLock キーイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);

            // Assert: キーが無効化されていることを確認
            Assert.NotNull(result);
            Assert.True(result.IsDisabled);
            Assert.Null(result.MappedKey);
        }

        [Fact]
        public void TestDisableMultipleKeys()
        {
            // Arrange: 複数のキーを無効化
            var deviceId = "04FE:0021";
            var mappings = new List<KeyMapping>
            {
                new KeyMapping { Type = "disable", SourceKey = "CapsLock", TargetKey = "0" },
                new KeyMapping { Type = "disable", SourceKey = "ScrollLock", TargetKey = "0" }
            };

            foreach (var mapping in mappings)
            {
                _engine.AddMapping(deviceId, mapping);
            }

            // Act: 各キーイベントを処理
            var result1 = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(deviceId, "ScrollLock", true);

            // Assert: 各キーが無効化されていることを確認
            Assert.True(result1.IsDisabled);
            Assert.True(result2.IsDisabled);
        }

        #endregion

        #region デバイス別テスト

        [Fact]
        public void TestDeviceSpecificMapping()
        {
            // Arrange: 2つのデバイスに異なるマッピングを設定
            var device1 = "04FE:0021";  // 外付けキーボード
            var device2 = "0000:0000";  // 内蔵キーボード

            var mapping1 = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            var mapping2 = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "Escape"
            };

            _engine.AddMapping(device1, mapping1);
            _engine.AddMapping(device2, mapping2);

            // Act: 各デバイスのキーイベントを処理
            var result1 = _engine.ProcessKeyEvent(device1, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(device2, "CapsLock", true);

            // Assert: 各デバイスで異なるマッピングが適用されていることを確認
            Assert.Equal("LCtrl", result1.MappedKey);
            Assert.Equal("Escape", result2.MappedKey);
        }

        [Fact]
        public void TestUnmappedKeyPassthrough()
        {
            // Arrange: CapsLock のみをマッピング
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: マッピングされていないキーイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "A", true);

            // Assert: マッピングされていないキーはそのまま通過することを確認
            Assert.NotNull(result);
            Assert.Equal("A", result.MappedKey);
            Assert.Null(result.MappingType);
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void TestEmptyMapping()
        {
            // Arrange: マッピングなし
            var deviceId = "04FE:0021";

            // Act: キーイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);

            // Assert: マッピングなしの場合、キーはそのまま通過することを確認
            Assert.NotNull(result);
            Assert.Equal("CapsLock", result.MappedKey);
        }

        [Fact]
        public void TestNonexistentDevice()
        {
            // Arrange: 存在しないデバイスID
            var deviceId = "FFFF:FFFF";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: キーイベントを処理
            var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);

            // Assert: マッピングが正しく処理されることを確認
            Assert.NotNull(result);
            Assert.Equal("LCtrl", result.MappedKey);
        }

        [Fact]
        public void TestCaseSensitivity()
        {
            // Arrange: キー名の大文字小文字を区別するか確認
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: 異なるケースでキーイベントを処理
            var result1 = _engine.ProcessKeyEvent(deviceId, "CapsLock", true);
            var result2 = _engine.ProcessKeyEvent(deviceId, "capslock", true);

            // Assert: 大文字小文字を区別することを確認
            Assert.Equal("LCtrl", result1.MappedKey);
            Assert.Equal("capslock", result2.MappedKey);  // マッピングされない
        }

        [Fact]
        public void TestRapidKeyPresses()
        {
            // Arrange: 複数のキープレスを連続処理
            var deviceId = "04FE:0021";
            var mapping = new KeyMapping
            {
                Type = "remap",
                SourceKey = "CapsLock",
                TargetKey = "LCtrl"
            };

            _engine.AddMapping(deviceId, mapping);

            // Act: 複数のキープレスを処理
            var results = new List<KeyEventResult>();
            for (int i = 0; i < 100; i++)
            {
                var result = _engine.ProcessKeyEvent(deviceId, "CapsLock", i % 2 == 0);
                results.Add(result);
            }

            // Assert: すべてのキープレスが正しく処理されていることを確認
            Assert.Equal(100, results.Count);
            foreach (var result in results)
            {
                Assert.Equal("LCtrl", result.MappedKey);
            }
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void TestPerformanceWithManyMappings()
        {
            // Arrange: 多数のマッピングを設定
            var deviceId = "04FE:0021";
            var keys = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

            foreach (var key in keys)
            {
                var mapping = new KeyMapping
                {
                    Type = "remap",
                    SourceKey = key,
                    TargetKey = key.ToLower()
                };
                _engine.AddMapping(deviceId, mapping);
            }

            // Act: キーイベントを1000回処理
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                _engine.ProcessKeyEvent(deviceId, "A", true);
            }
            sw.Stop();

            // Assert: 処理時間が許容範囲内であることを確認（1000回で1秒以内）
            Assert.True(sw.ElapsedMilliseconds < 1000, 
                $"Processing took {sw.ElapsedMilliseconds}ms, expected < 1000ms");
        }

        #endregion
    }

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
}
