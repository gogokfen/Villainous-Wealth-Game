using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : WeaponBase
{
    [SerializeField] GameObject fistGFX;
    //private SphereCollider SC;
    private BoxCollider BC;
    bool sphere;

    private void Start()
    {
        //SC = transform.GetComponent<SphereCollider>();
        if (transform.GetComponent<BoxCollider>() != null)
            BC = transform.GetComponent<BoxCollider>();
        else
            sphere = true;
    }

    private void OnDisable()
    {
        //transform.GetComponent<SphereCollider>().enabled = false;
        if (!sphere)
            BC.enabled = false;
        else
            transform.GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        //transform.position = fistGFX.transform.position;
    }
}
