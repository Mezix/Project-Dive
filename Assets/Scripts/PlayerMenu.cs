using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour
{
    public Button _returnToMainMenuButton;
    public Button _closeMenuButton;
    public GameObject _menuParent;
    public bool _menuOn;

    //  Settings
    public Button _openSettingsButton;
    public Button _closeSettingsButton;
    public GameObject _settingsMenu;
    public bool _settingsOn;
    public Slider _sensitivitySlider;

    //  Weapon Specific things

    public Button _cavLanceToggleButton;
    public Button _cavLanceHoldButton;
    public void Start()
    {
        InitUI();
        _menuOn = false;
        MenuOn(_menuOn);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (_settingsOn)
            {
                OpenSettings(false);
                return;
            }
            _menuOn = !_menuOn;
            MenuOn(_menuOn);
        }
    }
    private void InitUI()
    {
        _closeMenuButton.onClick.AddListener(() => MenuOn(false));
        _returnToMainMenuButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MenuScene));
        _openSettingsButton.onClick.AddListener(() => OpenSettings(true));
        _closeSettingsButton.onClick.AddListener(() => OpenSettings(false));
        _sensitivitySlider.onValueChanged.AddListener(delegate { SetSensitivity(_sensitivitySlider.value); });
        _cavLanceToggleButton.onClick.AddListener(() => SetCavLanceToggle(true));
        _cavLanceHoldButton.onClick.AddListener(() => SetCavLanceToggle(false));

        //  Sens

        PlayerPrefs.SetFloat("MinSens", 100);
        PlayerPrefs.SetFloat("MaxSens", 1000);
        if (!PlayerPrefs.HasKey("MouseSens")) PlayerPrefs.SetFloat("MouseSens", PlayerPrefs.GetFloat("MinSens"));
        SetSensitivity(PlayerPrefs.GetFloat("MouseSens") / PlayerPrefs.GetFloat("MaxSens"));

        _sensitivitySlider.minValue = PlayerPrefs.GetFloat("MinSens") / PlayerPrefs.GetFloat("MaxSens");
        _sensitivitySlider.maxValue = 1;
        _sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MouseSens") / PlayerPrefs.GetFloat("MaxSens"));

        //  Misc
        if (!PlayerPrefs.HasKey("ToggleOrHoldCavLance")) PlayerPrefs.SetString("ToggleOrHoldCavLance", "true");
        SetCavLanceToggle(HM.StringToBool(PlayerPrefs.GetString("ToggleOrHoldCavLance")));
    }

    private void MenuOn(bool menuOn)
    {
        _menuParent.gameObject.SetActive(menuOn);
        if(menuOn)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            REF.PCon._lockRotation = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            REF.PCon._lockRotation = false;

            OpenSettings(false);
        }
    }

    //  Settings
    public void OpenSettings(bool s)
    {
        _settingsOn = s;
        _settingsMenu.SetActive(s);
    }

    //  Misc
    public void SetSensitivity(float sliderPct)
    {
        _sensitivitySlider.SetValueWithoutNotify(sliderPct);
        PlayerPrefs.SetFloat("MouseSens", sliderPct * PlayerPrefs.GetFloat("MaxSens"));
        REF.PCon._currentSensitivity = PlayerPrefs.GetFloat("MouseSens");
    }

    private void SetCavLanceToggle(bool toggle)
    {
        PlayerPrefs.SetString("ToggleOrHoldCavLance", toggle.ToString());
        _cavLanceHoldButton.interactable = toggle;
        _cavLanceToggleButton.interactable = !toggle;
    }
}
