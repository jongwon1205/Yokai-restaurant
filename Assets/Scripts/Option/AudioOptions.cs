using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BgmParam = "BGMVolume";
    private const string SfxParam = "SFXVolume";

    private void Start()
    {
        // 저장값 불러오기 (없으면 1)
        float bgmValue = PlayerPrefs.GetFloat(BgmParam, 1f);
        float sfxValue = PlayerPrefs.GetFloat(SfxParam, 1f);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        ApplyBgm(bgmValue);
        ApplySfx(sfxValue);

        bgmSlider.onValueChanged.AddListener(ApplyBgm);
        sfxSlider.onValueChanged.AddListener(ApplySfx);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(BgmParam, bgmSlider.value);
        PlayerPrefs.SetFloat(SfxParam, sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void Cancel()
    {
        float bgmValue = PlayerPrefs.GetFloat(BgmParam, 1f);
        float sfxValue = PlayerPrefs.GetFloat(SfxParam, 1f);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        ApplyBgm(bgmValue);
        ApplySfx(sfxValue);
    }

    private void ApplyBgm(float value)
    {
        mixer.SetFloat(BgmParam, ToDecibel(value));
    }

    private void ApplySfx(float value)
    {
        mixer.SetFloat(SfxParam, ToDecibel(value));
    }

    private float ToDecibel(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        return Mathf.Log10(value) * 20f;
    }
}
