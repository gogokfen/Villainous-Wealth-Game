using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : WeaponBase
{
    [SerializeField] GameObject fistGFX;
    private SphereCollider SC;

    private void Start()
    {
        SC = transform.GetComponent<SphereCollider>();
    }

    private void OnDisable()
    {
        transform.GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        transform.position = fistGFX.transform.position;
    }
}
