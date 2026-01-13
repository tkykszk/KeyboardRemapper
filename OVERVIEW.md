# Windows デバイス別キーボードリマッパー - プロジェクト概要

## プロジェクト概要

**KeyboardRemapper** は、複数のキーボードを接続したWindows環境で、各キーボードに対して個別のキーマッピングを設定できるツールです。

### 背景

Windowsには25年以上、キーボードごとに異なるキー設定を行う標準機能がありません。Ctrl2Cap、PowerToys、レジストリ変更などの既存の解決策はすべてシステム全体に適用され、デバイスを区別できません。

**KeyboardRemapper** は、Windows Raw Input API を使用してこの問題を解決します。

## 主な特徴

### 1. デバイス別設定
- 接続されているキーボードをVID/PID/デバイス名で一意に識別
- 各キーボードに対して独立したキーマッピングを設定可能

### 2. 複数のマッピング方式
- **リマップ**: 単一キー → 単一キー（例：CapsLock → LCtrl）
- **スワップ**: 2つのキーを入れ替え（例：CapsLock ↔ LCtrl）
- **無効化**: キーを完全に無効化

### 3. JSON設定ファイル
- 設定を JSON 形式で保存・読み込み可能
- 複数のプロファイルを管理可能

### 4. CLIインターフェース
- コマンドラインから簡単に設定を管理
- スクリプト化による自動化が可能

## 技術仕様

### 使用技術

- **言語**: C# (.NET 8.0)
- **入力フック**: Windows Raw Input API
- **ライブラリ**: rawinput-sharp (v0.1.3)
- **UI**: CLI版MVP（GUI版は計画中）

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

### デバイス識別方式

各キーボードは以下の情報で一意に識別されます:

- **VID (Vendor ID)**: ベンダーID (16進数4桁)
- **PID (Product ID)**: プロダクトID (16進数4桁)
- **Device Path**: デバイスパス（Windowsが割り当てるユニークなパス）
- **Device Handle**: デバイスハンドル（Raw Input APIが割り当てるハンドル）

## ファイル構成

```
KeyboardRemapper/
├── KeyboardRemapper/                    # メインプロジェクト
│   ├── KeyboardDevice.cs               # デバイス管理
│   │   ├── KeyboardDevice             # キーボードデバイス情報
│   │   └── KeyboardDeviceManager      # デバイス検出・管理
│   ├── KeyMapping.cs                   # キーマッピング定義
│   │   ├── MappingType                # マッピングタイプ（Remap/Swap/Disable）
│   │   ├── KeyMapping                 # 単一のキーマッピング
│   │   └── DeviceMapping              # デバイスごとのマッピング設定
│   ├── ConfigurationManager.cs         # 設定ファイル管理
│   │   └── RemapperConfiguration      # 全体的な設定
│   ├── KeyCodeConverter.cs             # キーコード変換
│   │   └── キー名 ↔ 仮想キーコード変換
│   ├── KeyRemapEngine.cs               # リマップエンジン
│   │   └── キー入力の処理とリマップ適用
│   ├── CommandLineInterface.cs         # CLIインターフェース
│   │   └── コマンド処理とユーザーインタラクション
│   ├── Program.cs                      # エントリーポイント
│   └── KeyboardRemapper.csproj        # プロジェクトファイル
├── README.md                           # 使い方ドキュメント
├── BUILD.md                            # ビルド手順
├── CONTRIBUTING.md                     # 貢献ガイドライン
├── LICENSE                             # MIT License
├── OVERVIEW.md                         # このファイル
└── config.sample.json                  # 設定ファイルの例
```

## 実装の詳細

### 1. デバイス検出（KeyboardDevice.cs）

```csharp
// rawinput-sharpを使用してキーボードを検出
var devices = RawInputDevice.GetDevices();
foreach (var device in devices)
{
    if (device is RawInputKeyboard keyboard)
    {
        // デバイス情報を抽出
        var deviceId = $"{keyboard.VendorId:X04}:{keyboard.ProductId:X04}";
        var deviceName = keyboard.ProductName ?? "Unknown Keyboard";
        // ...
    }
}
```

### 2. キー入力の処理（KeyRemapEngine.cs）

```csharp
// RawInputReceiverWindowでWM_INPUTメッセージをキャッチ
window.Input += (sender, e) =>
{
    var data = e.Data;
    if (data is RawInputKeyboardData keyboardData)
    {
        var vkey = keyboardData.Keyboard.VirutalKey;
        var isKeyUp = (keyboardData.Keyboard.Flags & RawKeyboardFlags.Up) != 0;
        
        // リマップを適用
        ProcessKeyInput(deviceHandle, keyboardData, out var remappedVKey, out var shouldBlock);
    }
};
```

### 3. 設定ファイル管理（ConfigurationManager.cs）

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
        }
      ]
    }
  ]
}
```

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

## 使用例

### ユースケース1: HHKB Professional の設定

```
# HHKB Professional (VID: 04FE, PID: 0021) を接続

> list
Found 2 keyboard(s):
  [0] HHKB Professional (VID: 04FE, PID: 0021)
  [1] Built-in Keyboard (VID: 0000, PID: 0000)

# HHKB用の設定を作成
> config 04FE:0021
Configuring device: HHKB Professional (VID: 04FE, PID: 0021)

# CapsLockをLCtrlに変更
> map 04FE:0021 CapsLock LCtrl remap
Added mapping: CapsLock -> LCtrl (remap)

# 設定を保存
> save
Configuration saved to: C:\Users\YourName\AppData\Local\KeyboardRemapper\config.json
```

### ユースケース2: 内蔵キーボードは変更しない

```
# 内蔵キーボード（VID: 0000, PID: 0000）は設定なし
# HHKB Professional のみ設定

> show
Device: HHKB Professional (VID: 04FE, PID: 0021)
Mappings:
  - CapsLock -> LCtrl (remap)

Device: Built-in Keyboard (VID: 0000, PID: 0000)
Mappings:
  - (no mappings)
```

## インストール・実行

### 前提条件
- Windows 10 / 11 (64-bit)
- .NET 8.0 以上

### ビルド手順

```bash
# ソースコードを取得
cd KeyboardRemapper

# 依存ライブラリをインストール
dotnet restore

# ビルド
dotnet build -c Release

# 実行
dotnet run -c Release
```

詳細は [BUILD.md](BUILD.md) を参照してください。

## ドキュメント

- **[README.md](README.md)**: 使い方ドキュメント
- **[BUILD.md](BUILD.md)**: ビルド手順
- **[CONTRIBUTING.md](CONTRIBUTING.md)**: 貢献ガイドライン
- **[LICENSE](LICENSE)**: MIT License

## 参考資料

- [Windows Raw Input API](https://learn.microsoft.com/en-us/windows/win32/inputdev/raw-input)
- [rawinput-sharp GitHub](https://github.com/mfakane/rawinput-sharp)
- [Virtual-Key Codes](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)

## ライセンス

このプロジェクトは **MIT License** の下で公開されています。

## 貢献

バグ報告、機能リクエスト、プルリクエストを歓迎します。詳細は [CONTRIBUTING.md](CONTRIBUTING.md) を参照してください。

---

**作成日**: 2026年1月13日  
**バージョン**: 1.0.0  
**ステータス**: MVP (Minimum Viable Product)
