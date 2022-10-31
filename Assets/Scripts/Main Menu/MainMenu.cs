using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button _startGameButton;
    public Button _quitGameButton;

    //  Settings
    public Button _openSettingsButton;
    public Button _closeSettingsButton;
    public GameObject _settingsMenu;
    public bool _settingsOn;

    //  Credits
    public Button _openCreditsButton;
    public Button _closeCreditsButton;
    public GameObject _creditsMenu;
    public bool _creditsOn;

    private void Start()
    {
        OpenSettings(false);
        OpenCredits(false);
        InitButtons();
    }

    private void InitButtons()
    {
        _startGameButton.onClick.AddListener(() => LoadGame());
        _openSettingsButton.onClick.AddListener(() => OpenSettings(true));
        _closeSettingsButton.onClick.AddListener(() => OpenSettings(false));
        _openCreditsButton.onClick.AddListener(() => OpenCredits(true));
        _closeCreditsButton.onClick.AddListener(() => OpenCredits(false));
        _quitGameButton.onClick.AddListener(() => QuitGame());
    }

    private void LoadGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    private void ToggleSettings()
    {
        _settingsOn = !_settingsOn;
        OpenSettings(_settingsOn);
    }
    public void OpenSettings(bool s)
    {
        _settingsOn = s;
        _settingsMenu.SetActive(s);
        if (s)
        {
        }
        else
        {

        }
    }

    private void ToggleCredits()
    {
        _creditsOn = !_creditsOn;
        OpenCredits(_creditsOn);
    }
    public void OpenCredits(bool c)
    {
        _creditsOn = c;
        _creditsMenu.SetActive(c);
        if(c)
        {

        }
        else
        {

        }
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
