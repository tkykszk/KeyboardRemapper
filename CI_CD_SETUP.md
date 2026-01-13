# GitHub Actions CI/CD セットアップガイド

このドキュメントでは、GitHub Actions を使用して KeyboardRemapper の自動ビルド・テスト・パッケージングを設定する方法を説明します。

## 概要

GitHub Actions により、以下の処理が自動的に実行されます:

1. **ビルド**: Debug/Release ビルドの実行
2. **テスト**: デバイス検出機能のテスト
3. **品質チェック**: コードスタイルの検証
4. **パッケージング**: Windows x64 用の自己完結型実行ファイルの作成

## セットアップ手順

### 1. GitHub リポジトリの作成

```bash
# ローカルリポジトリを初期化
git init
git add .
git commit -m "Initial commit: KeyboardRemapper v1.0.0"

# リモートリポジトリを追加
git remote add origin https://github.com/yourusername/KeyboardRemapper.git
git branch -M main
git push -u origin main
```

### 2. ワークフロー設定ファイルの配置

`.github/workflows/build-and-test.yml` ファイルが以下の場所に配置されていることを確認してください:

```
KeyboardRemapper/
├── .github/
│   └── workflows/
│       └── build-and-test.yml
├── KeyboardRemapper/
└── README.md
```

### 3. GitHub Actions の有効化

1. GitHub リポジトリを開く
2. **Settings** → **Actions** → **General** に移動
3. **Actions permissions** で "Allow all actions and reusable workflows" を選択
4. **Save** をクリック

## ワークフロー詳細

### build ジョブ

**実行環境**: `windows-latest` (Windows Server 2022)

**実行内容**:
1. リポジトリをチェックアウト
2. .NET 8.0 SDK をセットアップ
3. NuGet パッケージを復元
4. Debug ビルドを実行
5. Release ビルドを実行
6. デバイス検出機能をテスト
7. ビルド成果物をアップロード

**実行時間**: 約 3-5 分

### code-quality ジョブ

**実行環境**: `windows-latest`

**実行内容**:
1. リポジトリをチェックアウト
2. .NET 8.0 SDK をセットアップ
3. NuGet パッケージを復元
4. コードスタイルをチェック

**実行時間**: 約 2-3 分

### publish ジョブ

**実行条件**: `main` ブランチへのプッシュ時のみ

**実行環境**: `windows-latest`

**実行内容**:
1. リポジトリをチェックアウト
2. .NET 8.0 SDK をセットアップ
3. Windows x64 用の自己完結型実行ファイルを発行
4. ZIP アーカイブを作成
5. リリースパッケージをアップロード

**実行時間**: 約 5-10 分

## ワークフロー実行の確認

### 1. GitHub Actions ダッシュボード

1. GitHub リポジトリを開く
2. **Actions** タブをクリック
3. ワークフロー実行状況を確認

### 2. ワークフロー実行ログの確認

1. ワークフロー実行をクリック
2. ジョブをクリック
3. ステップのログを確認

### 3. ビルド成果物のダウンロード

1. ワークフロー実行をクリック
2. **Artifacts** セクションから成果物をダウンロード

## トラブルシューティング

### エラー: "dotnet: command not found"

**原因**: .NET SDK がインストールされていない

**解決策**: ワークフロー設定で `actions/setup-dotnet@v3` が正しく実行されているか確認

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v3
  with:
    dotnet-version: '8.0.x'
```

### エラー: "NuGet restore failed"

**原因**: NuGet パッケージの復元に失敗

**解決策**: 以下を確認
1. `KeyboardRemapper.csproj` ファイルが正しい場所にあるか
2. インターネット接続が正常か
3. NuGet パッケージが利用可能か

### エラー: "Build failed"

**原因**: コンパイルエラー

**解決策**: ワークフロー実行ログを確認し、エラーメッセージを確認

## カスタマイズ

### ビルド対象の .NET バージョンを変更

```yaml
strategy:
  matrix:
    dotnet-version: ['8.0.x', '9.0.x']  # 複数バージョンをテスト
```

### テスト対象の OS を追加

```yaml
runs-on: ${{ matrix.os }}

strategy:
  matrix:
    os: [windows-latest, ubuntu-latest, macos-latest]
    dotnet-version: ['8.0.x']
```

### 特定のブランチでのみ実行

```yaml
on:
  push:
    branches: [ main ]  # main ブランチのみ
  pull_request:
    branches: [ main ]
```

### スケジュール実行

```yaml
on:
  schedule:
    - cron: '0 0 * * *'  # 毎日 UTC 00:00 に実行
```

## パフォーマンス最適化

### 1. キャッシュの活用

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

### 2. 並列実行

```yaml
jobs:
  build:
    # ...
  code-quality:
    # ...
  # 複数のジョブが並列実行される
```

### 3. 条件付き実行

```yaml
- name: Publish
  if: github.event_name == 'push' && github.ref == 'refs/heads/main'
  run: dotnet publish ...
```

## セキュリティ

### 1. シークレットの管理

機密情報（API キー、トークンなど）は GitHub Secrets に保存:

1. **Settings** → **Secrets and variables** → **Actions** に移動
2. **New repository secret** をクリック
3. 名前と値を入力

ワークフロー内での使用:

```yaml
- name: Deploy
  env:
    API_KEY: ${{ secrets.API_KEY }}
  run: ./deploy.sh
```

### 2. アクセス権限の制限

1. **Settings** → **Actions** → **General** に移動
2. **Workflow permissions** で適切な権限を設定

## 本番環境へのデプロイ

### 1. Release の作成

```bash
git tag v1.0.0
git push origin v1.0.0
```

### 2. GitHub Releases への自動アップロード

```yaml
- name: Create Release
  uses: actions/create-release@v1
  with:
    tag_name: ${{ github.ref }}
    release_name: Release ${{ github.ref }}
    files: KeyboardRemapper-v1.0.0-win-x64.zip
```

## 参考資料

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [GitHub Actions Marketplace](https://github.com/marketplace?type=actions)

## よくある質問

### Q: ワークフローが実行されない

**A**: 以下を確認してください:
1. `.github/workflows/` ディレクトリが存在するか
2. YAML ファイルが正しくフォーマットされているか
3. GitHub Actions が有効になっているか

### Q: ビルド時間が長い

**A**: 以下の最適化を試してください:
1. NuGet キャッシュを活用
2. 不要なジョブを削除
3. 並列実行を活用

### Q: Windows でのみテストしたい

**A**: `runs-on` を `windows-latest` に固定してください

### Q: 複数の OS でテストしたい

**A**: `strategy.matrix.os` で複数の OS を指定してください

---

**作成日**: 2026年1月13日  
**バージョン**: 1.0.0
