using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptWindow : MonoBehaviour
{
    public TMP_InputField PromptTextUI;

    public static PromptWindow Instance;
    // Start is called before the first frame update

    void Start()
    {
        Instance = this;
    }

    public void SetText(string txt)
    {
        if (PromptTextUI == null) return;
        PromptTextUI.text = txt;
    }
}