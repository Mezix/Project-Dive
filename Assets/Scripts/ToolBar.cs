using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
    public List<ToolIcon> _toolIcons = new List<ToolIcon>();
    public List<ToolIcon> _circularToolIcons = new List<ToolIcon>();

    public HorizontalLayoutGroup _hLayoutGroup;
    public GameObject _circularParent;
    private float toolIconOffset;
    private int selectedIndex;
    private void Awake()
    {
        toolIconOffset = 150;
        selectedIndex = -1;
    }
    private void Update()
    {
        if(_toolIcons.Count > 1 && !REF.PlayerUI._menu._menuOn)
        {
            if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                TryToSelectWeapon();
            }

            if (Input.GetKey(KeyCode.Mouse2))
            {
                _circularParent.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0.5f;
                REF.PCon._lockRotation = true;
                HoverCircularMenu();
            }
            else
            {
                _circularParent.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                REF.PCon._lockRotation = false;
            }
        }
    }

    private void HoverCircularMenu()
    {
        Vector2 mousePos = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
        if (mousePos.magnitude >= (toolIconOffset - 50))
        {
            float angle = HM.GetAngle2DBetween(Vector3.zero, mousePos);
            angle += 180;

            selectedIndex = -1;
            int index = 0;
            foreach (ToolIcon icon in _circularToolIcons)
            {
                //Debug.Log(icon._toolIconParent.transform.localRotation.eulerAngles.z);
                float diff = icon.transform.localRotation.eulerAngles.z - angle;
                if (Mathf.Abs(diff) < 15)
                {
                    selectedIndex = index;
                    break;
                }
                index++;
            }
        }
        HighlightWeapon(selectedIndex);
    }

    private void TryToSelectWeapon()
    {
        if (selectedIndex >= 0) REF.PCon.SelectToolFromToolbar(selectedIndex);
    }

    private void HighlightWeapon(int selectedIndex)
    {
        foreach(ToolIcon icon in _circularToolIcons)
        {
            icon.Highlight(false);
        }
        if(selectedIndex >= 0) _circularToolIcons[selectedIndex].Highlight(true);
    }

    public void SpawnToolIcons(List<AWeapon> weapons)
    {
        float count = weapons.Count;
        int index = 0;
        foreach (AWeapon wep in weapons)
        {
            //  Normal UI

            GameObject g = (GameObject) Instantiate(Resources.Load("Weapons/ToolIcon"));
            ToolIcon t = g.GetComponentInChildren<ToolIcon>();
            t._toolIconImage.sprite = wep._toolIcon;
            t._hotkeyText.text = (index+1).ToString();
            g.transform.SetParent(_hLayoutGroup.transform, false);
            _hLayoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(count * 100, 100);
            _toolIcons.Add(t);

            //  Circular UI

            g = (GameObject) Instantiate(Resources.Load("Weapons/CircularToolIcon"));
            t = g.GetComponentInChildren<ToolIcon>();
            t._toolIconImage.sprite = wep._toolIcon;
            t._hotkeyText.text = (index+1).ToString();
            t._toolIconParent.transform.localPosition = new Vector3(toolIconOffset, 0, 0);
            g.transform.SetParent(_circularParent.transform, false);
            if(index > 0)
            {
                HM.RotateLocalTransformToAngle(g.transform, new Vector3(0, 0, index * (360 / count)));
                HM.RotateLocalTransformToAngle(t._toolIconParent.transform, new Vector3(0, 0, index * (-360 / count)));
            }
            _circularToolIcons.Add(t);
            index++;
        }
    }
}
