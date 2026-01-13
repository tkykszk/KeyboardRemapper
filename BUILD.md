# ビルド手順書

このドキュメントでは、KeyboardRemapperをソースコードからビルドする方法を説明します。

## 前提条件

### システム要件

- **OS**: Windows 10 / 11 (64-bit)
- **.NET SDK**: .NET 8.0 以上
- **ビルドツール**: Visual Studio 2022 または .NET CLI

### 必要なソフトウェア

1. **.NET 8.0 SDK** のインストール
   - [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) からダウンロード
   - インストール後、以下のコマンドで確認:
   ```bash
   dotnet --version
   ```

2. **Git** のインストール（オプション）
   - [https://git-scm.com/download/win](https://git-scm.com/download/win) からダウンロード

## ビルド手順

### 方法1: コマンドラインでビルド

#### 1. ソースコードの取得

```bash
# GitHubからクローン
git clone https://github.com/yourusername/KeyboardRemapper.git
cd KeyboardRemapper
```

または、ZIPファイルをダウンロードして展開:

```bash
# ダウンロードしたZIPを展開
unzip KeyboardRemapper-main.zip
cd KeyboardRemapper-main
```

#### 2. 依存ライブラリの復元

```bash
cd KeyboardRemapper
dotnet restore
```

#### 3. デバッグビルド

```bash
dotnet build
```

出力:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:02.31
```

#### 4. リリースビルド（最適化版）

```bash
dotnet build -c Release
```

#### 5. 実行

```bash
# デバッグ版
dotnet run

# リリース版
dotnet run -c Release
```

### 方法2: Visual Studio 2022でビルド

#### 1. Visual Studioを開く

```bash
# または、Visual Studioから直接開く
start KeyboardRemapper.sln
```

#### 2. ソリューションを開く

- File → Open → Project/Solution
- `KeyboardRemapper.sln` を選択

#### 3. ビルド

- Build → Build Solution (Ctrl+Shift+B)
- または、Build → Build KeyboardRemapper

#### 4. 実行

- Debug → Start Debugging (F5)
- または、Debug → Start Without Debugging (Ctrl+F5)

## ビルド出力

### デバッグビルド

```
bin/Debug/net8.0/
├── KeyboardRemapper.exe          # 実行ファイル
├── KeyboardRemapper.dll          # DLL
├── KeyboardRemapper.pdb          # デバッグシンボル
├── RawInput.Sharp.dll            # 依存ライブラリ
└── その他の依存ファイル
```

### リリースビルド

```
bin/Release/net8.0/
├── KeyboardRemapper.exe          # 実行ファイル（最適化版）
├── KeyboardRemapper.dll          # DLL（最適化版）
└── その他の依存ファイル
```

## 実行

### コマンドラインから実行

```bash
# デバッグ版
./bin/Debug/net8.0/KeyboardRemapper.exe

# リリース版
./bin/Release/net8.0/KeyboardRemapper.exe
```

### CLIコマンドの例

```
> list
Found 2 keyboard(s):
  [0] HHKB Professional (VID: 04FE, PID: 0021)
       Handle: 18350
       ID: 04FE:0021
  [1] Built-in Keyboard (VID: 0000, PID: 0000)
       Handle: 18351
       ID: 0000:0000

> map 04FE:0021 CapsLock LCtrl remap
Added mapping: CapsLock -> LCtrl (remap)

> save
Configuration saved to: C:\Users\YourName\AppData\Local\KeyboardRemapper\config.json

> exit
```

## トラブルシューティング

### エラー: ".NET SDK not found"

**原因**: .NET SDKがインストールされていない

**解決策**:
1. [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) から .NET 8.0 SDK をダウンロード
2. インストーラーを実行
3. コマンドプロンプトを再起動
4. `dotnet --version` で確認

### エラー: "NuGet restore failed"

**原因**: NuGetパッケージの復元に失敗

**解決策**:
```bash
# NuGetキャッシュをクリア
dotnet nuget locals all --clear

# 再度復元を試みる
dotnet restore
```

### エラー: "Build failed with errors"

**原因**: コンパイルエラー

**解決策**:
1. エラーメッセージを確認
2. 以下の手順でクリーンビルドを試みる:
```bash
dotnet clean
dotnet build
```

### エラー: "rawinput-sharp not found"

**原因**: 依存ライブラリが見つからない

**解決策**:
```bash
# NuGetパッケージを明示的にインストール
dotnet add package RawInput.Sharp --version 0.1.3

# または、プロジェクトファイルを確認
cat KeyboardRemapper.csproj
```

## 開発環境のセットアップ

### Visual Studio Code での開発

#### 1. 必要な拡張機能をインストール

- C# (powered by OmniSharp)
- .NET Runtime Installer

#### 2. ワークスペースを開く

```bash
code KeyboardRemapper
```

#### 3. ビルドタスクを実行

- Ctrl+Shift+B でビルド

### Visual Studio 2022 での開発

#### 1. 拡張機能をインストール

- .NET MAUI
- C# Dev Kit

#### 2. ソリューションを開く

```bash
start KeyboardRemapper.sln
```

#### 3. デバッグを開始

- F5 でデバッグ開始

## パッケージング

### スタンドアロン実行ファイルの作成

```bash
# 自己完結型の実行ファイルを作成（Windows x64）
dotnet publish -c Release -r win-x64 --self-contained

# 出力ファイル
bin/Release/net8.0/win-x64/publish/KeyboardRemapper.exe
```

### ZIPアーカイブの作成

```bash
# PowerShellスクリプト
$source = "bin/Release/net8.0/win-x64/publish"
$destination = "KeyboardRemapper-v1.0.0-win-x64.zip"
Compress-Archive -Path $source -DestinationPath $destination
```

## クリーンアップ

### ビルド成果物の削除

```bash
dotnet clean
```

### NuGetキャッシュのクリア

```bash
dotnet nuget locals all --clear
```

## 次のステップ

- [README.md](README.md) で使い方を確認
- [config.sample.json](config.sample.json) で設定ファイルの例を確認
- [GitHub Issues](https://github.com/yourusername/KeyboardRemapper/issues) でバグ報告

## 参考資料

- [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [rawinput-sharp](https://github.com/mfakane/rawinput-sharp)
