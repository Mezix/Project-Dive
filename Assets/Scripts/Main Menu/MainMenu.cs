using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button _startGameButton;

    private void Start()
    {
        _startGameButton.onClick.AddListener(() => Loader.Load(Loader.Scene.GameScene));
    }
}
