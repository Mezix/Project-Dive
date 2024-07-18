using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackChangerCollider : MonoBehaviour
{
    public static int soundtrackLevel = 0;
    public bool collided = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInChildren<PlayerController>() && !collided)
        {
            soundtrackLevel++;
            collided = true;
            Debug.Log(soundtrackLevel);
        }
    }
}
