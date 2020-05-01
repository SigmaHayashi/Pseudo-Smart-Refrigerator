# Pseudo Smart Refrigerator


# 概要
ROS-TMSのデータベース（tms_db）に含まれる，冷蔵庫内のオブジェクト（お茶のペットボトル，缶コーヒー，醤油差し）の位置データを変更するAndroidアプリケーション

ROS-TMSにもともとある機能の知的冷蔵庫を使わずに，データベースの情報をGUIを使って変更するアプリケーション

# 必要な環境
PC1 : Windows10 64bit（アプリケーションビルド用）  
PC2 : Ubuntu 16（Smart Previewed Reality実行用）  
※PC1とPC2は同時に起動する必要なし，デュアルブートでOK

Androidスマートフォン

ROS kinetic (Ubuntuにインストールしておく)


# 開発環境
PC : Windows 10 64bit  
* Unity 2018.4.1f1  
* Visual Studio 2017  
* Android Studio 3.5.1  

Android（動作確認済み） : Pixel 3 XL, Pixel 4 XL


# アプリケーションをビルドするためのPCの準備
1. Unityのインストール  
    URL : https://unity3d.com/jp/get-unity/download

1. Visual Studioのインストール  
    ※VS Codeではない  
    ※Unityのインストール中にインストールされるものでOK  
    URL : https://visualstudio.microsoft.com/ja/downloads/

1. Android Studioのインストール  
    ※Android SDKが必要  
    URL : https://developer.android.com/studio


# アプリケーションのインストール方法

1. GitHubから任意の場所にダウンロード

1. Unityでプロジェクトを開く

1. "SampleScene"のSceneを開く

1. AndroidRosSocketClient.csのIPアドレスを指定する行を，ROS-TMSを実行しているUbuntu PCのIPアドレスに変更

1. File > Build Settingsからビルド環境の設定を開く

1. Androidを選択し，Switch Platformを選択

1. Android端末をPCに接続し，Build & Run


# 使い方

## ROS-TMS for Smart Previewed Realityの実行

実行前に，ROSをインストールしたUbuntuでROS-TMS for Smart Previewed Realityをcatkin_makeしておく必要がある．

ROS-TMS for Smart Previewed Reality : https://github.com/SigmaHayashi/ros_tms_for_smart_previewed_reality

このアプリケーションはデータベースを利用するため，mongodbをインストールする必要がある．その他依存関係はROS-TMSのWikiを参照．

Wiki : https://github.com/irvs/ros_tms/wiki


### 実行手順

```
$ roscore
$ roslaunch rosbridge_server rosbridge_websocket.launch
$ roslaunch tms_db_manager tms_db_manager.launch
```
