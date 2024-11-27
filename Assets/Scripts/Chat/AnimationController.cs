using Gallop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static AnimStruct;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance;
    private static UmaViewerBuilder Builder => UmaViewerBuilder.Instance;
    private static UmaViewerMain Main => UmaViewerMain.Instance;
    private static UmaAssetManager UmaAssetManager => UmaAssetManager.instance;
    private static CharacterInfo CharacterInfo => CharacterInfo.Instance;
    private static Utils Utils => Utils.Instance;

    private bool RequestStatus = false;

    public CharaEntry NowChara;

    private bool loadStatus = false;
    public List<AnimStruct.Action> Actions;

    private bool winkRunning = true;

    private List<string> LastEmotion = new List<string>();

    private List<Gallop.FacialMorph> EyeControll = new List<Gallop.FacialMorph>();

    private string defaultIdle = "anm_eve_chr1001_00_idle01_loop";

    private float idleTime = 6.0f;
    private float MaxidleTime = 6.0f;

    public void Start()
    {
        Instance = this;

        UmaAssetManager.OnLoadProgressChange += LoadingStatusChange;
    }

    public void LoadingStatusChange(int curren, int target, string message = null)
    {
        loadStatus = curren != -1;
    }

    public void ResetIdleTime()
    {
        idleTime = MaxidleTime;
        enterIdleMode = false;
    }

    public void SetRequestStatus(bool input)
    {
        RequestStatus = input;
    }

    public void UmaModelLoad(CharaEntry chara)
    {
        if (chara == null)
        {
            return;
        }
        if (Builder.CurrentUMAContainer == null)
        {
            Utils.PushAlertWindow($"No character has been loaded!\n没有加载任何角色！");
            return;
        }

        enterIdleMode = true;
        Actions = CharacterInfo.GetActions($"{chara.Id}");

        EyeControll.Clear();
        foreach (var item in Builder.CurrentUMAContainer.FaceDrivenKeyTarget.EyeMorphs)
        {
            if (item.tag == "CloseA")
            {
                EyeControll.Add(item);
            }
        }

        Debug.Log($"{chara.Id}");
    }

    public void ChangeDefaultIdleAnim(string AnimName)
    {
        defaultIdle = AnimName;
    }

    public bool HasAction(string ActionName)
    {
        return Actions.FirstOrDefault(item => item.Key == ActionName) != null;
    }

    public void ChangeAction(string ActionName)
    {
        var act = Actions.FirstOrDefault(item => item.Key == ActionName);
        if (act == null)
        {
            Debug.LogWarning($"未找到 {ActionName} 动作");
            Utils.PushAlertWindow($"A animation file named '{ActionName}' could not be found.\n名称为 '{ActionName}' 的动画文件未找到。");
            return;
        }
        Debug.Log($"切换动作 {ActionName}");
        winkRunning = act.Wink;
        ClearAllWeight();
        ChangeMorph(act.Morph);
        ChangeMontion(act.Motion);
        ChangeAnim(Utils.RandomAnim(act.PlayAnim));
        ResetIdleTime();
    }

    public void ChangeAnim(string AnimName)
    {
        if (Builder.CurrentUMAContainer == null)
        {
            Utils.PushAlertWindow($"No character has been loaded!\n没有加载任何角色！");
            return;
        }

        var montion = Main.AbMotions.FirstOrDefault(motion => Path.GetFileName(motion.Name) == AnimName);
        if (montion == null)
        {
            Debug.LogError($"A resource file named '{AnimName}' could not be found.");
            Utils.PushAlertWindow($"A resource file named '{AnimName}' could not be found.\n名称为 '{AnimName}' 的资源文件未找到。");
            return;
        }
        Debug.Log($"切换动作 {AnimName}");
        UmaAssetManager.LoadAssetBundle(montion);
        Builder.CurrentUMAContainer.LoadAnimation(montion);
    }

    public void ChangeMontion(List<MotionGroup> motions)
    {
        //ResetMontion();
        foreach (var emotion in Builder.CurrentUMAContainer.FaceEmotionKeyTarget.FaceEmotionKey)
        {
            if (emotion.label == "Base")
            {
                continue;
            }
            var element = motions.FirstOrDefault(motion => motion.Name == emotion.label);
            if (element == null)
            {
                continue;
            }
            if (!string.IsNullOrEmpty(element.Name))
            {
                LastEmotion.Add(emotion.label);
                Builder.CurrentUMAContainer.FaceEmotionKeyTarget.ChangeEmotionWeight(emotion, element.Weight);
            }
        }
    }

    public void ChangeMorph(List<MorphGroup> morphs)
    {
        foreach (var item in morphs)
        {
            var tmp = Builder.CurrentUMAContainer.FaceDrivenKeyTarget.AllMorphs.Where(item => morphs.Any(other => other.Name == item.name || other.Name == item.tag)).ToList();
            foreach (var item1 in tmp)
            {
                Builder.CurrentUMAContainer.FaceDrivenKeyTarget.ChangeMorphWeight(item1, item.Weight);
            }
        }
    }

    public void ResetMontion()
    {
        foreach (var emotion in Builder.CurrentUMAContainer.FaceEmotionKeyTarget.FaceEmotionKey)
        {
            var lastElement = LastEmotion.FirstOrDefault(motion => motion == emotion.label);
            if (!string.IsNullOrEmpty(lastElement))
            {
                Builder.CurrentUMAContainer.FaceEmotionKeyTarget.ChangeEmotionWeight(emotion, 0.0f);
            }
        }
        LastEmotion.Clear();
    }

    public void ClearAllWeight()
    {
        if (Builder.CurrentUMAContainer == null) return;
        Builder.ClearMorphs();
        //Builder.CurrentUMAContainer.FaceDrivenKeyTarget.ClearAllWeights();
        //Builder.CurrentUMAContainer.FaceEmotionKeyTarget.UpdateAllTargetWeight();
        //Builder.CurrentUMAContainer.FaceEmotionKeyTarget.Initialize();
    }

    private float waitTime = 5.0f;
    private float waitTimeMax = 5.0f;
    private bool isCloseEye = false;
    private float closeEyeValue = 0.0f;
    private bool enterIdleMode = true;
    private void Update()
    {
        if (loadStatus)
        {
            return;
        }
        if (!RequestStatus && idleTime > 0 && enterIdleMode == false)
        {
            idleTime -= Time.deltaTime;
            if (idleTime < 0)
            {
                ClearAllWeight();
                ChangeAnim(defaultIdle);
                Builder.CurrentUMAContainer.FaceDrivenKeyTarget.ChangeMorphWeight(Builder.CurrentUMAContainer.FaceDrivenKeyTarget.MouthMorphs[3], 1);
                enterIdleMode = true;
            }
        }
        if (NowChara != null && winkRunning)
        {
            //处理眨眼的部分
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                if (!isCloseEye)
                {
                    closeEyeValue -= (Time.deltaTime / 0.15f);
                    if (closeEyeValue < 0.0f)
                    {
                        closeEyeValue = 0.0f;
                    }
                }
                else
                {
                    closeEyeValue += (Time.deltaTime / 0.15f);
                    if (closeEyeValue > 1.0f)
                    {
                        closeEyeValue = 1.0f;
                    }
                }
                foreach (var item in EyeControll)
                {
                    Builder.CurrentUMAContainer.FaceDrivenKeyTarget.ChangeMorphWeight(item, closeEyeValue);
                }
            }
            else
            {
                if (isCloseEye)
                {
                    waitTime = waitTimeMax;
                    isCloseEye = false;
                }
                else
                {
                    waitTime = 0.15f;
                    isCloseEye = true;
                }
            }
        }
    }
}
