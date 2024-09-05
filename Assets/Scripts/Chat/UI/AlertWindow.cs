using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertWindow : MonoBehaviour
{
    public TMP_Text text1;

    public void SetText(string text)
    {
        text1.text = text;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
