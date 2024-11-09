using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : WeaponBase
{
    [SerializeField] GameObject fistGFX;
    SphereCollider SC;

    private void Start()
    {
        SC = transform.GetComponent<SphereCollider>();
    }
    private void OnEnable()
    {
        //fistGFX.SetActive(true);
        transform.GetComponent<SphereCollider>().enabled = true;
    }

    private void OnDisable()
    {
        //fistGFX.SetActive(false);
        transform.GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        //SC.transform.position = fistGFX.transform.position;
        transform.position = fistGFX.transform.position;
    }
}
