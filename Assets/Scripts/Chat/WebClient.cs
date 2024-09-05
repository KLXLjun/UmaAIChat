using System;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public static WebClient Instance;
    private static Utils Utils => Utils.Instance;
    public static string APIEndPoint = "http://127.0.0.1:32679/v1";

    public void Start()
    {
        Instance = this;
    }

    public async Task<string> SetSystemPrompt(string content)
    {
        var postContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("prompt", content),
        });

        string result = "";
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(APIEndPoint + "/setSystemPrompt", postContent);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Utils.PushAlertWindow($"{ex.GetType().FullName}\n{ex.Message}\n\nPlease check whether the UmaAIChatServer is running properly.\n请检查“UmaAIChatServer”是否正常运行");
            result = string.Empty;
        }
        return result;
    }

    public async Task<string> Chat(string content, string emotion, int target)
    {
        var postContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("prompt", content),
            new KeyValuePair<string, string>("emotion", emotion),
            new KeyValuePair<string, string>("language", $"{target}")
        });

        string result = "";
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(APIEndPoint + $"/chat", postContent);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Utils.PushAlertWindow($"{ex.GetType().FullName}\n{ex.Message}\n\nPlease check whether the UmaAIChatServer configuration is correct.\n请检查“UmaAIChatServer”配置是否正确");
            result = string.Empty;
        }
        return result;
    }

    public async Task<byte[]> GenerateVoice(string content)
    {
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(APIEndPoint + $"/vits-fast?text={Uri.EscapeDataString(content)}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return result;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Utils.PushAlertWindow($"{ex.GetType().FullName}\n{ex.Message}\n\nPlease check whether the UmaAIChatServer configuration is correct.\n请检查“UmaAIChatServer”配置是否正确");
        }
        return null;
    }
}
