using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Chat;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public static VoicePlayer Instance;

    public AudioSource audioSource;
    private static WebClient Client => WebClient.Instance;

    private AsyncThreadSafeQueue<AudioClip> _AudioClipQueue = new AsyncThreadSafeQueue<AudioClip>();
    private AsyncThreadSafeQueue<Model.RequestStruct> _AudioProcessQueue = new AsyncThreadSafeQueue<Model.RequestStruct> ();
    private bool isProcess = false;
    private bool isPlaying = false;

    public async void PushTask(List<Model.RequestStruct> msg)
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
        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(audioClip.length);
        complete?.Invoke();
    }

    async void Generate(Model.RequestStruct item)
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
