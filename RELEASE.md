# Release ガイド

このドキュメントでは、KeyboardRemapper の新しいバージョンをリリースする方法について説明します。

## 自動リリース機能

GitHub Actions を使用して、タグをプッシュするだけで自動的にリリースが作成されます。

## リリース手順

### 1. バージョン番号を決定

セマンティックバージョニング (Semantic Versioning) に従ってバージョン番号を決定します:
- **メジャー**: 互換性のない大きな変更（例：v1.0.0 → v2.0.0）
- **マイナー**: 新機能の追加（例：v1.0.0 → v1.1.0）
- **パッチ**: バグ修正（例：v1.0.0 → v1.0.1）

### 2. ローカルでテストを実行

```bash
dotnet test
```

すべてのテストが成功することを確認してください。

### 3. タグを作成してプッシュ

```bash
# タグを作成
git tag -a v1.0.1 -m "Release v1.0.1: Bug fixes and performance improvements"

# タグをプッシュ
git push origin v1.0.1
```

### 4. GitHub Actions が自動実行

タグがプッシュされると、以下の処理が自動的に実行されます:

1. ✅ .NET 8.0 でビルド（Release 構成）
2. ✅ ユニットテスト実行
3. ✅ Windows x64 自己完結型バイナリを作成
4. ✅ ZIP アーカイブを作成
5. ✅ GitHub Release を作成
6. ✅ ZIP ファイルをリリースに添付

### 5. リリースページで確認

GitHub リポジトリの [Releases](https://github.com/tkykszk/KeyboardRemapper/releases) ページでリリースが表示されます。

## リリース内容

各リリースには以下が含まれます:

- **KeyboardRemapper-vX.X.X-win-x64.zip**: Windows x64 用の実行ファイルと依存ライブラリ
- **リリースノート**: 変更内容の自動生成

## リリースノート例

```
## Changes
- Added support for multiple keyboard profiles
- Fixed memory leak in key remapping engine
- Improved performance for rapid key presses
- Updated documentation

## Contributors
- tkykszk
```

## ダウンロード

ユーザーは以下の方法でリリースをダウンロードできます:

1. GitHub リポジトリの [Releases](https://github.com/tkykszk/KeyboardRemapper/releases) ページにアクセス
2. 最新のリリースを選択
3. `KeyboardRemapper-vX.X.X-win-x64.zip` をダウンロード
4. ZIP ファイルを解凍
5. `KeyboardRemapper.exe` を実行

## トラブルシューティング

### リリースが作成されない

**原因**: GitHub Actions ワークフローが失敗している

**解決方法**:
1. GitHub リポジトリの **Actions** タブを確認
2. 失敗したワークフロー実行の詳細を確認
3. エラーログを確認して原因を特定

### テストが失敗する

**原因**: ユニットテストが失敗している

**解決方法**:
1. ローカルで `dotnet test` を実行
2. 失敗したテストを修正
3. タグを再度プッシュ

## バージョン履歴

| バージョン | リリース日 | 主な変更 |
|-----------|-----------|--------|
| v1.0.0 | 2026-01-13 | 初回リリース（MVP） |
| v1.0.1 | TBD | バグ修正 |
| v1.1.0 | TBD | GUI版の追加 |

## リリースチェックリスト

新しいバージョンをリリースする前に、以下を確認してください:

- [ ] すべてのテストが成功している
- [ ] ドキュメント（README、QUICKSTART など）が最新になっている
- [ ] バージョン番号が正しく設定されている
- [ ] 変更ログが記載されている
- [ ] ソースコードがコミットされている

---

**GitHub Actions により、リリースプロセスが完全に自動化されています。** 🚀
