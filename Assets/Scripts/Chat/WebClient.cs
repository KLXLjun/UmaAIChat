using System;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class WebClient : MonoBehaviour
{
    public static WebClient Instance;
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
            result = string.Empty;
        }
        return result;
    }

    public async Task<string> Chat(string content, string emotion)
    {
        var postContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("prompt", content),
            new KeyValuePair<string, string>("emotion", emotion)
        });

        string result = "";
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(APIEndPoint + "/chat", postContent);
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
            result = string.Empty;
        }
        return result;
    }
}
