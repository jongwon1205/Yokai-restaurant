using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;


    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        bgmSource.volume = volume;

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void SetVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }
}
