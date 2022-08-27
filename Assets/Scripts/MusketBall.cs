using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusketBall : AProjectile
{
    private void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
    }
}
