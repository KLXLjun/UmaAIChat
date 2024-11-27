using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TMP_InputField InputField;
    public TMP_Text RequestField;
    public TMP_Dropdown Dropdown;
    public PromptWindow PromptWindowObject;
    public AutoScroll RequestAutoScroll;
    public Transform AnimDebugList;
    public Transform AnimDebug;

    private static UmaViewerBuilder Builder => UmaViewerBuilder.Instance;
    private static AnimationController AnimationController => AnimationController.Instance;
    private static UmaViewerUI UI => UmaViewerUI.Instance;
    private static Prompt prompt => Prompt.Instance;
    private static WebClient Client => WebClient.Instance;
    private static PromptWindow PromptWindow => PromptWindow.Instance;
    private static Utils Utils => Utils.Instance;
    private static CharacterInfo CharacterInfo => CharacterInfo.Instance;
    private static VoicePlayer VoicePlayer => VoicePlayer.Instance;

    public AudioSource audioSource;
    public static CharaEntry NowChara;
    private static string emotionStr = string.Empty;
    private bool RequestStatus = false;

    public void Start()
    {
        Builder.OnNormalUmaModelLoadComplete += UmaModelLoadComplete;
    }

    public async void UmaModelLoadComplete(CharaEntry chara)
    {
        if (chara == null)
        {
            NowChara = null;
            return;
        }
        Debug.Log($"{Builder.CurrentUMAContainer.CharaEntry.Id} {Builder.CurrentUMAContainer.CharaEntry.EnName} {Builder.CurrentUMAContainer.CharaEntry.Name}");
        AnimationController.ChangeDefaultIdleAnim($"anm_eve_chr{Builder.CurrentUMAContainer.CharaEntry.Id}_00_idle01_loop");
        NowChara = chara;
        await Client.SetSystemPrompt(prompt.GetPrompt($"{chara.Id}"));
        AnimationController.UmaModelLoad(chara);
        int i = 0;
        foreach (var item in AnimationController.Actions)
        {
            if (i > 0)
            {
                emotionStr += ",";
            }
            emotionStr += item.Key;
            i++;
        }
        AnimDebugListUpdate();

    }

    public void ReloadAnimDebugList()
    {
        CharacterInfo.LoadAllActions();
        int i = 0;
        foreach (var item in AnimationController.Actions)
        {
            if (i > 0)
            {
                emotionStr += ",";
            }
            emotionStr += item.Key;
            i++;
        }
        AnimDebugListUpdate();
    }

    public void AnimDebugListUpdate()
    {
        while (AnimDebugList.childCount > 0)
        {
            DestroyImmediate(AnimDebugList.GetChild(0).gameObject);
        }

        foreach (var item in AnimationController.Actions)
        {
            GameObject ui = Instantiate(Resources.Load<GameObject>("Prefabs/ChatUI/Button"), AnimDebugList);
            var t = ui.GetComponent<UIButton>();
            t.button.onClick.AddListener(() =>
            {
                AnimationController.ChangeAction(item.Key);
            });
            t.text.text = item.Key;
        }
    }

    public async void SendMessage()
    {
        if (InputField.text == string.Empty)
        {
            return;
        }
        RequestStatus = true;
        AnimationController.SetRequestStatus(true);
        RequestField.text = "...";
        if (AnimationController.HasAction("Think"))
        {
            AnimationController.ChangeAction("Think");
        }
        else
        {
            if (AnimationController.HasAction("思考"))
            {
                AnimationController.ChangeAction("思考");
            }
        }
        
        var response = await Client.Chat(InputField.text, emotionStr, Dropdown.value);
        if (string.IsNullOrEmpty(response))
        {
            AnimationController.SetRequestStatus(false);
            return;
        }
        else
        {
            JToken json = JToken.Parse(response);
            var msg = json.Value<string>("Message");
            var act = json.Value<string>("Emotion");
            var translate = json.Value<string>("Translate");
            VoicePlayer.PushTask(Utils.SplitStr(msg));

            RequestField.text = "";

            StartCoroutine(TypeWriter(translate != "" ? translate : msg, () =>
            {
                AnimationController.SetRequestStatus(false);
            }));

            AnimationController.ChangeAction(act);
        }
    }

    IEnumerator TypeWriter(string content, Action complete)
    {
        foreach (var item in content)
        {
            RequestField.text += item;
            RequestAutoScroll.UpdateScrollView();
            yield return new WaitForSeconds(0.15f);
        }
        complete?.Invoke();
    }

    public void TogglePromptWindow()
    {
        if(PromptWindowObject == null)
        {
            return;
        }
        if(NowChara == null)
        {
            Utils.PushAlertWindow($"No character has been loaded!\n没有加载任何角色！");
            return;
        }
        if (!PromptWindowObject.gameObject.activeSelf)
        {
            PromptWindowObject.SetText(prompt.GetPrompt($"{NowChara.Id}"));
        }
        PromptWindowObject.gameObject.SetActive(!PromptWindowObject.gameObject.activeSelf);
    }

    public void ToggleDebugWindow()
    {
        if (AnimDebug == null)
        {
            return;
        }
        AnimDebug.gameObject.SetActive(!AnimDebug.gameObject.activeSelf);
    }

    public void SavePrompt()
    {
        if (PromptWindowObject == null || NowChara == null)
        {
            Utils.PushAlertWindow($"No character has been loaded!\n没有加载任何角色！");
            return;
        }
        prompt.SetPrompt($"{NowChara.Id}",PromptWindowObject.PromptTextUI.text);
    }

    public void Exit()
    {
        UnityEngine.Application.Quit(0);
    }
}
