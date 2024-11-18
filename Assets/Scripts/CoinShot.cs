using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinShot : MonoBehaviour
{
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.up * Random.Range(5,11) + transform.forward *Random.Range(5,20), ForceMode.Impulse);
    }

}
