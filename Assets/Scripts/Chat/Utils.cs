using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Instance;

    private void Start()
    {
        Instance = this;
    }

    public void PushAlertWindow(string text)
    {
        var t = (GameObject)Instantiate(Resources.Load("Prefabs/ChatUI/AlertPanel"), this.gameObject.transform);
        t.GetComponent<AlertWindow>().SetText(text);
    }
}
