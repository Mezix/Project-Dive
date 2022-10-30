using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolIcon : MonoBehaviour
{
    public Image _toolIconImage;
    public GameObject _toolIconParent;
    public Text _hotkeyText;

    public void Highlight(bool highlight)
    {
        if(highlight)
        {
            _toolIconParent.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else
        {
            _toolIconParent.transform.localScale = Vector3.one;
        }
    }
}