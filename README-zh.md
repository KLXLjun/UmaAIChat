UmaAIChat
---

该项目基于 [UmaViewer](https://github.com/katboi01/UmaViewer)，在此感谢 [UmaViewer](https://github.com/katboi01/UmaViewer) 的开发者们！

使用的额外库：[UniWindowController](https://github.com/kirurobo/UniWindowController)，[UnityWav](https://github.com/deadlyfingers/UnityWav)，感谢它们的开发者！

一款基于Unity开发的应用程序，旨在于轻松查看Uma Musume: Pretty Derby中的资产内容。

而且，你可以在此与马娘们进行对话。此应用可用和计划开发的功能在最下方的表格中。

### 要求/安装方法
- 普通用户:
1. 您需要安装并下载 [Uma Musume: Pretty Derby DMM](https://dmg.umamusume.jp/)  的全部数据。
1. 确认 C:\\Users\\*you*\AppData\LocalLow\Cygames\umamusume\ 目录下看起来像是这样的:
   * umamusume\
     * **meta**
     * master\
       * **master.mdb**
     * dat\
       - 2A\\...
       - 2B\\...
       - ...\\...
1. 下载我的另一个仓库 [UmaAIChatServer](https://github.com/KLXLjun/UmaAIChatServer) 并按照readme中的使用方法进行配置
1. 从 [Releases](https://github.com/KLXLjun/UmaAIChat/releases) 页面下载最新的程序压缩包。
1. 将压缩包解压到你想要放的任何位置。
1. 运行 UmaAIChat.exe 即可

------------

- 开发者/贡献者
1. 建议使用 [Unity Hub](https://unity3d.com/get-unity/download) 并安装 [Unity Engine Version 2020.3.24f1](unityhub://2022.3.24f1/334eb2a0b267) 进行编辑与开发，当然，使用的2020.3.X 版本也是可以的。
1. 下载我的另一个仓库 [UmaAIChatServer](https://github.com/KLXLjun/UmaAIChatServer) 并按照readme中的使用方法进行配置
1. 克隆或是下载并解压此仓库。
1. 在 Unity Hub 中导入克隆或是下载并解压的此仓库并打开，缺失的文件会自动生成。
1. 打开这个场景：Assets/Scenes/Version2 .
   - 注意: 如果你看的控制台有错误，你可能需要安装： [JSON .NET For Unity](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347) 

### 代办列表

||||
| ------------ | ------------ | ------------ |
| ✓ - 完全可用 | / - 不完全可用 | x - 不可用 |

|功能名称|可用程度|
| ------------ | ------------ |
| 支持更多的马娘 | /  |
| 根据提问与回复识别情感并改变马娘的动作与表情（可自定义动作与表情） | ✓  |
| 可自定义的提示词 | / |
| 自定义动作与表情的教程 | x |
| 更好的界面布局与交互 | / |
| 可作为电脑桌面宠物 | / |
