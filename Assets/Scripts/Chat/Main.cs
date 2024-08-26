using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Procedural;

public class Main : MonoBehaviour
{
    public TMP_InputField InputField;
    public TMP_InputField RequestField;
    private static UmaViewerBuilder Builder => UmaViewerBuilder.Instance;
    private static AnimationController AnimationController => AnimationController.Instance;
    private static Prompt prompt => Prompt.Instance;
    private static WebClient Client => WebClient.Instance;

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
        var response = await Client.Chat(InputField.text, emotionStr);
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
            AnimationController.ChangeAction(act);
            RequestField.text = "";
            StartCoroutine(TypeWriter(msg, () =>
            {
                AnimationController.SetRequestStatus(false);
            }));
        }
    }

    IEnumerator TypeWriter(string content, Action complete)
    {
        foreach (var item in content)
        {
            RequestField.text += item;
            yield return new WaitForSeconds(0.15f);
        }
        complete?.Invoke();
    }
}
