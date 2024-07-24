using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsCanvas : MonoBehaviour
{
    public Image RedTint;
    public Image WhiteFlashImage;
    public GameObject DemoScreenCanvas;
    public float desiredRedTintOpacity = 0;
    public Button _restartGameButton;

    public static EffectsCanvas EC;
    private void Awake()
    {
        EC = this;
        WhiteFlashImage.color = new Color(0, 0, 0, 0);
    }
    public void Start()
    {
        DemoScreenCanvas.SetActive(false);
        _restartGameButton.onClick.AddListener(() => RestartGame());
        //StartWhiteFlash();
        SetRedTint(0);
        //StartWhiteFlash();
    }
    private void Update()
    {
        SetRedTint(Mathf.Lerp(RedTint.color.a, desiredRedTintOpacity, 0.5f * Time.deltaTime));
    }
    public void StartWhiteFlash()
    {
        StartCoroutine(WhiteFlashEffect());
    }
    public void SetDesiredRedTint(float opacity)
    {
        desiredRedTintOpacity = opacity;
    }
    private void SetRedTint(float opacity)
    {
        RedTint.color = new Color(1, 0, 0, opacity);
    }
    public IEnumerator WhiteFlashEffect()
    {
        float fadeInDur = 6;
        for(float i = 0; i < fadeInDur; i += Time.deltaTime)
        {
            WhiteFlashImage.color = new Color(1,1,1, Mathf.Min(1, i/fadeInDur));
            yield return new WaitForFixedUpdate();
        }
        WhiteFlashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(1);
        float fadeOutDur = 0.5f;
        for (float i = 0; i < fadeOutDur; i += Time.deltaTime)
        {
            WhiteFlashImage.color = new Color(1, 1, 1, 1- Mathf.Min(1, i / fadeOutDur));
            yield return new WaitForFixedUpdate();
        }
        WhiteFlashImage.color = new Color(1, 1, 1, 0);
    }
    public IEnumerator ShowDemoEnd()
    {
        yield return new WaitForSeconds(15);
        REF.PCon._lockRotation = true;
        REF.PCon.lockInput = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DemoScreenCanvas.SetActive(true);
    }
    private void RestartGame()
    {
        print("DEMO RESTARTED");
        Loader.Load(Loader.Scene.GameScene);
    }
}
