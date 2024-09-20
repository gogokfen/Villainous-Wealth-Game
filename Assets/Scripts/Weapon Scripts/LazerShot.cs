using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerShot : WeaponBase
{
    [SerializeField] Transform GFX;
    void Start()
    {
        Destroy(gameObject, 0.35f);
    }

    void Update()
    {
        GFX.transform.localScale = new Vector3 (GFX.transform.localScale.x/(1 + Time.deltaTime*5f),20, GFX.transform.localScale.z / (1 + Time.deltaTime * 5f));
    }
}
