using System.Collections;
using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public void FootstepSound()
    {
        SoundManager.singleton.PlayClip("Footstep", transform.position, 0.025f, true, true);
    }
}
