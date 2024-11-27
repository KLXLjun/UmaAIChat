using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public static VoicePlayer Instance;

    public AudioSource audioSource;
    private static WebClient Client => WebClient.Instance;

    private AsyncThreadSafeQueue<AudioClip> _AudioClipQueue = new AsyncThreadSafeQueue<AudioClip>();
    private AsyncThreadSafeQueue<string> _AudioProcessQueue = new AsyncThreadSafeQueue<string> ();
    private bool isProcess = false;
    private bool isPlaying = false;

    public async void PushTask(List<string> msg)
    {
        await _AudioProcessQueue.ClearAndEnqueueRangeAsync(msg);
        await _AudioClipQueue.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    async void Update()
    {
        if (!isProcess) {
            var rsl = await _AudioProcessQueue.TryDequeueAsync();
            if(rsl != null)
            {
                Generate(rsl);
            }
        }

        if (!isPlaying) {
            var rsl = await _AudioClipQueue.TryDequeueAsync();
            if (rsl != null)
            {
                isPlaying = true;
                StartCoroutine(PlayVoice(rsl, () =>
                {
                    Thread.Sleep(300);
                    isPlaying = false;
                }));
            }
        }
    }

    IEnumerator PlayVoice(AudioClip audioClip, Action complete)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
        complete?.Invoke();
    }

    async void Generate(string item)
    {
        isProcess = true;
        var audioRAWData = await Client.GenerateVoice(item);
        if (audioRAWData != null)
        {
            AudioClip audioClip = WavUtility.ToAudioClip(audioRAWData);
            await _AudioClipQueue.EnqueueAsync(audioClip);
        }
        isProcess = false;
    }
}
