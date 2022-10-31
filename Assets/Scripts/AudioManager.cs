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

    private void Start()
    {
        InitAudio();
    }

    private void InitAudio()
    {
        if (!PlayerPrefs.HasKey("MasterVolume")) PlayerPrefs.SetFloat("MasterVolume", 1);
        if (!PlayerPrefs.HasKey("MusicVolume")) PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        if (!PlayerPrefs.HasKey("SFXVolume")) PlayerPrefs.SetFloat("SFXVolume", 0.5f);

        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));

        _masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(_masterVolumeSlider.value); });
        _musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(_musicVolumeSlider.value); });
        _SFXVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(_SFXVolumeSlider.value); });
    }


    //  Sliders

    public void SetMasterVolume(float volume)
    {
        if (_masterVolumeSlider.value == 1)
        {
            _audioMixer.SetFloat("MasterVolume", 0);
        }
        else if (_masterVolumeSlider.value == 0)
        {
            _audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
        _masterVolumeSlider.value = volume;
    }
    public void SetMusicVolume(float volume)
    {
        if (_musicVolumeSlider.value == 1)
        {
            _audioMixer.SetFloat("MusicVolume", 0);
        }
        else if (_musicVolumeSlider.value == 0)
        {
            _audioMixer.SetFloat("MusicVolume", -80);
        }
        else
        {
            _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        _musicVolumeSlider.value = volume;
    }
    public void SetSFXVolume(float volume)
    {
        if (_SFXVolumeSlider.value == 1)
        {
            _audioMixer.SetFloat("SFXVolume", 0);
        }
        else if (_SFXVolumeSlider.value == 0)
        {
            _audioMixer.SetFloat("SFXVolume", -80);
        }
        else
        {
            _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        _SFXVolumeSlider.value = volume;
    }
}
