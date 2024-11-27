using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static AnimStruct;

public class CharacterInfo : MonoBehaviour
{
    public static CharacterInfo Instance;

    static string CharacterAnimationDir = System.IO.Path.Combine(Application.streamingAssetsPath, "CharacterAnimation");

    public bool IsLoadOk = false;
    public List<Action> Default;

    public Dictionary<string, List<Action>> CharacterAction { get; private set; } = new Dictionary<string, List<Action>>();

    public void Start()
    {
        Instance = this;
        LoadAllActions();
    }
    public void LoadAllActions()
    {
        var tmp = System.IO.Directory.GetFiles(CharacterAnimationDir);
        foreach (var file in tmp)
        {
            var charaID = Path.GetFileNameWithoutExtension(file);
            if (Path.GetExtension(file) != ".json")
            {
                continue;
            }

            string readData = "";
            StreamReader str = File.OpenText(Path.Combine(CharacterAnimationDir, file));
            readData = str.ReadToEnd();
            var group = new List<Action>();
            try
            {
                foreach (var item in JArray.Parse(readData))
                {
                    string name = item.Value<string>("Key");
                    int playType = item.Value<int>("PlayType");
                    bool wink = item.Value<bool?>("Wink") is null;

                    List<string> animGroup = new List<string>();
                    foreach (var item1 in (JArray)item["PlayAnim"])
                    {
                        animGroup.Add(item1.ToString());
                    }

                    List<MotionGroup> motionGroups = new List<MotionGroup>();
                    foreach (var item1 in item["Motion"])
                    {
                        string motionName = item1.Value<string>("Name");
                        float motionWeight = item1.Value<float>("Weight");
                        motionGroups.Add(new MotionGroup { Name = motionName, Weight = motionWeight });
                    }

                    List<MorphGroup> morphGroups = new List<MorphGroup>();
                    foreach (var item1 in item["Morph"])
                    {
                        string motionName = item1.Value<string>("Name");
                        float motionWeight = item1.Value<float>("Weight");
                        morphGroups.Add(new MorphGroup { Name = motionName, Weight = motionWeight });
                    }

                    group.Add(new Action
                    {
                        Key = name,
                        PlayType = playType,
                        Wink = wink,
                        PlayAnim = animGroup,
                        Motion = motionGroups,
                        Morph = morphGroups
                    });
                }
                CharacterAction.Add(charaID, group);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                continue;
            }
        }
        Debug.Log($"人物动作已载入，共{CharacterAction.Count}个");
        if (GetActions("Default") != null)
        {
            Default = GetActions("Default");
        }

        IsLoadOk = true;
    }

    public List<Action> GetActions(string EnName)
    {
        CharacterAction.TryGetValue(EnName, out var action);
        if (action == null)
        {
            if (Default != null)
            {
                Debug.LogWarning($"没有找到人物 {EnName} 的动作!已使用默认动作");
                return Default;
            }
            else
            {
                Debug.LogWarning($"没有找到人物 {EnName} 的动作!");
            }
        }
        return action;
    }
}
