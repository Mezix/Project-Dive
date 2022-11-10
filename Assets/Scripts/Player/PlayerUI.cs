using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("FOV")]
    public RectTransform _leftFOV;
    public RectTransform _rightFOV;

    [Header("Health")]
    public Image _maxHealthLeft;
    public Image _maxHealthRight;
    public Image _currentHealthLeft;
    public Image _currentHealthRight;

    [Header("Misc")]
    //  VengeanceMode
    [SerializeField] private GameObject vengeanceHUD;

    //  Speedlines
    [SerializeField] private GameObject speedLinesObj;
    [SerializeField] private Image speedLinesImage;

    //  Dash
    [SerializeField] private Image dashFill;

    //  Speed Display
    [SerializeField] private Text _speedTextDisplay;
    public Image _speedDisplayFill;

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
        UpdateDashBar(REF.PCon._timeSinceLastDash/REF.PCon._dashCooldown);
        UpdateSpeedDisplay();
}

    private void UpdateSpeedDisplay()
    {
        float speed = REF.PCon._playerRB.velocity.magnitude / 15f;
        _speedTextDisplay.text = HM.FloatToString(speed, 1);
        speed = Mathf.Min(speed, 5);
        _speedDisplayFill.fillAmount = speed / 5;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth, float drainPercentage)
    {
        _currentHealthLeft.fillAmount = currentHealth / maxHealth;
        _currentHealthRight.fillAmount = currentHealth / maxHealth;
        _maxHealthLeft.fillAmount = drainPercentage;
        _maxHealthRight.fillAmount = drainPercentage;


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
