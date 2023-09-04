using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    //  Audio

    public AudioMixer _audioMixer;
    public Slider _masterVolumeSlider;
    public Slider _musicVolumeSlider;
    public Slider _SFXVolumeSlider;

    public enum AudioEnums
    {
        MasterVolume,
        MusicVolume,
        SFXVolume
    }
    private void Awake()
    {
        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(_masterVolumeSlider.value); });
        _musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(_musicVolumeSlider.value); });
        _SFXVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(_SFXVolumeSlider.value); });
    }
    private void Start()
    {
        InitAudio();
    }
    private void InitAudio()
    {
        if (!PlayerPrefs.HasKey(AudioEnums.MasterVolume.ToString())) PlayerPrefs.SetFloat(AudioEnums.MasterVolume.ToString(), 1);
        if (!PlayerPrefs.HasKey(AudioEnums.MusicVolume.ToString())) PlayerPrefs.SetFloat(AudioEnums.MusicVolume.ToString(), 0.5f);
        if (!PlayerPrefs.HasKey(AudioEnums.SFXVolume.ToString())) PlayerPrefs.SetFloat(AudioEnums.SFXVolume.ToString(), 0.5f);

        SetMasterVolume(PlayerPrefs.GetFloat(AudioEnums.MasterVolume.ToString()));
        SetMusicVolume(PlayerPrefs.GetFloat(AudioEnums.MusicVolume.ToString()));
        SetSFXVolume(PlayerPrefs.GetFloat(AudioEnums.SFXVolume.ToString()));
    }


    //  Sliders

    public void SetMasterVolume(float volume)
    {
        if (volume == 1)
        {
            _audioMixer.SetFloat(AudioEnums.MasterVolume.ToString(), 0);
        }
        else if (volume == 0)
        {
            _audioMixer.SetFloat(AudioEnums.MasterVolume.ToString(), -80);
        }
        else
        {
            _audioMixer.SetFloat(AudioEnums.MasterVolume.ToString(), Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(AudioEnums.MasterVolume.ToString(), volume);
        _masterVolumeSlider.SetValueWithoutNotify(volume);
    }
    public void SetMusicVolume(float volume)
    {
        if (volume == 1)
        {
            _audioMixer.SetFloat(AudioEnums.MusicVolume.ToString(), 0);
        }
        else if (volume == 0)
        {
            _audioMixer.SetFloat(AudioEnums.MusicVolume.ToString(), -80);
        }
        else
        {
            _audioMixer.SetFloat(AudioEnums.MusicVolume.ToString(), Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(AudioEnums.MusicVolume.ToString(), volume);
        _musicVolumeSlider.SetValueWithoutNotify(volume);
    }
    public void SetSFXVolume(float volume)
    {
        if (volume == 1)
        {
            _audioMixer.SetFloat(AudioEnums.SFXVolume.ToString(), 0);
        }
        else if (volume == 0)
        {
            _audioMixer.SetFloat(AudioEnums.SFXVolume.ToString(), -80);
        }
        else
        {
            _audioMixer.SetFloat(AudioEnums.SFXVolume.ToString(), Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(AudioEnums.SFXVolume.ToString(), volume);
        _SFXVolumeSlider.SetValueWithoutNotify(volume);
    }
}
