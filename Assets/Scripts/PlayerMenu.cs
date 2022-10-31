using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour
{
    public Button _returnToMainMenuButton;
    public GameObject _menuParent;

    public bool _menuOn;
    private void Start()
    {
        _returnToMainMenuButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MenuScene));
        _menuOn = false;
        MenuOn(_menuOn);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _menuOn = !_menuOn;
            MenuOn(_menuOn);
        }
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
        }
    }
}
