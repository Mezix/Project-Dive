using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Button _startGameButton;
    public Button _quitGameButton;

    //  Settings
    public Button _openSettingsButton;
    public Button _closeSettingsButton;
    public GameObject _settingsMenu;
    public bool _settingsOn;

    public Slider _sensitivitySlider;

    //  Credits
    public Button _openCreditsButton;
    public Button _closeCreditsButton;
    public GameObject _creditsMenu;
    public bool _creditsOn;


    //  Weapon Specific things

    public Button _cavLanceToggleButton;
    public Button _cavLanceHoldButton;


    public void Start()
    {
        OpenSettings(false);
        OpenCredits(false);
        InitPlayerPrefabs();
        InitUIInteraction();
    }

    private void InitPlayerPrefabs()
    {
        PlayerPrefs.SetFloat("MinSens", 100);
        PlayerPrefs.SetFloat("MaxSens", 1000);
        if (!PlayerPrefs.HasKey("MouseSens"))    PlayerPrefs.SetFloat("MouseSens", PlayerPrefs.GetFloat("MinSens"));
        _sensitivitySlider.minValue = PlayerPrefs.GetFloat("MinSens") / PlayerPrefs.GetFloat("MaxSens");
        _sensitivitySlider.maxValue = 1;
        _sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSens") / PlayerPrefs.GetFloat("MaxSens"));

        if (!PlayerPrefs.HasKey("ToggleOrHoldCavLance")) PlayerPrefs.SetString("ToggleOrHoldCavLance", "true");
        SetCavLanceToggle(HM.StringToBool(PlayerPrefs.GetString("ToggleOrHoldCavLance")));
    }

    private void InitUIInteraction()
    {
        _startGameButton.onClick.AddListener(() => LoadGame());
        _openSettingsButton.onClick.AddListener(() => OpenSettings(true));
        _closeSettingsButton.onClick.AddListener(() => OpenSettings(false));
        _openCreditsButton.onClick.AddListener(() => OpenCredits(true));
        _closeCreditsButton.onClick.AddListener(() => OpenCredits(false));
        _quitGameButton.onClick.AddListener(() => QuitGame());

        _sensitivitySlider.onValueChanged.AddListener(delegate { SetSensitivity(_sensitivitySlider.value); });
        _cavLanceToggleButton.onClick.AddListener(() => SetCavLanceToggle(true));
        _cavLanceHoldButton.onClick.AddListener(() => SetCavLanceToggle(false));
    }

    private void LoadGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }
    public void OpenSettings(bool s)
    {
        _settingsOn = s;
        _settingsMenu.SetActive(s);
    }

    public void OpenCredits(bool c)
    {
        _creditsOn = c;
        _creditsMenu.SetActive(c);
    }

    //  Sensitivity

    public void SetSensitivity(float sliderPct)
    {
        _sensitivitySlider.SetValueWithoutNotify(sliderPct);
        PlayerPrefs.SetFloat("MouseSens", sliderPct * PlayerPrefs.GetFloat("MaxSens"));
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    private void SetCavLanceToggle(bool toggle)
    {
        PlayerPrefs.SetString("ToggleOrHoldCavLance", toggle.ToString());
        _cavLanceHoldButton.interactable = toggle;
        _cavLanceToggleButton.interactable = !toggle;
    }
}
