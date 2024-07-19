using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundsOrigin : MonoBehaviour
{
    internal void PlaySound(string soundID)
    {
        AkSoundEngine.PostEvent(soundID, gameObject);
    }
}
