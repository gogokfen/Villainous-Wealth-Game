using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KillAllClones : MonoBehaviour
{
    public static void KillAllCharacters()
    {
        GameObject[] characters = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in characters)
        {
            if (obj.name == "Character(Clone)")
            {
                Destroy(obj);
            }
        }
    }
}
