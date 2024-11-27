using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using UnityEngine;
using Random = System.Random;

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

    public string RandomAnim(List<string> list)
    {
        if (list.Count > 1)
        {
            Random random = new Random();
            return list.ElementAt(random.Next(0, list.Count));
        }
        if (list.Count == 0)
        {
            return "";
        }
        return list.First();
    }

    public static Bounds CalculateBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            bounds.Encapsulate(child.position);  // 扩展包围盒以包含子物体的位置
        }
        return bounds;
    }

    public static Transform FindTransform(Transform obj,string name)
    {
        for (int i = 0; i < obj.transform.childCount ; i++)
        {
            if (obj.transform.GetChild(i).name.StartsWith(name))
            {
                return obj.transform.GetChild(i);
            }
        }
        return null;
    }

    public List<string> SplitStr(string p)
    {
        var input = p.Replace("\r\n", string.Empty);
        input = input.Replace("\n", string.Empty);
        var result = new List<string>();
        var list = new List<int>();
        list = list.Union(IndexOfAll(input, ',')).ToList<int>();
        list = list.Union(IndexOfAll(input, '，')).ToList<int>();
        list = list.Union(IndexOfAll(input, '.')).ToList<int>();
        list = list.Union(IndexOfAll(input, '。')).ToList<int>();
        list = list.Union(IndexOfAll(input, '、')).ToList<int>();
        list = list.Union(IndexOfAll(input, '！')).ToList<int>();
        list = list.Union(IndexOfAll(input, '!')).ToList<int>();
        list = list.Union(IndexOfAll(input, '?')).ToList<int>();
        list = list.Union(IndexOfAll(input, '？')).ToList<int>();
        list.Sort();
        var last = 0;
        for (int i = 0; i < list.Count; i++) { 
            var element = list[i];
            var str = input.Substring(last, element - last);
            if(element + 1 <= input.Length && (input.Substring(element, 1) == "！" || input.Substring(element, 1) == "!" || input.Substring(element, 1) == "?" || input.Substring(element, 1) == "？"))
            {
                str += input.Substring(element, 1);
            }
            if (str.Length == 0) continue;
            str = str.Replace("*", string.Empty);
            str = str.Replace("-", string.Empty);
            result.Add(str);
            last = element + 1;
        }
        if(result.Count == 0)
        {
            result.Add(input);
        }
        return result;
    }

    private List<int> IndexOfAll(string input, char value)
    {
        var result = new List<int>();
        for (int i = 0; ;)
        {
            if (i >= input.Length) break;
            var index = input.IndexOf(value, i);
            if (index == -1) break;
            result.Add(index);
            i = index + 1;
        }
        return result;
    }
}
