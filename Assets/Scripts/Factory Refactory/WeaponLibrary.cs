using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Library", menuName = "Configs/Weapon Lib", order = 1)]
public class WeaponLibrary : ScriptableObject
{
    public GameObject[] Prefabs;
}
