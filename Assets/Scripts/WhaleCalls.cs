using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleCalls : MonoBehaviour
{
    public const int whaleCallDuration = 30;
    public float timeSinceLastWhaleCall = whaleCallDuration;
    public float timeUntilNextWhaleCall = whaleCallDuration;

    void Start()
    {
        
    }
    void Update()
    {
        timeSinceLastWhaleCall += Time.deltaTime;
        if(timeSinceLastWhaleCall >= timeUntilNextWhaleCall)
        {
            AkSoundEngine.PostEvent("Play_Whale_Call", gameObject);
            timeSinceLastWhaleCall = 0;
            timeUntilNextWhaleCall = whaleCallDuration + UnityEngine.Random.Range(15, 60);
        }
    }
}
