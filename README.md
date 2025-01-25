UmaAIChat
---

[中文文档在此](https://github.com/KLXLjun/UmaAIChat/blob/master/README-zh.md)

This project is based on [UmaViewer](https://github.com/katboi01/UmaViewer). Thanks to the developers of [UmaViewer](https://github.com/katboi01/UmaViewer)!

Additional libraries used: [UniWindowController](https://github.com/kirurobo/UniWindowController), [UnityWav](https://github.com/deadlyfingers/UnityWav). Thanks to their developers!

Unity application that makes it easy to view assets from Uma Musume: Pretty Derby.

Furthermore, you can engage in dialogue with the Umamusume. The features available and planned for development in this application are detailed in the to-do list at the bottom.

### Requirements/Installation
- For Users:
1. [Uma Musume: Pretty Derby DMM](https://dmg.umamusume.jp/) with full data download is required to run the viewer.
1. Confirm your file listing in C:\\Users\\*you*\AppData\LocalLow\Cygames\umamusume\ looks like this:
   * umamusume\
     * **meta**
     * master\
       * **master.mdb**
     * dat\
       - 2A\\...
       - 2B\\...
       - ...\\...
1. Download my other repository [UmaAIChatServer](https://github.com/KLXLjun/UmaAIChatServer) and configure it according to the usage instructions in the  readme.
1. Download the most recent UmaViewer.zip file from [Releases](https://github.com/katboi01/UmaViewer/releases/) tab.
1. Extract the archive anywhere, can be extracted over previous version.
1. Run the UmaViewer.exe

------------

- For Developers/Contributors
1. [Unity Hub](https://unity3d.com/get-unity/download) with [Unity Engine Version 2022.3.56f1](unityhub://2022.3.56f1/dd0c98481d00) is recommended. It should be possible to run it on newer 2022.3.X versions.
1. Clone or download and extract this repository.
1. Import and Open the project in Unity Hub, missing files should be automatically repaired.
1. Open the Assets/Scenes/Version2 scene.
   - note: If there are errors in the console, [JSON .NET For Unity](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347) may be required

### To-do list

||||
| ------------ | ------------ | ------------ |
| ✓ - Working | / - Incomplete  | x - Unsupported  |

|||
| ------------ | ------------ |
| Support for additional Umamusume. | /  |
| Recognize emotions based on questions and answers and change the Umamusume actions and expressions (customizable actions and expressions) | ✓  |
| Customizable prompt | / |
| Tutorial on customizing actions and expressions | / |
| Better interface layout and interaction | x |
| Can be used as a computer desktop pet | / |
