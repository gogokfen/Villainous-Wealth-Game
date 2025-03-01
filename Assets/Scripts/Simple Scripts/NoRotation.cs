using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotation : MonoBehaviour
{
    //late update to prevent jittering
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
