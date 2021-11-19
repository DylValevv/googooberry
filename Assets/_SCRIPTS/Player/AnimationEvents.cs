using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public PlayerController player;

    /// <summary>
    /// slow the player to a stop for more impact on the third hit
    /// </summary>
    public void ThirdHit()
    {
        player.ThirdHitHelper(0, false);
        Invoke("ThirdHitPost", 0.85f);  // This line is to fix a bug where cancelling the third hit animation caused player to freeze, as it would skip that animation event flag

    }

    /// <summary>
    /// resume the player's movement speed
    /// </summary>
    public void ThirdHitPost()
    {
        player.ThirdHitHelper(0, true);
    }

    /// <summary>
    /// play foot noises
    /// </summary>
    public void Foot()
    {
        player.PlayFootSteps();
    }

    public void PlayChargeup()
    {
        player.PlayChargeup();
    }
}
