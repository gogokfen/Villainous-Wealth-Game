using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpinner : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0.1f * Time.deltaTime, 0);
    }
}
