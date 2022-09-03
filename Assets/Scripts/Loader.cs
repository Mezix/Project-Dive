using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class Loader {
    public enum Scene { GameScene, MenuScene }

    public static void Load(Scene scene)
    {
        if (scene.ToString() == "GameScene")
        {
        }
        if(scene.ToString() == "MenuScene")
        {
        }
        SceneManager.LoadScene(scene.ToString());
    }
}
