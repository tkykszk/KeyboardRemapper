# Windows デバイス別キーボードリマッパー

複数のキーボードを接続したWindows環境で、各キーボードに対して個別のキーマッピングを設定できるツールです。ノートPCの内蔵キーボードと外付けキーボードで異なる設定（例：CapsLock↔Ctrl入れ替え）をしたいニーズに対応します。

## 特徴

- **デバイス別設定**: 接続されているキーボードをVID/PID/デバイス名で一意に識別し、各キーボードに対して独立したキーマッピングを設定可能
- **複数のマッピング方式**:
  - **リマップ**: 単一キー → 単一キー（例：CapsLock → LCtrl）
  - **スワップ**: 2つのキーを入れ替え（例：CapsLock ↔ LCtrl）
  - **無効化**: キーを完全に無効化（例：CapsLock → 無効）
- **JSON設定ファイル**: 設定を JSON 形式で保存・読み込み可能
- **CLI インターフェース**: コマンドラインから簡単に設定を管理
- **Windows 10/11 対応**: Windows Raw Input API を使用した安定した実装

## システム要件

- **OS**: Windows 10 / 11 (64-bit)
- **.NET**: .NET 8.0 以上
- **管理者権限**: 基本機能は管理者権限なしで動作（キーボード入力の完全な制御には管理者権限推奨）

## インストール

### 方法1: ビルド済みバイナリを使用

1. [Releases](https://github.com/yourusername/KeyboardRemapper/releases) からダウンロード
2. `KeyboardRemapper.exe` を実行

### 方法2: ソースコードからビルド

```bash
git clone https://github.com/yourusername/KeyboardRemapper.git
cd KeyboardRemapper/KeyboardRemapper
dotnet build -c Release
dotnet run
```

## 使い方

### 基本的なコマンド

#### 1. キーボードの検出

```
> list
Found 2 keyboard(s):
  [0] HHKB Professional (VID: 04FE, PID: 0021)
       Handle: 18350
       ID: 04FE:0021
  [1] Built-in Keyboard (VID: 0000, PID: 0000)
       Handle: 18351
       ID: 0000:0000
```

#### 2. デバイスの設定

```
> config 04FE:0021
Configuring device: HHKB Professional (VID: 04FE, PID: 0021)
Device configuration saved.
```

#### 3. キーマッピングの追加

```
> map 04FE:0021 CapsLock LCtrl remap
Added mapping: CapsLock -> LCtrl (remap)

> map 04FE:0021 CapsLock LCtrl swap
Added mapping: CapsLock <-> LCtrl (swap)

> map 04FE:0021 CapsLock 0 disable
Added mapping: CapsLock -> disabled
```

#### 4. 設定の表示

```
> show 04FE:0021
Device: HHKB Professional (VID: 04FE, PID: 0021)
Mappings:
  - CapsLock -> LCtrl (remap)
  - CapsLock <-> LCtrl (swap)
  - CapsLock -> disabled
```

#### 5. 設定の保存・読み込み

```
> save
Configuration saved to: C:\Users\YourName\AppData\Local\KeyboardRemapper\config.json

> load
Configuration loaded from: C:\Users\YourName\AppData\Local\KeyboardRemapper\config.json
```

#### 6. マッピングの削除

```
> remove 04FE:0021 CapsLock
Removed mapping: CapsLock
```

#### 7. デバイス設定のクリア

```
> clear 04FE:0021
Cleared all mappings for device: HHKB Professional
```

### 全コマンド一覧

| コマンド | 説明 |
|---------|------|
| `help` | ヘルプを表示 |
| `list` | 接続されているキーボード一覧を表示 |
| `detect` | キーボードを再検出 |
| `config <device_id>` | デバイスを設定 |
| `map <device_id> <src> <target> [type]` | キーマッピングを追加（type: remap\|swap\|disable） |
| `show [device_id]` | 設定を表示（全体または特定デバイス） |
| `save` | 設定をファイルに保存 |
| `load` | 設定をファイルから読み込み |
| `remove <device_id> <src_key>` | キーマッピングを削除 |
| `clear <device_id>` | デバイスの全マッピングをクリア |
| `test <device_id> <key>` | キーマッピングをテスト |
| `exit` / `quit` | アプリケーションを終了 |

## 設定ファイル形式

設定は JSON 形式で以下の場所に保存されます:

```
C:\Users\<ユーザー名>\AppData\Local\KeyboardRemapper\config.json
```

### 設定ファイルの例

```json
{
  "version": "1.0",
  "devices": [
    {
      "device_id": "04FE:0021",
      "device_name": "HHKB Professional",
      "vid": "04FE",
      "pid": "0021",
      "enabled": true,
      "mappings": [
        {
          "type": "remap",
          "source_key": "CapsLock",
          "target_key": "LCtrl",
          "description": "CapsLock -> LCtrl"
        },
        {
          "type": "swap",
          "source_key": "LCtrl",
          "target_key": "CapsLock",
          "description": "LCtrl <-> CapsLock"
        },
        {
          "type": "disable",
          "source_key": "CapsLock",
          "target_key": "0",
          "description": "CapsLock disabled"
        }
      ]
    },
    {
      "device_id": "0000:0000",
      "device_name": "Built-in Keyboard",
      "vid": "0000",
      "pid": "0000",
      "enabled": false,
      "mappings": []
    }
  ]
}
```

## サポートされているキー

### 標準キー

| キー名 | 説明 |
|--------|------|
| `A`-`Z` | アルファベットキー |
| `0`-`9` | 数字キー |
| `F1`-`F24` | ファンクションキー |
| `Enter` | Enterキー |
| `Escape` | Escapeキー |
| `Tab` | Tabキー |
| `Space` | スペースキー |
| `Backspace` | Backspaceキー |
| `Delete` | Deleteキー |
| `Insert` | Insertキー |
| `Home` | Homeキー |
| `End` | Endキー |
| `PageUp` | Page Upキー |
| `PageDown` | Page Downキー |

### 修飾キー

| キー名 | 説明 |
|--------|------|
| `LCtrl` | 左Ctrlキー |
| `RCtrl` | 右Ctrlキー |
| `LShift` | 左Shiftキー |
| `RShift` | 右Shiftキー |
| `LAlt` | 左Altキー |
| `RAlt` | 右Altキー |
| `LWin` | 左Windowsキー |
| `RWin` | 右Windowsキー |
| `CapsLock` | CapsLockキー |

### 特殊キー

| キー名 | 説明 |
|--------|------|
| `Up` | 上矢印キー |
| `Down` | 下矢印キー |
| `Left` | 左矢印キー |
| `Right` | 右矢印キー |
| `NumLock` | NumLockキー |
| `ScrollLock` | ScrollLockキー |
| `PrintScreen` | Print Screenキー |
| `Pause` | Pauseキー |

## トラブルシューティング

### キーボードが検出されない

1. キーボードが正しく接続されていることを確認
2. デバイスマネージャーでキーボードが認識されているか確認
3. `detect` コマンドで再検出を試みる

```
> detect
Detecting keyboards...
Detected 1 keyboard(s):
  - HHKB Professional (VID: 04FE, PID: 0021)
```

### マッピングが反映されない

1. 設定が正しく保存されているか確認

```
> show 04FE:0021
```

2. アプリケーションを再起動して設定を再読み込み

```
> load
```

### 特定のキーが認識されない

- サポートされているキー名を確認（大文字小文字を区別）
- `test` コマンドでマッピングをテスト

```
> test 04FE:0021 CapsLock
Testing mapping for CapsLock on device 04FE:0021
```

## 技術仕様

### アーキテクチャ

```
┌─────────────────────────────────────────────────────┐
│         KeyboardRemapperApplication                 │
│  (CLI版MVP → WinForms GUI版へ拡張)                   │
└────────────────┬────────────────────────────────────┘
                 │
        ┌────────┴────────┐
        │                 │
   ┌────▼─────┐    ┌─────▼────────┐
   │ CLI UI    │    │ Configuration│
   │ (MVP)     │    │ Manager      │
   └────┬─────┘    └─────┬────────┘
        │                │
        └────────┬───────┘
                 │
        ┌────────▼──────────────┐
        │ KeyRemapEngine        │
        │ - Device Detection    │
        │ - Key Interception    │
        │ - Remapping Logic     │
        └────────┬──────────────┘
                 │
        ┌────────▼──────────────┐
        │ Raw Input API Wrapper │
        │ (rawinput-sharp)      │
        └────────┬──────────────┘
                 │
        ┌────────▼──────────────┐
        │ Windows Raw Input API │
        └───────────────────────┘
```

### 使用ライブラリ

- **rawinput-sharp** (v0.1.3): Windows Raw Input API のC#ラッパー
  - ライセンス: Zlib License
  - GitHub: https://github.com/mfakane/rawinput-sharp

### デバイス識別方式

各キーボードは以下の情報で一意に識別されます:

- **VID (Vendor ID)**: ベンダーID (16進数4桁)
- **PID (Product ID)**: プロダクトID (16進数4桁)
- **Device Path**: デバイスパス（Windowsが割り当てるユニークなパス）
- **Device Handle**: デバイスハンドル（Raw Input APIが割り当てるハンドル）

## 開発ロードマップ

### v1.0 (現在)

- ✅ CLI版MVP
- ✅ デバイス検出機能
- ✅ 基本的なキーマッピング（リマップ、スワップ、無効化）
- ✅ JSON設定ファイル管理

### v1.1 (計画中)

- [ ] GUI版（WinForms）
- [ ] キーボードビジュアル表示
- [ ] プロファイル管理機能
- [ ] ホットキー設定

### v1.2 (計画中)

- [ ] 修飾キー付きリマップ（Ctrl+Shift など）
- [ ] マクロ機能
- [ ] スケジューリング機能
- [ ] ログ機能

## ライセンス

このプロジェクトは **MIT License** の下で公開されています。

詳細は [LICENSE](LICENSE) ファイルを参照してください。

## 貢献

バグ報告、機能リクエスト、プルリクエストを歓迎します。

## 参考資料

- [Windows Raw Input API](https://learn.microsoft.com/en-us/windows/win32/inputdev/raw-input)
- [rawinput-sharp GitHub](https://github.com/mfakane/rawinput-sharp)
- [Virtual-Key Codes](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)

## よくある質問

### Q: 管理者権限は必要ですか？

A: 基本的な機能（デバイス検出、設定管理）は管理者権限なしで動作します。ただし、キーボード入力を完全に制御するには管理者権限の実行を推奨します。

### Q: 複数のキーボードを同時に使用できますか？

A: はい。各キーボードに対して独立した設定が可能です。

### Q: 設定は永続的に保存されますか？

A: はい。JSON設定ファイルに保存されます。アプリケーション再起動時に自動的に読み込まれます。

### Q: 日本語キーボード（JIS配列）に対応していますか？

A: 対応予定です。現在は標準的なキーコードのみサポートしています。

## サポート

問題が発生した場合は、以下の手順でサポートを受けてください:

1. [Issues](https://github.com/yourusername/KeyboardRemapper/issues) で既知の問題を確認
2. 問題が見つからない場合は、新しい Issue を作成
3. 以下の情報を含めてください:
   - Windows バージョン
   - キーボードモデル（VID/PID）
   - エラーメッセージ
   - 実行したコマンド

## 更新履歴

### v1.0.0 (2026-01-13)

- 初版リリース
- CLI版MVP実装
- デバイス検出機能
- 基本的なキーマッピング機能
- JSON設定ファイル管理
