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

    public RectTransform _maxHealth; public float _maxHealthWidth;
    public RectTransform _currentHealth;  public float _currentHealthWidth;
    private void Awake()
    {
        REF.PlayerUI = this;
        _maxHealthWidth = _maxHealth.sizeDelta.x;
        _currentHealthWidth = _currentHealth.sizeDelta.x;
    }
    void Update()
    {
        //UpdateDamageIndicator();
    }
    public void UpdateHealthBar(float currentHealth, float maxHealth, float drainPercentage)
    {
        _currentHealth.sizeDelta = new Vector2(currentHealth/maxHealth *  _currentHealthWidth, _currentHealth.sizeDelta.y);
        _maxHealth.sizeDelta = new Vector2(drainPercentage * _maxHealthWidth, _maxHealth.sizeDelta.y);


        _leftFOV.anchoredPosition = new Vector3(-960 + 480 * (1 - currentHealth/maxHealth), 0);
        _rightFOV.anchoredPosition = new Vector3(960 - 480 * (1 - currentHealth / maxHealth), 0);
    }

    internal void SelectTool(int toolIndex)
    {
        throw new NotImplementedException();
    }

    internal void ChangeToolBarOpacity(bool v)
    {
        throw new NotImplementedException();
    }
}
