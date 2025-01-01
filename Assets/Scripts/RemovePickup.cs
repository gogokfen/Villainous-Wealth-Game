using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePickup : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 20);
    }

    private void OnDestroy()
    {
        PickupManager.singleton.RemovePickupFromList(gameObject);
    }

}
