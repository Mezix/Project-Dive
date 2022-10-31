using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider _sensitivitySlider;

    public RectTransform _leftFOV;
    public RectTransform _rightFOV;

    public RectTransform _maxHealth; private float _maxHealthWidth;
    public RectTransform _currentHealth; private float _currentHealthWidth;

    //  VengeanceMode
    [SerializeField] private GameObject vengeanceHUD;

    //  Speedlines
    [SerializeField] private GameObject speedLinesObj;
    [SerializeField] private Image speedLinesImage;

    //  Dash
    [SerializeField] private Image dashBG;
    [SerializeField] private Image dashFill;

    //  Speed Display
    [SerializeField] private Text _speedTextDisplay;

    //  Death Screen
    public GameObject _deathUI;
    public Image _deathOverlay;
    public Text _gameOverText;
    public Button _restartGameButton;

    public ToolBar _toolbar;
    public PlayerMenu _menu;

    public void Awake()
    {
        REF.PlayerUI = this;
        _maxHealthWidth = _maxHealth.sizeDelta.x;
        _currentHealthWidth = _currentHealth.sizeDelta.x;

        InitButtons();
    }

    private void InitButtons()
    {
        _restartGameButton.onClick.AddListener(() => RestartGame());
    }

    public void Start()
    {
        SpeedLinesUI(false, 0.5f);
        vengeanceHUD.SetActive(false);
        _deathUI.gameObject.SetActive(false);
    }
    public void Update()
    {
        //UpdateDamageIndicator();
        UpdateDashBar(REF.PCon._timeSinceLastDash/REF.PCon._dashCooldown);
        UpdateSpeedDisplay();
}

    private void UpdateSpeedDisplay()
    {
        _speedTextDisplay.text = HM.FloatToString(REF.PCon._playerRB.velocity.magnitude / 15f, 1);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth, float drainPercentage)
    {
        _currentHealth.sizeDelta = new Vector2(currentHealth/maxHealth *  _currentHealthWidth, _currentHealth.sizeDelta.y);
        _maxHealth.sizeDelta = new Vector2(drainPercentage * _maxHealthWidth, _maxHealth.sizeDelta.y);


        _leftFOV.anchoredPosition = new Vector3(-960 + 480 * (1 - currentHealth / maxHealth), 0);
        _rightFOV.anchoredPosition = new Vector3(960 - 480 * (1 - currentHealth / maxHealth), 0);
    }

    public void UpdateDashBar(float currentDashPct)
    {
        currentDashPct = Mathf.Min(1, currentDashPct);
        dashFill.fillAmount = currentDashPct;
    }

    public void VeangenceModeUI(bool on)
    {
        vengeanceHUD.gameObject.SetActive(on);
    }
    public void SpeedLinesUI(bool on, float transparency = 1)
    {
        speedLinesObj.SetActive(on);
        HM.RotateLocalTransformToAngle(speedLinesImage.transform, new Vector3(0,0,180));
        speedLinesImage.color = new Color(1,1,1,transparency);
    }

    public void InitiateDeathUI()
    {
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        _restartGameButton.gameObject.SetActive(false);
        _deathUI.gameObject.SetActive(true);
        for(int i = 0; i < 60; i++)
        {
            _deathOverlay.color = new Color(0, 0, 0, i / 60f);
            _gameOverText.color = new Color(1, 1, 1, i / 60f);
            yield return new WaitForFixedUpdate();
        }
        _deathOverlay.color = new Color(0, 0, 0, 1);
        _gameOverText.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(2f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _restartGameButton.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        print("RESTART");
        Loader.Load(Loader.Scene.GameScene);
    }
    //  Tools

    internal void SelectTool(int toolIndex)
    {
        //throw new NotImplementedException();
    }

    internal void ChangeToolBarOpacity(bool v)
    {
        //throw new NotImplementedException();
    }

}
