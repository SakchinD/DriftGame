using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseSound
{
    Click,
    Buy
}

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public struct KeyValueAudioClips
    {
        public BaseSound Key;
        public AudioClip Audio;
    }
    public List<KeyValueAudioClips> audioClips;

    [Serializable]
    public struct KeyValueCrossFade
    {
        public string Key;
        public AudioSource[] Audios;
    }
    public List<KeyValueCrossFade> audioSourcesCrossFade;

    [SerializeField] AudioSource sourseForClips;

    [SerializeField] private float toFade;
    [SerializeField] private float maxVolume;

    private AudioSource nowPlayed;
    private IEnumerator coroutine;

    public void PlayAudio(BaseSound key)
    {
        var obj = audioClips.Find(item => item.Key == key);
        sourseForClips.clip = obj.Audio;
        sourseForClips.Play();
    }

    public void StopAudio(string key)
    {
        sourseForClips.Stop();
    }

    public void StopCrossFadeMusic()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    public void PlayCrossFadeMusic(string name)
    {
        var obj = audioSourcesCrossFade.Find(item => item.Key == name);
        coroutine = PlayCrossFade(obj.Audios);
        StartCoroutine(coroutine);
    }

    IEnumerator PlayCrossFade(AudioSource[] audios)
    {
        bool isStart = false;
        int ran = UnityEngine.Random.Range(0, audios.Length);
        AudioSource played;
        if (nowPlayed == audios[ran])
        {
            played = ran < audios.Length - 1 ? audios[ran + 1] : audios[0];
        }
        else played = audios[ran];

        nowPlayed = played;
        played.volume = 0;
        played.Play();
        float t = 0;
        while (t <= toFade)
        {
            played.volume = Mathf.Lerp(0.0f, maxVolume, t / toFade);
            t += Time.deltaTime;
            yield return null;
        }
        float lent = played.clip.length;
        lent -= (toFade * 2);
        yield return new WaitForSeconds(lent);
        t = 0;
        while (t <= toFade)
        {
            played.volume = Mathf.Lerp(maxVolume, 0.0f, t / toFade);
            t += Time.deltaTime;
            if (t > (toFade / 2) && !isStart)
            {
                isStart = true;
                coroutine = PlayCrossFade(audios);
                StartCoroutine(coroutine);
            }
            yield return null;
        }
        played.Stop();
    }
}
