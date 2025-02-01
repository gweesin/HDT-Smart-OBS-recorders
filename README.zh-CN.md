# 炉石传说智能 OBS 录制插件

> 从 [hearthstone-obs-recorder](https://github.com/darksworm/hearthstone-obs-recorder/) 分叉，感谢原作者 darksworm。

文档: [English](README.md) | 中文文档

## 🚀 特性

1. 在炉石传说游戏开始和结束时自动启动和停止 OBS 录制。
2. 可以检测游戏中的空闲时间，并在这些空闲期间暂停录制。

## 📦 安装

1. 从[最新发布](https://github.com/darksworm/hearthstone-obs-recorder/releases/latest)下载插件 DLL 文件
2. 按照 [HDT 插件安装指南](https://github.com/HearthSim/Hearthstone-Deck-Tracker/wiki/Available-Plugins)进行安装

### 如何在 OBS Studio 中启用 WebSocket 服务器

1. 打开 OBS Studio 并导航到 **工具 > WebSocket 服务器设置**。
2. 在 **插件设置** 中点击 **启用 WebSocket 服务器**。
3. 将 **服务器端口** 设置为 4444 并禁用身份验证。
4. 点击底部的 **应用** 按钮保存配置