using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackChangerCollider : MonoBehaviour
{
    public static int soundtrackLevel = 0;
    public bool collided = false;
    public static GameObject PressureSoundtrackObject; // make sure we have the same obj
    private void Start()
    {
        if (!PressureSoundtrackObject)
        {
            PressureSoundtrackObject = gameObject;
            AkSoundEngine.PostEvent("Play_PressureSoundtrack", PressureSoundtrackObject);
            AkSoundEngine.PostEvent("Play_KillstreakSoundtrack", PressureSoundtrackObject);
            AkSoundEngine.SetSwitch("PressureSoundtrackSwitch", "Pressure0", PressureSoundtrackObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInChildren<PlayerController>() && !collided)
        {
            soundtrackLevel++;
            collided = true;
            AkSoundEngine.SetSwitch("PressureSoundtrackSwitch", "Pressure" + soundtrackLevel, PressureSoundtrackObject);
        }
    }
}
