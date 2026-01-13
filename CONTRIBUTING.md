# 貢献ガイドライン

KeyboardRemapperへの貢献をありがとうございます。このドキュメントでは、プロジェクトに貢献するための方法を説明します。

## 行動規範

このプロジェクトとその参加者は、[Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/) に従うことを約束します。

## 貢献の方法

### 1. バグ報告

バグを発見した場合は、[GitHub Issues](https://github.com/yourusername/KeyboardRemapper/issues) で報告してください。

**報告時に含めるべき情報**:

- **タイトル**: 簡潔で説明的なタイトル
- **説明**: バグの詳細な説明
- **再現手順**: バグを再現するための手順
  1. ...
  2. ...
  3. ...
- **期待される動作**: 本来の動作
- **実際の動作**: 実際に起こった動作
- **スクリーンショット**: 必要に応じて
- **環境情報**:
  - Windows バージョン（例：Windows 11 22H2）
  - .NET バージョン（例：.NET 8.0）
  - キーボードモデル（VID/PID）
  - アプリケーションバージョン

**バグ報告の例**:

```
Title: CapsLockマッピングが反映されない

Description:
CapsLockをLCtrlにマッピングしても、キーボード入力が反映されません。

Steps to Reproduce:
1. HHKB Professional (VID: 04FE, PID: 0021) を接続
2. `map 04FE:0021 CapsLock LCtrl remap` コマンドを実行
3. `save` コマンドで設定を保存
4. アプリケーションを再起動
5. CapsLockキーを押す

Expected Behavior:
CapsLockキーを押すとLCtrlキーとして機能する

Actual Behavior:
CapsLockキーが通常通り機能する

Environment:
- Windows 11 22H2
- .NET 8.0
- HHKB Professional (VID: 04FE, PID: 0021)
- Application Version: 1.0.0
```

### 2. 機能リクエスト

新しい機能を提案したい場合は、[GitHub Issues](https://github.com/yourusername/KeyboardRemapper/issues) で提案してください。

**提案時に含めるべき情報**:

- **タイトル**: 機能の簡潔な説明
- **説明**: 機能の詳細な説明
- **ユースケース**: この機能がなぜ必要か
- **実装案**: 可能であれば実装方法の提案

**機能リクエストの例**:

```
Title: GUI版の実装

Description:
現在のCLI版に加えて、WinForms/WPFを使用したGUI版を実装したいです。

Use Cases:
- コマンドラインに不慣れなユーザーが設定を簡単に変更できる
- キーボードビジュアルでマッピングを確認できる
- プロファイル管理が直感的になる

Implementation Idea:
- WinFormsを使用したシンプルなUI
- キーボードビジュアル表示
- ドラッグ&ドロップでマッピング設定
```

### 3. プルリクエスト

コードの改善を提案したい場合は、プルリクエストを送信してください。

#### 準備

1. **フォーク**: リポジトリをフォーク
2. **ブランチを作成**: 機能ブランチを作成
   ```bash
   git checkout -b feature/add-new-feature
   ```
3. **コミット**: 変更をコミット
   ```bash
   git commit -am 'Add new feature'
   ```
4. **プッシュ**: ブランチをプッシュ
   ```bash
   git push origin feature/add-new-feature
   ```
5. **プルリクエストを作成**: GitHub上でプルリクエストを作成

#### コーディング規約

- **言語**: C# (.NET 8.0)
- **命名規則**:
  - クラス名: PascalCase (例: `KeyboardDevice`)
  - メソッド名: PascalCase (例: `DetectKeyboards()`)
  - プロパティ名: PascalCase (例: `DeviceName`)
  - ローカル変数: camelCase (例: `deviceId`)
  - 定数: UPPER_SNAKE_CASE (例: `MAX_DEVICES`)

- **フォーマット**:
  - インデント: 4スペース
  - 行の最大長: 120文字
  - 末尾のスペースなし

- **ドキュメント**:
  - XML コメント（`///`）を使用
  - 公開メソッド・プロパティに説明を記載
  - 複雑なロジックにはインラインコメントを記載

**コーディング例**:

```csharp
/// <summary>
/// キーボードデバイスを検出
/// </summary>
/// <returns>検出されたキーボードデバイスのリスト</returns>
public List<KeyboardDevice> DetectKeyboards()
{
    var detectedDevices = new List<KeyboardDevice>();
    
    try
    {
        var devices = RawInputDevice.GetDevices();
        
        foreach (var device in devices)
        {
            if (device is RawInputKeyboard keyboard)
            {
                // デバイス情報を抽出
                var deviceId = $"{keyboard.VendorId:X04}:{keyboard.ProductId:X04}";
                var deviceName = keyboard.ProductName ?? "Unknown Keyboard";
                
                var kbDevice = new KeyboardDevice
                {
                    DeviceHandle = RawInputDeviceHandle.GetRawValue(keyboard.Handle),
                    DeviceName = deviceName,
                    DeviceId = deviceId,
                    VendorId = keyboard.VendorId,
                    ProductId = keyboard.ProductId
                };
                
                detectedDevices.Add(kbDevice);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error detecting keyboards: {ex.Message}");
    }
    
    return detectedDevices;
}
```

#### テスト

- 新機能は必ずテストしてください
- 既存の機能が壊れていないか確認してください
- テストコードを追加してください（可能な場合）

#### プルリクエストの説明

プルリクエストを作成する際は、以下の情報を含めてください:

```markdown
## 説明
この変更が何をするのか、簡潔に説明してください。

## 関連する Issue
Fixes #123

## 変更内容
- 変更1
- 変更2
- 変更3

## テスト方法
この変更をテストする方法を説明してください。

## チェックリスト
- [ ] コードをテストした
- [ ] ドキュメントを更新した
- [ ] コーディング規約に従っている
- [ ] 既存の機能を壊していない
```

## 開発環境のセットアップ

### 必要なツール

- .NET 8.0 SDK
- Visual Studio 2022 または Visual Studio Code
- Git

### セットアップ手順

```bash
# リポジトリをクローン
git clone https://github.com/yourusername/KeyboardRemapper.git
cd KeyboardRemapper

# 依存ライブラリをインストール
dotnet restore

# ビルド
dotnet build

# テスト
dotnet test
```

## プロジェクト構造

```
KeyboardRemapper/
├── KeyboardRemapper/                    # メインプロジェクト
│   ├── KeyboardDevice.cs               # デバイス管理
│   ├── KeyMapping.cs                   # キーマッピング定義
│   ├── ConfigurationManager.cs         # 設定ファイル管理
│   ├── KeyCodeConverter.cs             # キーコード変換
│   ├── KeyRemapEngine.cs               # リマップエンジン
│   ├── CommandLineInterface.cs         # CLIインターフェース
│   ├── Program.cs                      # エントリーポイント
│   └── KeyboardRemapper.csproj        # プロジェクトファイル
├── README.md                           # ドキュメント
├── BUILD.md                            # ビルド手順
├── LICENSE                             # ライセンス
└── CONTRIBUTING.md                     # このファイル
```

## コミットメッセージの規約

コミットメッセージは以下の形式に従ってください:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Type** (必須):
- `feat`: 新機能
- `fix`: バグ修正
- `docs`: ドキュメント変更
- `style`: コード整形（機能変更なし）
- `refactor`: リファクタリング
- `perf`: パフォーマンス改善
- `test`: テスト追加・変更
- `chore`: ビルド・依存関係・ツール変更

**Scope** (オプション):
- `device`: デバイス管理
- `mapping`: キーマッピング
- `config`: 設定管理
- `cli`: CLIインターフェース
- `engine`: リマップエンジン

**Subject** (必須):
- 命令形で記述（"Add feature"、"Fix bug"）
- 最初の文字は大文字
- 末尾にピリオドなし
- 50文字以内

**Body** (オプション):
- 何を変更したか、なぜ変更したかを説明
- 72文字で折り返し

**Footer** (オプション):
- `Fixes #123` のような Issue 参照

**コミットメッセージの例**:

```
feat(device): Add support for wireless keyboards

Implement detection of wireless keyboards using Raw Input API.
This allows users to configure wireless keyboards independently
from wired keyboards.

Fixes #42
```

## レビュープロセス

1. **自動チェック**: CI/CD パイプラインが実行
2. **コードレビュー**: メンテナーがコードをレビュー
3. **修正リクエスト**: 必要に応じて修正をリクエスト
4. **マージ**: 承認後、メインブランチにマージ

## 質問・相談

質問や相談がある場合は:

- [GitHub Discussions](https://github.com/yourusername/KeyboardRemapper/discussions) で質問
- [GitHub Issues](https://github.com/yourusername/KeyboardRemapper/issues) でディスカッション

## 謝辞

KeyboardRemapperへの貢献をありがとうございます。皆さんのサポートがプロジェクトを成長させます。

---

**Happy coding!** 🎉
