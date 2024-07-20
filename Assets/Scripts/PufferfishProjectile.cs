using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferfishProjectile : AProjectile
{
    uint SoundID;
    public override IEnumerator DespawnAnimation()
    {
        yield return null;
        base.DespawnAnimation();
        AkSoundEngine.StopPlayingID(SoundID);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SoundID = AkSoundEngine.PostEvent("Play_PufferfishProjectileSound", gameObject);
    }
}
