using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class MineShot : WeaponBase
{
    [SerializeField] GameObject mineBaseGFX;
    [SerializeField] GameObject mineHeadGFX;
    [SerializeField] float timeToArm = 2;
    [SerializeField] GameObject radiusGFX;
    [SerializeField] ParticleSystem explosionEffect;
    private CapsuleCollider CC;
    void Start()
    {
        CC = GetComponent<CapsuleCollider>();
        CC.enabled = false;
        mineHeadGFX.GetComponent<Renderer>().material.color = Color.grey;

        Destroy(gameObject, 25);

        if (playerID == CharacterControl.PlayerTypes.Red)
        {
            mineBaseGFX.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (playerID == CharacterControl.PlayerTypes.Blue)
        {
            mineBaseGFX.GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (playerID == CharacterControl.PlayerTypes.Green)
        {
            mineBaseGFX.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (playerID == CharacterControl.PlayerTypes.Yellow)
        {
            mineBaseGFX.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    void Update()
    {
        if (timeToArm>0)
        {
            timeToArm -= Time.deltaTime;
            if (timeToArm<=0)
            {
                CC.enabled = true;
                mineHeadGFX.GetComponent<Renderer>().material.color = Color.red;
                radiusGFX.SetActive(true);
            }
        }
    }

    private void OnDestroy() 
    {
        explosionEffect.transform.SetParent(null);
        explosionEffect.Play();

        Destroy(explosionEffect.gameObject, 2f);
    }
}
