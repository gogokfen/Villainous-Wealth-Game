using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponConfig : ScriptableObject
{
    public WeaponBase.damageTypes DamageType;
    public int Damage;

    public GameObject Prefab;
    public string Name;
}

[CreateAssetMenu(fileName = "Sword Config", menuName = "Configs/Sword", order = 1)]
public class SwordConfig : WeaponConfig
{
    public bool WindSlashUpgrade;
}
