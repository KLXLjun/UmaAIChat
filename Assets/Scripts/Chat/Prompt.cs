using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    public static Prompt Instance;

    static string CharacterAnimationDir = System.IO.Path.Combine(Application.streamingAssetsPath, "Prompt");

    public static Dictionary<string, string> CharacterPrompt { get; private set; } = new Dictionary<string, string>();

    void Start()
    {
        Instance = this;
        var tmp = System.IO.Directory.GetFiles(CharacterAnimationDir);
        foreach (var file in tmp)
        {
            var charaName = Path.GetFileNameWithoutExtension(file);
            if (Path.GetExtension(file) != ".txt")
            {
                continue;
            }

            string readData = "";
            StreamReader str = File.OpenText(Path.Combine(CharacterAnimationDir, file));
            readData = str.ReadToEnd();
            CharacterPrompt.Add(charaName, readData);
        }
        Debug.Log($"人物提示词已载入，共{CharacterPrompt.Count}个");
    }

    public string GetPrompt(string CharaID)
    {
        CharacterPrompt.TryGetValue(CharaID, out var prompt);
        if (prompt == null)
        {
            Debug.LogWarning($"没有找到人物 {CharaID} 的提示词!");
            return string.Empty;
        }
        return prompt;
    }
}
