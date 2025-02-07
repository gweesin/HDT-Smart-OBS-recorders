# Hearthstone game Smart OBS recorder

> forked from [hearthstone-obs-recorder](https://github.com/darksworm/hearthstone-obs-recorder/), thanks to the original author darksworm.

Documentation: English | [中文文档](README.zh-CN.md)

To use this you will need:

1. [OBS v28+](https://obsproject.com/)(the latest version is recommended)
2. [Hearthstone Deck Tracker](https://hsreplay.net/downloads/)

## 🚀 Features

1. automatically starts and stops OBS recordings when a Hearthstone game starts and ends.
2. detect idle time during the game and will pause the recording during these idle periods.

## 📦 Install

1. Download the plugin DLL file from the [latest release](https://github.com/gweesin/HDT-Smart-OBS-recorders/releases/latest)
2. Follow the [HDT plugin installation guide](https://github.com/HearthSim/Hearthstone-Deck-Tracker/wiki/Available-Plugins)

### How to enable websocket server in OBS Studio

1. Open OBS Studio and navigate to **Tools > WebSocket Server Settings**.
2. Click **Enable Websocket Server** in the **Plugin Settings** zoom
3. Set **Server Port** as 4444 and disabled Authentication
4. Now Click the **Apply** button in the bottom zoom to save configuration

### Add transition between Pause and Resume in OBS Studio

!\[Experimental\] 

download ScreenSwitcher from [here](https://github.com/WarmUpTill/SceneSwitcher)

Learn more info from [wiki](https://github.com/WarmUpTill/SceneSwitcher/wiki)

discussions in [reddit - is it possible to automatically add transitions](https://www.reddit.com/r/obs/comments/uce4r8/is_it_possible_to_automatically_add_transitions/)
