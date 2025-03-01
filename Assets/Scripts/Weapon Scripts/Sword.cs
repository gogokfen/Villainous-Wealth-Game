using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : WeaponBase
{
    [SerializeField] GameObject swordGFX;

    private void OnEnable()
    {
        swordGFX.SetActive(true);
    }

    private void OnDisable()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        swordGFX.SetActive(false);
    }

    private void Update()
    {
        transform.position = swordGFX.transform.position;
        transform.rotation = swordGFX.transform.rotation;
    }
}
