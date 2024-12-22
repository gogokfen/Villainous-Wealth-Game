using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory
{

    public WeaponBase Make(WeaponConfig config)
    {
        GameObject go = GameObject.Instantiate(config.Prefab);
        WeaponBase weapon = go.GetComponent<WeaponBase>();
        if (config is SwordConfig)
        {
            Sword sword = weapon as Sword;
            sword.SetConfig((SwordConfig)config);
        }



        return weapon;
    }
}
