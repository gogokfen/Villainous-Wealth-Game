using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneProtect : MonoBehaviour
{
    private void OnTriggerStay(Collider other) //collider override searches for characters only
    {
        other.GetComponent<CharacterControl>().zoneImmunity = true;
        other.GetComponent<CharacterControl>().zoneTicksGraceAmount = 2;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<CharacterControl>().zoneImmunity = false;
    }
}
