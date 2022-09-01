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

    public void Awake()
    {
        REF.PlayerUI = this;
        _maxHealthWidth = _maxHealth.sizeDelta.x;
        _currentHealthWidth = _currentHealth.sizeDelta.x;
    }
    public void Start()
    {
        SpeedLinesUI(false, 0.5f);
        vengeanceHUD.SetActive(false);
    }
    public void Update()
    {
        //UpdateDamageIndicator();
        UpdateDashBar(REF.PCon._timeSinceLastDash/REF.PCon._dashCooldown);
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

    //  Tools

    internal void SelectTool(int toolIndex)
    {
        throw new NotImplementedException();
    }

    internal void ChangeToolBarOpacity(bool v)
    {
        throw new NotImplementedException();
    }
}
