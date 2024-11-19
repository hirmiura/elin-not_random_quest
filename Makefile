# SPDX-License-Identifier: MIT
# Copyright 2024 hirmiura (https://github.com/hirmiura)
#
SHELL := /bin/bash

# 各種ディレクトリ/ファイル
D_ElinSrc		:= ElinSrc
D_ElinHome		:= ElinHome
D_Target		:= Not_Random_Quest


#==============================================================================
# カラーコード
# ヘルプ表示
#==============================================================================
include ColorCode.mk
include Help.mk
include Check.mk


#==============================================================================
# 各種確認
#==============================================================================
.PHONY: check
check: ## 事前にチェック項目を確認します
check: check_link_elin


#==============================================================================
# Elin へのリンク/ディレクトリを確認
#==============================================================================
.PHONY: check_link_elin
check_link_elin: ## Elinへのリンク/ディレクトリを確認します
check_link_elin:
	$(call check.linkordir,$(D_ElinHome),Elin,/mnt/c/SteamLibrary/steamapps/common/Elin)


#==============================================================================
# バージョンをアップデート
#==============================================================================
.PHONY: update_version
update_version: ## バージョンをアップデートします
update_version:
	$(eval ver := `cat version.txt`)
	$(eval verElin := `cat versionElin.txt`)
	sed -i -r "s|(\[assembly: Assembly(File)?Version\(\")(.+)(\"\)\])|\1$(ver)\4|g" src/csharp/Properties/AssemblyInfo.cs
	sed -i -r "s|(PLUGIN_VERSION\s*=\s*\")(.+)(\";)|\1$(ver)\3|g" src/csharp/Plugin.cs
	sed -i -r "s|(<version>).*(</version>)|\1$(verElin)-$(ver)\2|g" $(D_Target)/package.xml


#==============================================================================
# DLL生成
#==============================================================================
.PHONY: dll
dll: ## DLLを生成します
dll: update_version
	dotnet build -c Release
	cp -f src/csharp/bin/Release/NotRandomQuest.dll $(D_Target)


#==============================================================================
# ビルド
#==============================================================================
.PHONY: build
build: ## ビルドします
build: dll


#==============================================================================
# 全ての作業を一括で実施する
#==============================================================================
.PHONY: all
all: ## 全ての作業を一括で実施します
all: check build


#==============================================================================
# クリーンアップ
#==============================================================================
.PHONY: clean clean-docs clean-dll clean-dll-all clean-all
clean: ## クリーンアップします
clean: clean-dll

clean-dll:
	dotnet clean -c Debug
	dotnet clean -c Release

clean-dll-all:
	rm -fr src/csharp/.vs
	rm -fr src/csharp/bin
	rm -fr src/csharp/obj

clean-all: ## 生成した全てのファイルを削除します
clean-all: clean-dll-all
